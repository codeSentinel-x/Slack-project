using System.Collections;
using UnityEngine;
using MyUtils.Structs;
using Unity.Jobs;
using Unity.Collections;
using System;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;

public class NoiseGeneration : MonoBehaviour {
    public static NoiseGeneration _instance;
    public static uint seed = 768754;
    public Action<float[,]> _onNoiseGenerationCompleat;

    void Awake() {
        _instance = this;
    }
    public static void RefreshSeed() {
        seed = (uint)UnityEngine.Random.Range(1, 1000000);
    }
    //This two methods start coroutines that will create noise map   
    public void GenerateNoise(NoiseSetting nS, AnimationCurve aC) {
        StartCoroutine(GenerateNoiseCoroutine(nS, aC)); //Without falloff
    }
    public void GenerateNoise(NoiseSetting nS, FalloffSetting fS, AnimationCurve aC) {
        StartCoroutine(GenerateNoiseCoroutine(nS, fS, aC)); //With falloff
    }
    private IEnumerator GenerateNoiseCoroutine(NoiseSetting nS, AnimationCurve heightCurve) {
        NativeList<float> _noiseMapResult = new(Allocator.TempJob);

        GenerateNoiseMapJob noiseGenJob = new() {
            result = _noiseMapResult,
            seed = seed,
            mapSize = nS.mapSize,
            lacunarity = nS.lacunarity,
            scale = nS.scale,
            octaves = nS.octaves,
            persistance = nS.persistance,
            offset = new(nS.offset.x, nS.offset.y),
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
        _onNoiseGenerationCompleat?.Invoke(generatedResult);
    }
    private IEnumerator GenerateNoiseCoroutine(NoiseSetting nS, FalloffSetting fS, AnimationCurve heightCurve) {
        int mapSize = nS.mapSize;

        NativeList<float> _noiseMapResult = new(Allocator.TempJob);
        NativeList<float> _falloffMapResult = new(Allocator.TempJob);
        GenerateNoiseMapJob noiseGenJob = new() {
            result = _noiseMapResult,
            seed = seed,
            mapSize = mapSize,
            lacunarity = nS.lacunarity,
            scale = nS.scale,
            octaves = nS.octaves,
            persistance = nS.persistance,
            offset = new(nS.offset.x, nS.offset.y),
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
        _onNoiseGenerationCompleat?.Invoke(generatedResult);
    }

    private NativeArray<Keyframe> CurveToNativeArray(AnimationCurve curve) {
        Keyframe[] keyframes = curve.keys;
        NativeArray<Keyframe> nativeArray = new(keyframes.Length, Allocator.TempJob);
        for (int i = 0; i < keyframes.Length; i++) {
            nativeArray[i] = keyframes[i];
        }
        return nativeArray;
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
        private readonly float InverseLerp(float p1, float p2, float v) {
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
