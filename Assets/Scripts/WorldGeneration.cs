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
                    sprR.color = _biome._heightMap.Evaluate(h);
                }
            }
        }
        else {
            int mapSize = obj.GetLength(0);
            chunkSize = mapSize;
            GameObject chunk = Instantiate(_chunkPrefab, new Vector3(start.x, start.y), Quaternion.identity).gameObject;
            chunk.transform.localScale = new Vector3(mapSize, mapSize, 1);
            Texture2D texture = new(mapSize, mapSize);
            Color[] chunkColor = new Color[mapSize * mapSize];
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    chunkColor[y * mapSize + x] = _biome._heightMap.Evaluate(obj[x, y]);
                }
            }
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(chunkColor);
            texture.Apply();
            (chunk.GetComponent<MeshRenderer>().material = new Material(_sourceMaterial)).mainTexture = texture;
            chunk.transform.parent = chunkHolder.transform;
        }
    }

}
