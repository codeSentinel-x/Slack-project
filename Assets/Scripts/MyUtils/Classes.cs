using System;
using MyUtils.Structs;
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

}