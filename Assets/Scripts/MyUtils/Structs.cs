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
        public bool isWalkable;
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
        public bool isWalkable;
    }

    public struct BiomeType {
        public string name;
        public float temperature;
        public BiomeSO biomeSO;


    }
    //TODO move this to Classes.cs 
    public class PathFindingCellItem {
        public int _x;
        public int _y;
        public ChunkItem _cell;
        public PathFindingCellItem _previous;
        public int _gCost;
        public int _hCost;
        public int _fCost;
        public void CalculateFCost() {
            _fCost = _hCost + _gCost;
        }
    }

}
