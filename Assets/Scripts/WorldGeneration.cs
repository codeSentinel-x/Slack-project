using System;
using System.Collections.Generic;
using MyUtils.Classes;
using MyUtils.Structs;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {


    public static WorldGeneration _instance;
    public static MultipleLayerNoiseSetting _currentSettings;
    public static int chunkSize;
    public Dictionary<Vector2Int, GameObject> _currentChunksDict = new();
    public Dictionary<Vector2Int, GameObject> _oldChunksDict = new();

    [SerializeField] private NoiseLayerSetting _humidityNoiseSettings;
    [SerializeField] private NoiseLayerSetting _temperatureNoiseSettings;
    [SerializeField] private BiomeAssets _biomeAsset;
    [SerializeField] private BiomeSO _biome;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private Material _sourceMaterial;
    private GameObject _chunkHolder;
    private NoiseGeneration _noiseGen;

    public Action<int, bool> _OnNoiseSettingChange;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        _noiseGen = NoiseGeneration._instance;
        NoiseGeneration._onNoiseGenerationCompleat += GenerateChunk;
        NoiseGeneration._onAdvanceNoiseMapGenerationCompleat += GenerateComplexChunk;
    }

    private void GenerateComplexChunk(float[,,] obj, Vector2Int start) {
        if (_chunkHolder == null) return;

        chunkSize = obj.GetLength(1); ;
        GameObject chunk = Instantiate(_chunkPrefab, new Vector3(start.x + chunkSize / 2, start.y + chunkSize / 2), Quaternion.identity).gameObject;
        ChunkController cT = chunk.GetComponent<ChunkController>();
        cT._chunks = new ChunkItem[obj.GetLength(1), obj.GetLength(2)];
        chunk.transform.localScale = new Vector3(chunkSize, chunkSize, 1);
        Texture2D colorTexture = new(chunkSize, chunkSize);
        Color[] chunkColors = new Color[chunkSize * chunkSize];
        BiomeSO biome;
        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float h = obj[0, x, y];
                h = Mathf.Clamp01(h);
                biome = _biomeAsset.GetBiomeSO(obj[1, x, y], obj[2, x, y]);
                for (int i = 0; i < biome._terrainTypes.Length; i++) {
                    if (biome._terrainTypes[i]._h >= h) {
                        float minH = i == 0 ? 0f : biome._terrainTypes[i - 1]._h;
                        float maxH = biome._terrainTypes[i]._h;
                        float localH = Mathf.InverseLerp(minH, maxH, h);
                        chunkColors[y * chunkSize + x] = biome._terrainTypes[i]._gradient.Evaluate(localH);
                        cT._chunks[x, y]._cellH = h;
                        cT._chunks[x, y]._terrainTypeName = biome._terrainTypes[i]._cellName;
                        cT._chunks[x, y]._isWalkable = biome._terrainTypes[i]._isWalkable;
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

        if (!_currentChunksDict.TryAdd(start / chunkSize, chunk)) {
            _currentChunksDict[start / chunkSize] = chunk;
        }

        chunk.transform.parent = _chunkHolder.transform;

    }

    public void GenerateChunks(MultipleLayerNoiseSetting mLNS, uint seed) {
        if (_chunkHolder == null) _chunkHolder = new("Chunk holder");
        else {
            Destroy(_chunkHolder);
            _chunkHolder = new("Chunk holder");
        }

        // chunks = new Transform[mLNS.chunkSize, mLNS.chunkSize];
        chunkSize = mLNS._chunkSize;
        NoiseGeneration._seed = seed;
        _currentSettings = mLNS;
        _OnNoiseSettingChange?.Invoke(mLNS._chunkCount, false);


    }
    public void GenerateAdvancedChunks(NoiseSettingData data) {
        if (_chunkHolder == null) _chunkHolder = new("Chunk holder");
        else {
            Destroy(_chunkHolder);
            _chunkHolder = new("Chunk holder");
        }
        chunkSize = data._settings._chunkSize;
        NoiseGeneration._seed = data._seed;
        _currentSettings = data._settings;
        _temperatureNoiseSettings = data._temperatureNoise;
        _humidityNoiseSettings = data._humidityNoise;
        _OnNoiseSettingChange?.Invoke(data._settings._chunkCount, true);


    }

    public void GenerateChunkAt(Vector2Int offset) {
        _noiseGen.GenerateNoise(_currentSettings, offset * chunkSize);
    }

    public void GenerateAdvancedChunkAt(Vector2Int offset) {
        _noiseGen.GenerateNoise(_currentSettings, _temperatureNoiseSettings, _humidityNoiseSettings, offset * chunkSize);
    }
    public void DestroyChunkAt() {

    }

    public void DestroyWorld() {
        if (_chunkHolder != null) Destroy(_chunkHolder);
    }
    private void GenerateChunk(float[,] obj, Vector2Int start) {
        if (_chunkHolder == null) return;

        chunkSize = obj.GetLength(0);
        GameObject chunk = Instantiate(_chunkPrefab, new Vector3(start.x + chunkSize / 2, start.y + chunkSize / 2), Quaternion.identity).gameObject;
        ChunkController cT = chunk.GetComponent<ChunkController>();
        cT._chunks = new ChunkItem[obj.GetLength(0), obj.GetLength(1)];
        chunk.transform.localScale = new Vector3(chunkSize, chunkSize, 1);
        Texture2D colorTexture = new(chunkSize, chunkSize);
        Color[] chunkColors = new Color[chunkSize * chunkSize];
        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float h = obj[x, y];
                h = Mathf.Clamp01(h);
                for (int i = 0; i < _biome._terrainTypes.Length; i++) {
                    if (_biome._terrainTypes[i]._h >= h) {
                        float minH = i == 0 ? 0f : _biome._terrainTypes[i - 1]._h;
                        float maxH = _biome._terrainTypes[i]._h;
                        float localH = Mathf.InverseLerp(minH, maxH, h);
                        chunkColors[y * chunkSize + x] = _biome._terrainTypes[i]._gradient.Evaluate(localH);
                        cT._chunks[x, y]._cellH = h;
                        cT._chunks[x, y]._terrainTypeName = _biome._terrainTypes[i]._cellName;
                        cT._chunks[x, y]._isWalkable = _biome._terrainTypes[i]._isWalkable;
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

        if (!_currentChunksDict.TryAdd(start / chunkSize, chunk)) {
            _currentChunksDict[start / chunkSize] = chunk;
        }

        chunk.transform.parent = _chunkHolder.transform;

    }

}


