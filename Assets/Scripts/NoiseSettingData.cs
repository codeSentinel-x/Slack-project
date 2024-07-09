using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using UnityEngine;

[Serializable]
public class NoiseSettingData {
    public WeightedNoiseSetting[] settings;
    public uint seed;
    public int chunkSize;
    public int amount;
}
