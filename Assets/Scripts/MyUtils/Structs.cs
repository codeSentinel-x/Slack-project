using System;
using UnityEngine;

namespace MyUtils.Structs {

    [Serializable]
    public struct NoiseSetting {
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;


    }
    [Serializable]
    public struct WeightedNoiseSetting {
        public NoiseSetting noiseSetting;
        public float weight;
    }
    [Serializable]
    public struct MultipleLayerNoiseSetting {
        public WeightedNoiseSetting[] weightedNoiseSettings;
        public int chunkSize;
        public int chunkCount;
    }
    [Serializable]
    public struct TerrainType {
        public Gradient gradient;
        public float h;
        public string name;
    }

    [Serializable]
    public struct FalloffSetting {
        public float a;
        public float b;
    }
    [Serializable]
    public struct ChunkItem {
        public string name;
        public float h;
    }

    public struct BiomeType {
        public string name;
        public float temperature;
        public BiomeSO biomeSO;


    }

}
