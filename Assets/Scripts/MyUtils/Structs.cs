using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace MyUtils.Structs {
    
    [Serializable]
    public struct NoiseSetting{
        public int mapSize;
        public float noiseScale;
        public int octaves;
        public float persistance;
        public uint seed;
        public Vector2 offset;
        public bool applyFalloff;
        public float lacunarity;
    }
    [Serializable]
    public struct FallofSetting{
        public float a;
        
    }
    
    
    
}
