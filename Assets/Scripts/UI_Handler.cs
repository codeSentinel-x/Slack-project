using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MyUtils.Structs;
using TMPro;
using UnityEngine;

public class UI_Handler : MonoBehaviour {

    public TMP_InputField mapSizeText;
    public TMP_InputField scaleText;
    public TMP_InputField octavesText;
    public TMP_InputField persistanceText;
    public TMP_InputField lacunarityText;
    public TMP_InputField seedText;
    public TMP_InputField chunkMapSizeText;
    private WorldGeneration _worldGenerator;

    void Start() {
        _worldGenerator = WorldGeneration._instance;
    }


    public void GenerateWorld() {
        NoiseSetting setting = new() {
            mapSize = int.Parse(mapSizeText.text),
            scale = float.Parse(scaleText.text),
            octaves = int.Parse(octavesText.text),
            persistance = float.Parse(persistanceText.text),
            lacunarity = float.Parse(lacunarityText.text)
        };
        uint seed = uint.Parse(seedText.text);
        int chunkSize = int.Parse(chunkMapSizeText.text);
    }
    public void DestroyWorld() {
        _worldGenerator.DestroyWorld();
    }
}
