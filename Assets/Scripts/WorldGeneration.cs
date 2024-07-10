using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MyUtils.Structs;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {
    public static WorldGeneration _instance;
    public static MultipleLayerNoiseSetting currentSettings;
    public static int chunkSize;

    [SerializeField] private BiomeSO _biome;
    public Transform _chunkPrefab;
    public Material _sourceMaterial;
    public Dictionary<Vector2Int, GameObject> chunks = new();
    private NoiseGeneration noiseGen;
    public GameObject chunkHolder;


    void Awake() {
        _instance = this;
    }
    void Start() {
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

        NoiseGeneration.seed = seed;
        currentSettings = mLNS;
        for (int i = 0; i < mLNS.chunkCount; i++) {
            for (int j = 0; j < mLNS.chunkCount; j++) {
                noiseGen.GenerateNoise(mLNS, new Vector2Int(i * mLNS.chunkSize, j * mLNS.chunkSize));
            }
        }

    }
    public void GenerateChunkAt() {

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
        cT.chunkH = new ChunkItem[obj.GetLength(0), obj.GetLength(1)];
        chunk.transform.localScale = new Vector3(chunkSize, chunkSize, 1);
        Texture2D colorTexture = new(chunkSize, chunkSize);
        Color[] chunkColors = new Color[chunkSize * chunkSize];
        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float h = obj[x, y];
                h = Mathf.Clamp01(h);
                for (int i = 0; i < _biome.terrainTypes.Length; i++) {
                    if (_biome.terrainTypes[i].h >= h) {
                        float minH = i == 0 ? 0f : _biome.terrainTypes[i - 1].h;
                        float maxH = _biome.terrainTypes[i].h;
                        float localH = Mathf.InverseLerp(minH, maxH, h);
                        chunkColors[y * chunkSize + x] = _biome.terrainTypes[i].gradient.Evaluate(localH);
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

        chunks.Add(start, chunk);
        chunk.transform.parent = chunkHolder.transform;

    }

}


