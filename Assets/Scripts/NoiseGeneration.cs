using System.Collections;
using UnityEngine;
using MyUtils.Structs;
using Unity.Jobs;
using Unity.Collections;
using System;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.ComponentModel;

public class NoiseGeneration : MonoBehaviour {
    public static NoiseGeneration _instance;
    public static uint seed = 768754;
    public Action<float[,], Vector2Int> _onNoiseGenerationCompleat;

    void Awake() {
        _instance = this;
    }
    public static void RefreshSeed() {
        seed = (uint)UnityEngine.Random.Range(1, 1000000);
    }

    public void GenerateNoise(NoiseSetting nS, AnimationCurve aC, Vector2Int offset = default) {
        if (offset == default) offset = Vector2Int.zero;
        StartCoroutine(GenerateNoiseCoroutine(nS, aC, offset)); //Without falloff
    }
    public void GenerateNoise(NoiseSetting nS, FalloffSetting fS, AnimationCurve aC, Vector2Int offset = default) {
        if (offset == default) offset = Vector2Int.zero;
        StartCoroutine(GenerateNoiseCoroutine(nS, fS, aC, offset)); //With falloff
    }
    public void GenerateNoise(WeightedNoiseSetting[] ws, Vector2Int offset = default) {
        if (offset == default) offset = Vector2Int.zero;
        StartCoroutine(GenerateMultipleNoise(ws, offset)); //With falloff
    }

    private IEnumerator GenerateNoiseCoroutine(NoiseSetting nS, AnimationCurve heightCurve, Vector2Int offset) {
        NativeArray<float> _noiseMapResult = new(nS.mapSize * nS.mapSize, Allocator.TempJob);

        GenerateEntireNoiseMapJob noiseGenJob = new() {
            result = _noiseMapResult,
            seed = seed,
            nS = nS,
            offset = new(offset.x, offset.y),
        };

        JobHandle _noiseJobH = noiseGenJob.Schedule();

        _noiseJobH.Complete();

        while (!_noiseJobH.IsCompleted) {
            yield return new WaitForSeconds(0.5f);
        }
        float[,] generatedResult = new float[nS.mapSize, nS.mapSize];
        for (int x = 0; x < generatedResult.GetLength(0); x++) {
            for (int y = 0; y < generatedResult.GetLength(1); y++) {
                generatedResult[x, y] = heightCurve.Evaluate(_noiseMapResult[x + nS.mapSize * y]);
            }
        }

        _noiseMapResult.Dispose();
        _onNoiseGenerationCompleat?.Invoke(generatedResult, offset);
    }
    private IEnumerator GenerateNoiseCoroutine(NoiseSetting nS, FalloffSetting fS, AnimationCurve heightCurve, Vector2Int offset) {
        int mapSize = nS.mapSize;

        NativeArray<float> _noiseMapResult = new(nS.mapSize * nS.mapSize, Allocator.TempJob);
        NativeArray<float> _falloffMapResult = new(nS.mapSize * nS.mapSize, Allocator.TempJob);
        GenerateEntireNoiseMapJob noiseGenJob = new() {
            result = _noiseMapResult,
            seed = seed,
            nS = nS,
            offset = new(offset.x, offset.y),
        };
        GenerateFalloffMapJob falloffGenJob = new() {
            result = _falloffMapResult,
            size = mapSize,
            a = fS.a,
            b = fS.b,
        };
        JobHandle _noiseJobH = noiseGenJob.Schedule();
        JobHandle _falloffJobH = falloffGenJob.Schedule();

        _noiseJobH.Complete();
        _falloffJobH.Complete();

        while (!_noiseJobH.IsCompleted || !_falloffJobH.IsCompleted) {
            yield return new WaitForSeconds(0.5f);
        }
        float[,] generatedResult = new float[mapSize, mapSize];
        for (int x = 0; x < generatedResult.GetLength(0); x++) {
            for (int y = 0; y < generatedResult.GetLength(1); y++) {
                float h = heightCurve.Evaluate(_noiseMapResult[x + mapSize * y]);
                generatedResult[x, y] = Mathf.Clamp01(h - _falloffMapResult[x + mapSize * y]);
            }
        }

        _noiseMapResult.Dispose();
        _falloffMapResult.Dispose();
        _onNoiseGenerationCompleat?.Invoke(generatedResult, offset);
    }

