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
    public enum TerrainRuleAssetType {
        SingleColor,
        Gradient,
        SingleTexture,
        TextureGradient,
        RandomTexture,
    }
    [Serializable]
    public struct TerrainRule {
        public Gradient _gradient;
        public float _maxHeight;
        public string _cellName;
        public bool _isWalkable;
    }

    [Serializable]
    public struct ChunkItem<T> {
        public string _terrainTypeName;
        public float _cellH;
        public bool _isWalkable;
        public T content;
    }

    [Serializable]
    public struct BiomeType {
        public string _biomeName;
        public float _minTemperature;
        public BiomeSO _baseBiomeSO;
    }

    public enum Biomes {
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
