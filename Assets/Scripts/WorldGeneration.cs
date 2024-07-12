using System;
using System.Collections.Generic;
using MyUtils.Structs;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {
    public static WorldGeneration _instance;
    public static MultipleLayerNoiseSetting currentSettings;
    public static int chunkSize;

    [SerializeField] private BiomeSO _biome;
    public Transform _chunkPrefab;
    public Material _sourceMaterial;
    public Dictionary<Vector2Int, GameObject> chunks = new();
    public Dictionary<Vector2Int, GameObject> oldChunks = new();
    private NoiseGeneration noiseGen;
    public GameObject chunkHolder;
    public Action<int> OnNoiseSettingChange;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        noiseGen = NoiseGeneration._instance;
        noiseGen._onNoiseGenerationCompleat += GenerateChunk;
    }

    public void GenerateChunks(MultipleLayerNoiseSetting mLNS, uint seed) {
        if (chunkHolder == null) chunkHolder = new("Chunk holder");
        else {
            Destroy(chunkHolder);
            chunkHolder = new("Chunk holder");
        }

        // chunks = new Transform[mLNS.chunkSize, mLNS.chunkSize];
        chunkSize = mLNS._chunkSize;
        NoiseGeneration._seed = seed;
        currentSettings = mLNS;
        OnNoiseSettingChange?.Invoke(mLNS._chunkCount);


    }
    public void GenerateChunkAt(Vector2Int offset) {
        noiseGen.GenerateNoise(currentSettings, offset * chunkSize);
    }
    public void DestroyChunkAt() {

    }

    public void DestroyWorld() {
        if (chunkHolder != null) Destroy(chunkHolder);
    }
    private void GenerateChunk(float[,] obj, Vector2Int start) {
        if (chunkHolder == null) return;

        chunkSize = obj.GetLength(0); ;
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

        if (!chunks.TryAdd(start / chunkSize, chunk)) {
            chunks[start / chunkSize] = chunk;
        }

        chunk.transform.parent = chunkHolder.transform;

    }

}


