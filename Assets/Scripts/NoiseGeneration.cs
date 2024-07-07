using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Structs;
using Unity.Jobs;
using Unity.Collections;
using System;
using Unity.Burst;
using Unity.Mathematics;
public class NoiseGeneration : MonoBehaviour {
    public NoiseSetting _noiseSetting;
    public FalloffSetting _falloffSetting;
    public static NoiseGeneration _instance;
    public Action<float[,]> _onNoiseGenerationCompleat;
    private JobHandle _noiseJobH, _falloffJobH;
    private NativeList<float> _noiseMapResult, _falloffMapResult;
    public IEnumerator GenerateNoiseWithFalloff() {
        _noiseMapResult = new(Allocator.TempJob);
        _falloffMapResult = new(Allocator.TempJob);
        GenerateNoiseMapJob noiseGenJob = new() {
            result = _noiseMapResult,
            seed = _noiseSetting.seed,
            mapSize = _noiseSetting.mapSize,
            lacunarity = _noiseSetting.lacunarity,
            scale = _noiseSetting.scale,
            octaves = _noiseSetting.octaves,
            persistance = _noiseSetting.persistance,
            offset = new(_noiseSetting.offset.x, _noiseSetting.offset.y)
        };
        GenerateFalloffMapJob falloffGenJob = new() {
            result = _falloffMapResult,
            size = _noiseSetting.mapSize,
            a = _falloffSetting.a,
            b = _falloffSetting.b,
        };
        _noiseJobH = noiseGenJob.Schedule();
        _falloffJobH = falloffGenJob.Schedule();

        _noiseJobH.Complete();
        _falloffJobH.Complete();

        while (!_noiseJobH.IsCompleted || !_falloffJobH.IsCompleted) {
            yield return new WaitForSeconds(0.5f);
        }
        float[,] generatedResult = new float[_noiseSetting.mapSize, _noiseSetting.mapSize];
        for (int x = 0; x < generatedResult.GetLength(0); x++) {
            for (int y = 0; y < generatedResult.GetLength(1); y++) {
                generatedResult[x, y] = Mathf.Clamp01(_noiseMapResult[x + _noiseSetting.mapSize * y] - _falloffMapResult[x + _noiseSetting.mapSize * y]);
            }
        }
        _noiseMapResult.Dispose();
        _falloffMapResult.Dispose();
        _onNoiseGenerationCompleat?.Invoke(generatedResult);
    }

    [BurstCompile]
    public struct GenerateNoiseMapJob : IJob {

        public NativeList<float> result;
        public uint seed;
        public int mapSize;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public float2 offset;
        public void Execute() {
            // result = new(Allocator.TempJob);
            Unity.Mathematics.Random rng = new(seed);
            // System.Random rng = new(seed);
            // NativeList<float> map = new(Allocator.Temp);
            NativeList<float2> octaveOffset = new(Allocator.Temp);
            for (int x = 0; x < mapSize; x++) {
                for (int y = 0; y < mapSize; y++) {
                    result.Add(0);
                }
            }
            for (int i = 0; i < octaves; i++) octaveOffset.Add(0);

            for (int i = 0; i < octaves; i++) {
                float x = rng.NextInt(-100000, 100000) + offset.x;
                float y = rng.NextInt(-100000, 100000) + offset.y;
                octaveOffset[i] = new float2(x, y);
            }

            float maxNoiseH = float.MinValue;
            float minNoiseH = float.MaxValue;

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseH = 0f;

                    for (int i = 0; i < octaves; i++) {

                        float2 sample = new((x - mapSize / 2) / scale * frequency + octaveOffset[i].x,
                                            (y - mapSize / 2) / scale * frequency + octaveOffset[i].y);

                        float perlinValue = Mathf.PerlinNoise(sample.x, sample.y) * 2 - 1;
                        noiseH += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseH > maxNoiseH) maxNoiseH = noiseH;
                    else if (noiseH < minNoiseH) minNoiseH = noiseH;

                    result[x + y * mapSize] = noiseH;
                }
            }
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    result[x + y * mapSize] = InverseLerp(minNoiseH, maxNoiseH, result[x + y * mapSize]);
                }
            }
        }
        public readonly float InverseLerp(float p1, float p2, float v) {
            float factor = (v - p1) / (p2 - p1);
            return factor;
        }
    }
    [BurstCompile]
    public struct GenerateFalloffMapJob : IJob {
        public NativeList<float> result;
        public int size;
        public float a;
        public float b;
        public void Execute() {
            for (int i = 0; i < size; i++) for (int j = 0; j < size; j++) result.Add(0);

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    float x = i / (float)size * 2 - 1;
                    float y = j / (float)size * 2 - 1;
                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                    result[i + j * size] = Evaluate(value);
                }
            }
        }
        private readonly float Evaluate(float v) {
            return Mathf.Pow(v, a) / (Mathf.Pow(v, a) + Mathf.Pow(b - v * b, a));
        }
    }

}
