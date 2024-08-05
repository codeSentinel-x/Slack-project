using System;
using MyUtils.Structs;
using Unity.Mathematics;
using UnityEngine;

namespace MyUtils.Classes {

    [Serializable]
    public class NoiseSettingData {
        [Tooltip("Settings for multiple layers of noise.")]
        public MultipleLayerNoiseSetting _settings;

        [Tooltip("Noise setting for temperature noise.")]
        public NoiseLayerSetting _temperatureNoise;

        [Tooltip("Noise setting for humidity noise.")]
        public NoiseLayerSetting _humidityNoise;

        [Tooltip("Seed value for all noises.")]
        public uint _seed;
    }


    [Serializable]
    public class PathFindingCellItem {
        public int _x;
        public int _y;
        public Vector2Int _worldPos;
        public ChunkItem<int> _cell;
        public PathFindingCellItem _previous;
        public int _gCost;
        public int _hCost;
        public int _fCost;
        public void CalculateFCost() {
            _fCost = _hCost + _gCost;
        }
    }
    [Serializable]
    public class Rule {
        public Sprite[] spriteVariants;
        public BiomeSO biome;
        public float minWorldHeight;
        public float maxWorldHeight;
        [Range(0, 1)]
        [Tooltip("Probability of spawning, between 0 (never) and 1 (always).")]
        public float probability;
        public bool CanBeSpawned(){
            return UnityEngine.Random.Range(0f, 1f) <= probability;
        }
    }

}