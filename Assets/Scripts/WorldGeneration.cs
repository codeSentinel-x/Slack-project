using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {


    public BiomeSO _biome;
    public NoiseSetting _noiseSetting;
    public FalloffSetting _falloffSetting;
    public bool _applyFalloff;
    public Transform _prefab;
    public Chunk<GameObject>[,] chunks;
    public GameObject[,] tiles;
    public bool changeSeed;
    private NoiseGeneration noiseGen;
    private GameObject tileHolder;
    void Start() {
        noiseGen = NoiseGeneration._instance;
        noiseGen._onNoiseGenerationCompleat += GenerateWorld;
        GenerateWorld();
    }

    public void GenerateWorld() {
        if (changeSeed) NoiseGeneration.RefreshSeed();
        if (_applyFalloff) noiseGen.GenerateNoise(_noiseSetting, _falloffSetting, _biome.heightCurve);
        else noiseGen.GenerateNoise(_noiseSetting, _biome.heightCurve);
    }
    private void GenerateWorld(float[,] obj) {

        if (tileHolder == null) tileHolder = new GameObject();
        else {
            Destroy(tileHolder);
            tileHolder = new();
        }
        chunks = new Chunk<GameObject>[obj.GetLength(0) / 20, obj.GetLength(1) / 20];
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
}
