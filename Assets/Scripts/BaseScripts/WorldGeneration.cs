using System;
using System.Collections.Generic;
using MyUtils.Classes;
using MyUtils.Structs;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {


    public static WorldGeneration _instance;
    public static NoiseSettingData _currentSettingsData;
    public static int _chunkSize;
    public Dictionary<Vector2Int, GameObject> _currentChunksDict = new();
    public Dictionary<Vector2Int, GameObject> _oldChunksDict = new();


    public NoiseSettingData data;
    [SerializeField] private BiomeAssets _biomeAsset;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private Material _sourceMaterial;
    private GameObject _chunkHolder;
    public Action<int> _OnNoiseSettingChange;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        NoiseGeneration._onAdvanceNoiseMapGenerationCompleat += GenerateComplexChunk;
    }

    private void GenerateComplexChunk(float[,,] obj, Vector2Int start) {
        if (_chunkHolder == null) return;

        _chunkSize = obj.GetLength(1); ;
        GameObject chunk = Instantiate(_chunkPrefab, new Vector3(start.x + _chunkSize / 2, start.y + _chunkSize / 2), Quaternion.identity).gameObject;
        ChunkController cT = chunk.GetComponent<ChunkController>();
        cT._chunks = new ChunkItem<int>[obj.GetLength(1), obj.GetLength(2)];
        chunk.transform.localScale = new Vector3(_chunkSize, _chunkSize, 1);
        Texture2D colorTexture = new(_chunkSize, _chunkSize);
        Color[] chunkColors = new Color[_chunkSize * _chunkSize];
        BiomeSO biome;
        for (int y = 0; y < _chunkSize; y++) {
            for (int x = 0; x < _chunkSize; x++) {
                float h = obj[0, x, y];
                h = Mathf.Clamp01(h);
                biome = _biomeAsset.GetBiomeSO(obj[1, x, y], obj[2, x, y]);
                for (int i = 0; i < biome._terrainRule.Length; i++) {
                    if (biome._terrainRule[i]._maxHeight >= h) {
                        float minH = i == 0 ? 0f : biome._terrainRule[i - 1]._maxHeight;
                        float maxH = biome._terrainRule[i]._maxHeight;
                        float localH = Mathf.InverseLerp(minH, maxH, h);
                        chunkColors[y * _chunkSize + x] = biome._terrainRule[i]._gradient.Evaluate(localH);
                        cT._chunks[x, y]._cellHeight = h;
                        cT._chunks[x, y]._terrainTypeName = biome._terrainRule[i]._cellName;
                        cT._chunks[x, y]._isWalkable = biome._terrainRule[i]._isWalkable;
                        break;
                    }
                }

            }
        }

        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.SetPixels(chunkColors);
        colorTexture.Apply();


        (chunk.GetComponent<MeshRenderer>().material = new Material(_sourceMaterial)).mainTexture = colorTexture;

        if (!_currentChunksDict.TryAdd(start / _chunkSize, chunk)) {
            _currentChunksDict[start / _chunkSize] = chunk;
        }

        chunk.transform.parent = _chunkHolder.transform;

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

        _OnNoiseSettingChange?.Invoke(data._settings._chunkCount);


    }
    public void GenerateAdvancedChunks() {
        if (_chunkHolder == null) _chunkHolder = new("Chunk holder");
        else {
            Destroy(_chunkHolder);
            _chunkHolder = new("Chunk holder");
        }
        _currentSettingsData = data;
        NoiseGeneration._seed = data._seed;
        _chunkSize = data._settings._chunkSize;

        _OnNoiseSettingChange?.Invoke(data._settings._chunkCount);


    }
    public void GenerateAdvancedChunkAt(Vector2Int offset) {
        NoiseGeneration.GenerateNoiseMap(_currentSettingsData, offset * _chunkSize);
    }

    public void DestroyWorld() {
        if (_chunkHolder != null) Destroy(_chunkHolder);
    }
}

