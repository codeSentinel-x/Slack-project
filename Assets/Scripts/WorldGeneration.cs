using System;
using System.Collections.Generic;
using MyUtils.Classes;
using MyUtils.Structs;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {


    public static int _chunkSize;
    public static WorldGeneration _instance;
    public static MultipleLayerNoiseSetting _currentSettings;
    public Dictionary<Vector2Int, GameObject> _currentChunksDict = new();
    public Dictionary<Vector2Int, GameObject> _oldChunksDict = new();

    [SerializeField] private NoiseLayerSetting _humidityNoiseSettings;
    [SerializeField] private NoiseLayerSetting _temperatureNoiseSettings;
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
        cT._chunks = new ChunkItem[obj.GetLength(1), obj.GetLength(2)];
        chunk.transform.localScale = new Vector3(_chunkSize, _chunkSize, 1);
        Texture2D colorTexture = new(_chunkSize, _chunkSize);
        Color[] chunkColors = new Color[_chunkSize * _chunkSize];
        BiomeSO biome;
        for (int y = 0; y < _chunkSize; y++) {
            for (int x = 0; x < _chunkSize; x++) {
                float h = obj[0, x, y];
                h = Mathf.Clamp01(h);
                biome = _biomeAsset.GetBiomeSO(obj[1, x, y], obj[2, x, y]);
                for (int i = 0; i < biome._terrainRules.Length; i++) {
                    if (biome._terrainRules[i]._maxHeight >= h) {
                        float minH = i == 0 ? 0f : biome._terrainRules[i - 1]._maxHeight;
                        float maxH = biome._terrainRules[i]._maxHeight;
                        float localH = Mathf.InverseLerp(minH, maxH, h);
                        chunkColors[y * _chunkSize + x] = biome._terrainRules[i]._gradient.Evaluate(localH);
                        cT._chunks[x, y]._cellH = h;
                        cT._chunks[x, y]._terrainTypeName = biome._terrainRules[i]._cellName;
                        cT._chunks[x, y]._isWalkable = biome._terrainRules[i]._isWalkable;
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
        _currentSettings = data._settings;
        _temperatureNoiseSettings = data._temperatureNoise;
        _humidityNoiseSettings = data._humidityNoise;
        _OnNoiseSettingChange?.Invoke(data._settings._chunkCount);


    }

    public void GenerateAdvancedChunkAt(Vector2Int offset) {
        NoiseGeneration.GenerateNoiseNap(_currentSettings, _temperatureNoiseSettings, _humidityNoiseSettings, offset * _chunkSize);
    }

    public void DestroyWorld() {
        if (_chunkHolder != null) Destroy(_chunkHolder);
    }
}


