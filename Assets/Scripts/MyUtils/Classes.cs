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

}