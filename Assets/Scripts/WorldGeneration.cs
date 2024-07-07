using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGeneration : MonoBehaviour {
    public NoiseSetting _noiseSetting;
    public FalloffSetting _falloffSetting;
    public bool _applyFalloff;
    public Transform _prefab;
    public Color[] _colors;
    public Chunk<GameObject>[,] chunks;
    public GameObject[,] tiles;

    private NoiseGeneration noiseGen;
    private GameObject tileHolder;
    void Start() {
        noiseGen = NoiseGeneration._instance;
        noiseGen._onNoiseGenerationCompleat += GenerateWorld;
        GenerateWorld();
    }

    public void GenerateWorld() {
        if (_applyFalloff) noiseGen.GenerateNoise(_noiseSetting, _falloffSetting);
        else noiseGen.GenerateNoise(_noiseSetting);
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
                Transform t = Instantiate(_prefab, new Vector2(x, y), Quaternion.identity).transform;
                SpriteRenderer sprR = t.GetComponent<SpriteRenderer>();
                t.parent = tileHolder.transform;
                tiles[x, y] = t.gameObject;
                // chunks[x, y] = sprR.
                float h = obj[x, y];
                sprR.color = h switch {
                    _ when 0.1f >= h => _colors[0],
                    _ when 0.2f >= h => _colors[1],
                    _ when 0.3f >= h => _colors[2],
                    _ when 0.4f >= h => _colors[3],
                    _ when 0.5f >= h => _colors[4],
                    _ when 0.6f >= h => _colors[5],
                    _ when 0.7f >= h => _colors[6],
                    _ when 0.8f >= h => _colors[7],
                    _ when 0.9f >= h => _colors[8],
                    _ when 1f >= h => _colors[9],
                    _ => _colors[9],
                };

            }
        }
    }
}
