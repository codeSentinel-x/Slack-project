using System;
using System.Collections.Generic;
using MyUtils;
using MyUtils.Classes;
using MyUtils.Structs;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {


    public static WorldGeneration _instance;
    public static NoiseSettingData _currentSettingsData;
    public static int _chunkSize;
    public static Unity.Mathematics.Random _rng;
    public Dictionary<Vector2Int, GameObject> _currentChunksDict = new();
    public Dictionary<Vector2Int, GameObject> _oldChunksDict = new();

    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private Material _sourceMaterial;
    [SerializeField] private WorldGenerationSettingSO _settingSource;
    private GameObject _chunkHolder;
    public Action<int> _OnNoiseSettingChange;

    private void Awake() {
        _instance = this;
        _OnNoiseSettingChange += (x) => _rng = new(_currentSettingsData._seed);
    }


    private void GenerateComplexChunk(float[,,] obj, Vector2Int start) {
        Debug.Log("Generating");
        if (_chunkHolder == null) return;
        _chunkSize = obj.GetLength(1); ;
        GameObject chunk = Instantiate(_chunkPrefab, new Vector3(start.x + _chunkSize / 2, start.y + _chunkSize / 2), Quaternion.identity).gameObject;
        ChunkController cT = chunk.GetComponent<ChunkController>();
        cT._chunks = new ChunkItem<GameObject>[obj.GetLength(1), obj.GetLength(2)];
        chunk.transform.localScale = new Vector3(_chunkSize, _chunkSize, 1);
        Texture2D colorTexture = new(_chunkSize, _chunkSize);
        Color32[] chunkColors = new Color32[_chunkSize * _chunkSize];
        BiomeSO biome;


        for (int y = 0; y < _chunkSize; y++) {
            for (int x = 0; x < _chunkSize; x++) {

                float h = obj[0, x, y];
                h = Mathf.Clamp01(h);

                //GET BIOME VALUE BASE ON TEMPERATURE AND HUMIDITY NOISE
                biome = _settingSource._biomeAssetSource.GetBiomeSO2(obj[1, x, y], obj[2, x, y]);

                for (int i = 0; i < biome._terrainRule.Length; i++) {
                    if (biome._terrainRule[i]._maxHeight >= h) {

                        float minH = i == 0 ? 0f : biome._terrainRule[i - 1]._maxHeight;
                        float maxH = biome._terrainRule[i]._maxHeight;

                        float localH = Mathf.InverseLerp(minH, maxH, h);

                        //SET PIXEL COLOR BASE ON EVALUATED GRADIENT VALUE FROM BIOME SETTING
                        chunkColors[y * _chunkSize + x] = biome._terrainRule[i]._gradient.Evaluate(localH);

                        //APPLY TERRAIN RULES FROM BIOME ASSET TO CHUNK ITEM
                        cT._chunks[x, y] = new() {
                            _cellHeight = h,
                            biomeName = biome.name,
                            _terrainTypeName = biome._terrainRule[i]._cellName,
                            _isWalkable = biome._terrainRule[i]._isWalkable,
                            isEmpty = true
                        };

                        break;
                    }
                }

            }
        }

        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.SetPixels32(chunkColors);
        colorTexture.Apply();


        (chunk.GetComponent<MeshRenderer>().material = new Material(_sourceMaterial)).mainTexture = colorTexture;

        if (!_currentChunksDict.TryAdd(start / _chunkSize, chunk)) {
            _currentChunksDict[start / _chunkSize] = chunk;
        }

        chunk.transform.parent = _chunkHolder.transform;
        NoiseGeneration.GenerateEnvironmentNoiseMap(_currentSettingsData, start, (x, y) => SpawnEnvironment(x, y));
    }
    private void SpawnEnvironment(float[,] noiseResult, Vector2Int start) {
        GameObject chunk = _instance._currentChunksDict[start / _chunkSize];
        Debug.Log(chunk.name);
        var cArray = chunk.GetComponent<ChunkController>()._chunks;
        for (int x = 0; x < cArray.GetLength(0); x++) {
            for (int y = 0; y < cArray.GetLength(1); y++) {
                var t = cArray[x, y];
                if (!t.isEmpty) continue;
                foreach (SpawnRuleSO eR in _settingSource._environmentRuleSource._rulesSO) {
                    foreach (EnviElementRuleSO enviRule in eR._enviRules) {
                        foreach (var r in enviRule._rules) {
                            if (t.biomeName == r._biome.name || enviRule._useFirstSettingForAllBiomes) {
                                if (r._minWorldHeight < t._cellHeight && t._cellHeight < r._maxWorldHeight) {
                                    if (r._minNoiseHeight < noiseResult[x, y] && noiseResult[x, y] < r._maxNoiseHeight) {
                                        if (r.CanBeSpawned()) {
                                            if (!AreEmpty(cArray, x, y, enviRule._size)) continue;
                                            Transform obj = Instantiate(enviRule._prefab, new Vector3(x + start.x, start.y + y), Quaternion.identity).transform;
                                            obj.SetParent(chunk.transform);
                                            obj.GetComponentInChildren<SpriteRenderer>().sprite = MyRandom.GetFromArray<Sprite>(r._spriteVariants);
                                            SetEmpty(cArray, x, y, enviRule._size);
                                        }
                                    }
                                }
                                continue;

                            }
                        }
                    }
                }
            }
        }
    }

    public bool AreEmpty(ChunkItem<GameObject>[,] cArray, int x, int y, int range) {
        int rowMin = Math.Max(0, x - range);
        int rowMax = Math.Min(cArray.GetLength(0) - 1, x + range);
        int colMin = Math.Max(0, y - range);
        int colMax = Math.Min(cArray.GetLength(1) - 1, y + range);

        for (int i = rowMin; i <= rowMax; i++) {
            for (int j = colMin; j <= colMax; j++) {
                if (!cArray[i, j].isEmpty) return false;
            }
        }

        return true;
    }
    public void SetEmpty(ChunkItem<GameObject>[,] cArray, int x, int y, int range) {
        int rowMin = Math.Max(0, x - range);
        int rowMax = Math.Min(cArray.GetLength(0) - 1, x + range);
        int colMin = Math.Max(0, y - range);
        int colMax = Math.Min(cArray.GetLength(1) - 1, y + range);

        for (int i = rowMin; i <= rowMax; i++) {
            for (int j = colMin; j <= colMax; j++) {
                cArray[i, j].isEmpty = false;
            }
        }

    }
    public void GenerateAdvancedChunks(NoiseSettingData data) {
        if (_chunkHolder == null) _chunkHolder = new("Chunk holder");
        else {
            Destroy(_chunkHolder);
            _chunkHolder = new("Chunk holder");
        }
        _chunkSize = data._settings._chunkSize;
        NoiseGeneration._seed = data._seed;
        _currentSettingsData = data;

        _OnNoiseSettingChange?.Invoke(data._settings._chunkRenderDistance);


    }
    public void GenerateAdvancedChunks() {
        if (_chunkHolder == null) _chunkHolder = new("Chunk holder");
        else {
            Destroy(_chunkHolder);
            _chunkHolder = new("Chunk holder");
        }
        _currentSettingsData = _settingSource._dataSource._data;
        NoiseGeneration._seed = _currentSettingsData._seed;
        _chunkSize = _currentSettingsData._settings._chunkSize;

        _OnNoiseSettingChange?.Invoke(_currentSettingsData._settings._chunkRenderDistance);


    }
    public void GenerateAdvancedChunkAt(Vector2Int offset) {
        NoiseGeneration.GenerateNoiseMap(_currentSettingsData, offset * _chunkSize, (x, y) => GenerateComplexChunk(x, y));
    }
    public void TestSpeed() {
        NoiseGeneration._seed = _currentSettingsData._seed;
        _chunkSize = _currentSettingsData._settings._chunkSize;

        System.Diagnostics.Stopwatch stopwatch = new();
        stopwatch.Start();
        NoiseGeneration.GenerateNoiseMap(_currentSettingsData, Vector2Int.zero * _chunkSize, (x, y) => GenerateComplexChunk(x, y));
        stopwatch.Stop();
        long normalTime = stopwatch.ElapsedTicks;
        UnityEngine.Debug.Log("Normal time: " + normalTime + " ticks");

        stopwatch.Reset();

        stopwatch.Start();
        NoiseGeneration.GenerateNoiseMapTest(_currentSettingsData, Vector2Int.zero * _chunkSize, (x, y) => GenerateComplexChunk(x, y));
        stopwatch.Stop();
        long parallelTime = stopwatch.ElapsedTicks;
        UnityEngine.Debug.Log("Parallel time: " + parallelTime + " ticks");



        if (parallelTime < normalTime) {
            UnityEngine.Debug.Log("Parallel is faster.");
        }
        else {
            UnityEngine.Debug.Log("Normal is faster.");
        }


    }
    public void DestroyWorld() {
        if (_chunkHolder != null) Destroy(_chunkHolder);
    }

    public void GenerateEnviVisual() {
        NoiseGeneration.GenerateEnvironmentNoiseMap(_currentSettingsData, new Vector2Int(1000, 1000), (x, y) => {
        
        });
    }
}


