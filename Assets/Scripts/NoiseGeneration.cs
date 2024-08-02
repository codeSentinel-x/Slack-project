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
    #region static fields: 
    public static NoiseGeneration _instance;
    public static Action<float[,,], Vector2Int> _onAdvanceNoiseMapGenerationCompleat;
    public static uint _seed = 768754;

    public static void GenerateRandomSeed() {
        _seed = (uint)UnityEngine.Random.Range(uint.MinValue, uint.MaxValue);
    }
    #endregion


    private void Awake() {
        _instance = this;
    }



    public void GenerateNoise(MultipleLayerNoiseSetting mLNS, NoiseLayerSetting temperatureNoise, NoiseLayerSetting humidityNoise, Vector2Int offset = default) {
        if (offset == default) offset = Vector2Int.zero;
        StartCoroutine(GenerateAdvancedNoiseMap(mLNS, temperatureNoise, humidityNoise, offset));
    }



    private IEnumerator GenerateAdvancedNoiseMap(MultipleLayerNoiseSetting mLNS, NoiseLayerSetting temperatureNoise, NoiseLayerSetting humidityNoise, Vector2Int offset = default) {
        float[,,] finalResult = new float[3, mLNS._chunkSize, mLNS._chunkSize];
        NativeList<JobHandle> allJobs = new(Allocator.Temp);
        NativeArray<WeightedNoiseSetting> weightedNoiseSettings = new(mLNS._weightedNoiseSettings.Length, Allocator.TempJob);
        NativeArray<float> noiseResults = new(weightedNoiseSettings.Length * mLNS._chunkSize * mLNS._chunkSize, Allocator.Persistent);
        for (int i = 0; i < mLNS._weightedNoiseSettings.Length; i++) weightedNoiseSettings[i] = mLNS._weightedNoiseSettings[i];
        //generate [0,x,y] - height map
        GenerateNoiseMapLayerJob noiseGenJob = new() {
            result = noiseResults,
            seed = _seed,
            wNS = weightedNoiseSettings,
            offset = new(offset.x, offset.y),
            mapSize = mLNS._chunkSize
        };

        allJobs.Add(noiseGenJob.Schedule(weightedNoiseSettings.Length, 1));



        //generate [1,x,y] - temperature map  and //generate [2,x,y] - humidity map
        NativeArray<float> temperatureResult = new(mLNS._chunkSize * mLNS._chunkSize, Allocator.TempJob);
        NativeArray<float> humidityResult = new(mLNS._chunkSize * mLNS._chunkSize, Allocator.TempJob);
        GenerateNoiseMapJob temperatureGenJob = new() {
            result = temperatureResult,
            seed = _seed,
            nS = temperatureNoise,
            offset = new(offset.x, offset.y),
            mapSize = mLNS._chunkSize
        };
        GenerateNoiseMapJob humidityGenJob = new() {
            result = humidityResult,
            seed = _seed,
            nS = humidityNoise,
            offset = new(offset.x, offset.y),
            mapSize = mLNS._chunkSize
        };


        allJobs.Add(temperatureGenJob.Schedule());
        allJobs.Add(humidityGenJob.Schedule());

        JobHandle.CompleteAll(allJobs);


        for (int x = 0; x < finalResult.GetLength(1); x++) {
            for (int y = 0; y < finalResult.GetLength(2); y++) {
                for (int i = 0; i < weightedNoiseSettings.Length; i++) finalResult[0, x, y] += noiseResults[i * (x + mLNS._chunkSize * y)];
                finalResult[1, x, y] = temperatureResult[x + mLNS._chunkSize * y];
                finalResult[2, x, y] = humidityResult[x + mLNS._chunkSize * y];
            }
        }


        noiseResults.Dispose();
        allJobs.Dispose();
        temperatureResult.Dispose();
        humidityResult.Dispose();

        _onAdvanceNoiseMapGenerationCompleat?.Invoke(finalResult, offset);
        yield return null;
    }

    [BurstCompile]
    public struct GenerateNoiseMapLayerJob : IJobParallelFor {
        public int mapSize;
        public NativeArray<float> result;
        public NativeArray<WeightedNoiseSetting> wNS;
        public uint seed;
        public float2 offset;
        public void Execute(int index) {
            Unity.Mathematics.Random rng = new(seed);
            NativeArray<float2> octaveOffset = new(mapSize * mapSize, Allocator.Temp);
            NativeArray<float> resultContainer = new(mapSize * mapSize, Allocator.Temp);
            float maxPossibleH = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < wNS[index]._noiseSetting._octaves; i++) {
                float x = rng.NextInt(-100000, 100000) + offset.x;
                float y = rng.NextInt(-100000, 100000) + offset.y;

                maxPossibleH += amplitude;
                amplitude *= wNS[index]._noiseSetting._persistance;
                octaveOffset[i] = new float2(x, y);
            }

            float maxNoiseH = float.MinValue;
            float minNoiseH = float.MaxValue;

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    amplitude = 1f;
                    frequency = 1f;
                    float noiseH = 0f;

                    for (int i = 0; i < wNS[index]._noiseSetting._octaves; i++) {

                        float2 sample = new((x - (mapSize / 2) + octaveOffset[i].x) / wNS[index]._noiseSetting._scale * frequency,
                                            (y - (mapSize / 2) + octaveOffset[i].y) / wNS[index]._noiseSetting._scale * frequency);

                        float perlinValue = Mathf.PerlinNoise(sample.x, sample.y) * 2 - 1;
                        noiseH += perlinValue * amplitude;

                        amplitude *= wNS[index]._noiseSetting._persistance;
                        frequency *= wNS[index]._noiseSetting._lacunarity;
                    }

                    if (noiseH > maxNoiseH) maxNoiseH = noiseH;
                    else if (noiseH < minNoiseH) minNoiseH = noiseH;
                    resultContainer[x + y * mapSize] = noiseH;

                    float normalizedHeight = (resultContainer[x + y * mapSize] + 1) / (maxPossibleH / 0.9f);
                    resultContainer[x + y * mapSize] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue) * GetNormalizedWeight(wNS, wNS[index]._weight);
                    result[index * (x + y * mapSize)] += resultContainer[x + y * mapSize];
                }
            }

        }

        private readonly float InverseLerp(float p1, float p2, float v) {
            float factor = (v - p1) / (p2 - p1);
            return factor;
        }
        private readonly float GetNormalizedWeight(NativeArray<WeightedNoiseSetting> wNS, float v) {
            float allWeight = 0f;
            foreach (WeightedNoiseSetting w in wNS) allWeight += w._weight;
            return Mathf.InverseLerp(0, allWeight, v);
        }

    }
    public struct GenerateNoiseMapJob : IJob {
        public int mapSize;
        public NativeArray<float> result;
        public NoiseLayerSetting nS;
        public uint seed;
        public float2 offset;
        public void Execute() {
            Unity.Mathematics.Random rng = new(seed);
            NativeArray<float2> octaveOffset = new(mapSize * mapSize, Allocator.Temp);
            float maxPossibleH = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < nS._octaves; i++) {
                float x = rng.NextInt(-100000, 100000) + offset.x;
                float y = rng.NextInt(-100000, 100000) + offset.y;

                maxPossibleH += amplitude;
                amplitude *= nS._persistance;
                octaveOffset[i] = new float2(x, y);
            }

            float maxNoiseH = float.MinValue;
            float minNoiseH = float.MaxValue;

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    amplitude = 1f;
                    frequency = 1f;
                    float noiseH = 0f;

                    for (int i = 0; i < nS._octaves; i++) {

                        float2 sample = new((x - (mapSize / 2) + octaveOffset[i].x) / nS._scale * frequency,
                                            (y - (mapSize / 2) + octaveOffset[i].y) / nS._scale * frequency);

                        float perlinValue = Mathf.PerlinNoise(sample.x, sample.y) * 2 - 1;
                        noiseH += perlinValue * amplitude;

                        amplitude *= nS._persistance;
                        frequency *= nS._lacunarity;
                    }

                    if (noiseH > maxNoiseH) maxNoiseH = noiseH;
                    else if (noiseH < minNoiseH) minNoiseH = noiseH;
                    result[x + y * mapSize] = noiseH;

                    float normalizedHeight = (result[x + y * mapSize] + 1) / (maxPossibleH / 0.9f);
                    result[x + y * mapSize] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
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