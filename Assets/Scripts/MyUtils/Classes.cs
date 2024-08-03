using System;
using MyUtils.Structs;
using UnityEngine;

namespace MyUtils.Classes {

    [Serializable]
    public class NoiseSettingData {
        [Tooltip("Settings for multiple layers of noise.")]
        public MultipleLayerNoiseSetting _settings;

        [Tooltip("")]
        public NoiseLayerSetting _temperatureNoise;

        [Tooltip("")]
        public NoiseLayerSetting _humidityNoise;

        [Tooltip("")]
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
    public class BiomeAssets {
        
        [Tooltip("")] 
        public BiomeSO DesertSO;
        
        [Tooltip("")] 
        public BiomeSO JungleSO;
        
        [Tooltip("")] 
        public BiomeSO SavannaSO;
        
        [Tooltip("")] 
        public BiomeSO TundraSO;
        
        [Tooltip("")] 
        public BiomeSO TaigaSO;
        
        [Tooltip("")] 
        public BiomeSO GrasslandSO;
        
        [Tooltip("")] 
        public BiomeSO SwampSO;
        
        [Tooltip("")] 
        public BiomeSO ForestSO;
        
        public BiomeSO GetBiomeSO(float temp, float humidity) {
            if (temp > 0.7f) {
                if (humidity < 0.3f) return DesertSO;
                else if (humidity > 0.7f) return JungleSO;
                else return SavannaSO;
            }
            else if (temp < 0.3f) {
                if (humidity < 0.3f) return TundraSO;
                else return TaigaSO;
            }
            else {
                if (humidity < 0.3f) return GrasslandSO;
                else if (humidity > 0.7) return SwampSO;
                else return ForestSO;
            }
        }
    }
}