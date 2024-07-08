using System;
using UnityEngine;

namespace MyUtils.Structs {

    [Serializable]
    public struct NoiseSetting {
        public int mapSize;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;


    }

    [Serializable]
    public struct FalloffSetting {
        public float a;
        public float b;
    }




}
