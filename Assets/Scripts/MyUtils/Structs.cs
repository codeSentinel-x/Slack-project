using System;
using UnityEngine;

namespace MyUtils.Structs {

    [Serializable]
    public struct NoiseSetting {
        public int mapSize;
        public float scale;
        public int octaves;
        public float persistance;
        public uint seed;
        public Vector2 offset;
        public float lacunarity;
    }
    [Serializable]
    public struct FalloffSetting {
        public float a;
        public float b;
    }



}
