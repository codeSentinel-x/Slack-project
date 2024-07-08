using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {

    public enum WorldGenerationMode {
        Normal,
        ChunkAsSquareWithTexture
    }
    public static WorldGeneration _instance;
    public static int chunkSize;
    public NoiseSetting _noiseSetting;
    public WorldGenerationMode _mode;
    public FalloffSetting _falloffSetting;
    public BiomeSO _biome;
    public int _mapSizeInChunk;
    public bool _applyFalloff;
    public bool _changeSeed;
    public Transform _prefab;
    public Transform _chunkPrefab;
    public Material _sourceMaterial;
    public AnimationCurve defaultCurve;
    public Transform[,] chunks;
    private NoiseGeneration noiseGen;
    public GameObject chunkHolder;
    void Awake() {
        _instance = this;
    }
    void Start() {

        noiseGen = NoiseGeneration._instance;
        noiseGen._onNoiseGenerationCompleat += GenerateChunk;
    }


    public void GenerateChunks() {
        if (chunkHolder == null) chunkHolder = new("Chunk holder");
        else {
            Destroy(chunkHolder);
            chunkHolder = new("Chunk holder");
        }
        chunks = new Transform[chunkSize, chunkSize];

        if (_changeSeed) NoiseGeneration.RefreshSeed();
        for (int i = 0; i < _mapSizeInChunk; i++) {
            for (int j = 0; j < _mapSizeInChunk; j++) {
                noiseGen.GenerateNoise(_noiseSetting, defaultCurve, new Vector2Int(i * _noiseSetting.mapSize, j * _noiseSetting.mapSize));
            }
        }
    }
    public void GenerateChunks(NoiseSetting nS, uint seed, int chunkSize) {
        if (chunkHolder == null) chunkHolder = new("Chunk holder");
        else {
            Destroy(chunkHolder);
            chunkHolder = new("Chunk holder");
        }
        chunks = new Transform[chunkSize, chunkSize];

        NoiseGeneration.seed = seed;
        for (int i = 0; i < chunkSize; i++) {
            for (int j = 0; j < chunkSize; j++) {
                noiseGen.GenerateNoise(nS, defaultCurve, new Vector2Int(i * nS.mapSize, j * nS.mapSize));
            }
        }
    }

    public void DestroyWorld() {
        if (chunkHolder != null) Destroy(chunkHolder);
    }
    private void GenerateChunk(float[,] obj, Vector2Int start) {
        if (_mode == WorldGenerationMode.Normal) {
            GameObject chunkHolder = new(start.ToString());
            chunkHolder.transform.parent = this.chunkHolder.transform;
            for (int x = 0; x < obj.GetLength(0); x++) {
                for (int y = 0; y < obj.GetLength(1); y++) {
                    Transform t = Instantiate(_prefab, start + new Vector2(x, y), Quaternion.identity);
                    SpriteRenderer sprR = t.GetComponent<SpriteRenderer>();
                    t.parent = chunkHolder.transform;
                    float h = obj[x, y];
                    for (int i = 0; i < _biome.terrainTypes.Length; i++) {
                        if (_biome.terrainTypes[i].h >= h) {
                            float minH = i == 0 ? 0f : _biome.terrainTypes[i - 1].h;
                            float maxH = i == _biome.terrainTypes.Length - 1 ? 1 : _biome.terrainTypes[i + 1].h;
                            float localH = Mathf.InverseLerp(minH, maxH, h);
                            sprR.color = _biome.terrainTypes[i].gradient.Evaluate(localH);
                            break;
                        }
                    }
                }
            }
        }
        else {
            int mapSize = obj.GetLength(0);
            chunkSize = mapSize;
            GameObject chunk = Instantiate(_chunkPrefab, new Vector3(start.x + mapSize / 2, start.y + mapSize / 2), Quaternion.identity).gameObject;
            ChunkController cT = chunk.GetComponent<ChunkController>();
            cT.chunkH = new ChunkItem[obj.GetLength(0), obj.GetLength(1)];
            chunk.transform.localScale = new Vector3(mapSize, mapSize, 1);
            Texture2D colorTexture = new(mapSize, mapSize);
            Color[] chunkColors = new Color[mapSize * mapSize];
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    float h = obj[x, y];
                    for (int i = 0; i < _biome.terrainTypes.Length; i++) {
                        if (_biome.terrainTypes[i].h >= h) {
                            float minH = i == 0 ? 0f : _biome.terrainTypes[i - 1].h;
                            float maxH = _biome.terrainTypes[i].h;
                            float localH = Mathf.InverseLerp(minH, maxH, h);
                            chunkColors[y * mapSize + x] = _biome.terrainTypes[i].gradient.Evaluate(localH);
                            cT.chunkH[x, y].h = h;
                            cT.chunkH[x, y].name = _biome.terrainTypes[i].name;
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

            chunks[start.x / chunkSize, start.y / chunkSize] = chunk.transform;
            chunk.transform.parent = chunkHolder.transform;
        }
    }
}


