using System;
using MyUtils.Structs;
using UnityEngine;

namespace MyUtils.Classes {

    [Serializable]
    public class NoiseSettingData {
        public MultipleLayerNoiseSetting _settings;
        public uint _seed;
    }


    [Serializable]
    public class PathFindingCellItem {
        public int _x;
        public int _y;
        public Vector2Int _worldPos;
        public ChunkItem _cell;
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
        public BiomeSO DesertSO;
        public BiomeSO JungleSO;
        public BiomeSO SavannaSO;
        public BiomeSO TundraSO;
        public BiomeSO TaigaSO;
        public BiomeSO GrasslandSO;
        public BiomeSO SwampSO;
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