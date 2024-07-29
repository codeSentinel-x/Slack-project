using System;
using UnityEngine;

namespace MyUtils.Structs {

    [Serializable]
    public struct NoiseLayerSetting {
        public float _scale;
        public int _octaves;
        public float _persistance;
        public float _lacunarity;


    }
    [Serializable]
    public struct WeightedNoiseSetting {
        public NoiseLayerSetting _noiseSetting;
        public float _weight;
    }
    [Serializable]
    public struct MultipleLayerNoiseSetting {
        public WeightedNoiseSetting[] _weightedNoiseSettings;
        public int _chunkSize;
        public int _chunkCount;
    }
    [Serializable]
    public struct TerrainType {
        public Gradient _gradient;
        public float _h;
        public string _cellName;
        public bool _isWalkable;
    }

    [Serializable]
    public struct FalloffSetting {
        public float _multiplierA;
        public float _multiplierB;
    }
    [Serializable]
    public struct ChunkItem {
        public string _terrainTypeName;
        public float _cellH;
        public bool _isWalkable;
    }

    [Serializable]
    public struct BiomeType {
        public string _biomeName;
        public float _minTemperature;
        public BiomeSO _baseBiomeSO;
    }
    
    public enum Biomes{
        Desert,
        Jungle,
        Savanna,
        Tundra,
        Taiga,
        Grassland,
        Swamp,
        Forest
    }


}
