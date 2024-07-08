using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {

    public static WorldGeneration _instance;

    public NoiseSetting _noiseSetting;
    public FalloffSetting _falloffSetting;
    public BiomeSO _biome;
    public int _mapSizeInChunk;
    public bool _applyFalloff;
    public bool _changeSeed;
    public Transform _prefab;
    public AnimationCurve defaultCurve;

    private NoiseGeneration noiseGen;
    private GameObject chunkHolder;
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
                noiseGen.GenerateNoise(_noiseSetting, _biome.heightCurve, new Vector2Int(i * _noiseSetting.mapSize, j * _noiseSetting.mapSize));
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
                noiseGen.GenerateNoise(nS, _biome.heightCurve, new Vector2Int(i * nS.mapSize, j * nS.mapSize));
            }
        }
    }

    public void DestroyWorld() {
        if (chunkHolder != null) Destroy(chunkHolder);
    }
    private void GenerateChunk(float[,] obj, Vector2Int start) {
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
}
