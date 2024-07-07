using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {


    public NoiseSetting _noiseSetting;
    public FalloffSetting _falloffSetting;
    public BiomeSO _biome;
    public int _mapSizeInChunk;
    public bool _applyFalloff;
    public bool _changeSeed;
    public Transform _prefab;
    private NoiseGeneration noiseGen;
    private GameObject tileHolder;
    private GameObject chunkHolder;

    void Start() {
        noiseGen = NoiseGeneration._instance;
        noiseGen._onNoiseGenerationCompleat += GenerateWorld;
        noiseGen._onPartNoiseGenerationCompleat += GenerateChunk;
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
                noiseGen.GeneratePartOfNoise(_noiseSetting, _biome.heightCurve, new Vector2Int(i * _noiseSetting.mapSize, j * _noiseSetting.mapSize));
            }
        }
    }

    public void GenerateWorld() {
        if (_changeSeed) NoiseGeneration.RefreshSeed();
        if (_applyFalloff) noiseGen.GenerateNoise(_noiseSetting, _falloffSetting, _biome.heightCurve);
        else noiseGen.GenerateNoise(_noiseSetting, _biome.heightCurve);
    }
    private void GenerateWorld(float[,] obj) {

        if (tileHolder == null) tileHolder = new GameObject();
        else {
            Destroy(tileHolder);
            tileHolder = new();
        }
        for (int x = 0; x < obj.GetLength(0); x++) {
            for (int y = 0; y < obj.GetLength(1); y++) {
                Transform t = Instantiate(_prefab, new Vector2(x, y), Quaternion.identity);
                SpriteRenderer sprR = t.GetComponent<SpriteRenderer>();
                t.parent = tileHolder.transform;
                float h = obj[x, y];
                sprR.color = _biome._heightMap.Evaluate(h);

            }
        }
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