    private IEnumerator GenerateMultipleNoise(WeightedNoiseSetting[] weightedNoiseSettings, Vector2Int offset) {
        float[,] finalResult = new float[weightedNoiseSettings[0].noiseSetting.mapSize, weightedNoiseSettings[0].noiseSetting.mapSize];
        foreach (WeightedNoiseSetting w in weightedNoiseSettings) {

            NativeArray<float> _noiseMapResult = new(w.noiseSetting.mapSize * w.noiseSetting.mapSize, Allocator.TempJob);

            GenerateEntireNoiseMapJob noiseGenJob = new() {
                result = _noiseMapResult,
                seed = seed,
                nS = w.noiseSetting,
                offset = new(offset.x, offset.y),
            };

            JobHandle _noiseJobH = noiseGenJob.Schedule();

            _noiseJobH.Complete();

            while (!_noiseJobH.IsCompleted) {
                yield return new WaitForSeconds(0.5f);
            }
            for (int x = 0; x < finalResult.GetLength(0); x++) {
                for (int y = 0; y < finalResult.GetLength(1); y++) {
                    finalResult[x, y] += _noiseMapResult[x + w.noiseSetting.mapSize * y] * w.weight;
                }
            }

            _noiseMapResult.Dispose();
            _onNoiseGenerationCompleat?.Invoke(finalResult, offset);
        }
    }
    [BurstCompile]
    public struct GenerateEntireNoiseMapJob : IJob {

        public NativeArray<float> result;
        public NoiseSetting nS;
        public uint seed;
        public float2 offset;
        public void Execute() {
            Unity.Mathematics.Random rng = new(seed);
            NativeArray<float2> octaveOffset = new(nS.mapSize * nS.mapSize, Allocator.Temp);

            float maxPossibleH = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < nS.octaves; i++) {
                float x = rng.NextInt(-100000, 100000) + offset.x;
                float y = rng.NextInt(-100000, 100000) + offset.y;

                maxPossibleH += amplitude;
                amplitude *= nS.persistance;
                octaveOffset[i] = new float2(x, y);
            }

            float maxNoiseH = float.MinValue;
            float minNoiseH = float.MaxValue;

            for (int y = 0; y < nS.mapSize; y++) {
                for (int x = 0; x < nS.mapSize; x++) {
                    amplitude = 1f;
                    frequency = 1f;
                    float noiseH = 0f;

                    for (int i = 0; i < nS.octaves; i++) {

                        float2 sample = new((x - (nS.mapSize / 2) + octaveOffset[i].x) / nS.scale * frequency,
                                            (y - (nS.mapSize / 2) + octaveOffset[i].y) / nS.scale * frequency);

                        float perlinValue = Mathf.PerlinNoise(sample.x, sample.y) * 2 - 1;
                        noiseH += perlinValue * amplitude;

                        amplitude *= nS.persistance;
                        frequency *= nS.lacunarity;
                    }

                    if (noiseH > maxNoiseH) maxNoiseH = noiseH;
                    else if (noiseH < minNoiseH) minNoiseH = noiseH;

                    result[x + y * nS.mapSize] = noiseH;

                    float normalizedHeight = (result[x + y * nS.mapSize] + 1) / (maxPossibleH / 0.9f);
                    result[x + y * nS.mapSize] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
            // for (int y = 0; y < nS.mapSize; y++) {
            //     for (int x = 0; x < nS.mapSize; x++) {
            //         result[x + y * nS.mapSize] = InverseLerp(minNoiseH, maxNoiseH, result[x + y * nS.mapSize]);
            //     }
            // }
        }
        private readonly float InverseLerp(float p1, float p2, float v) {
            float factor = (v - p1) / (p2 - p1);
            return factor;
        }


    }





    [BurstCompile]
    public struct GenerateFalloffMapJob : IJob {
        public NativeArray<float> result;
        public int size;
        public float a;
        public float b;
        public void Execute() {

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