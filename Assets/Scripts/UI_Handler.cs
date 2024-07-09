using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MyUtils.Structs;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;

public class UI_Handler : MonoBehaviour {

    public TMP_InputField mapSizeText;
    public TMP_InputField scaleText;
    public TMP_InputField octavesText;
    public TMP_InputField persistanceText;
    public TMP_InputField lacunarityText;
    public TMP_InputField seedText;
    public TMP_InputField chunkMapSizeText;
    public RectTransform layerHandler;
    public GameObject layerPrefab;
    public bool useMultipleLayer;
    private WorldGeneration _worldGenerator;
    private int index = 0;
    private List<UI_LayerHandler> layers = new();
    void Start() {
        _worldGenerator = WorldGeneration._instance;
        if (useMultipleLayer) CreateNewLayer();
        NextLayer();
    }



    public void GenerateWorld() {
        NoiseSetting setting = new() {
            mapSize = int.Parse(mapSizeText.text),
            scale = float.Parse(scaleText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture),
            octaves = int.Parse(octavesText.text),
            persistance = float.Parse(persistanceText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture),
            lacunarity = float.Parse(lacunarityText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture)
        };
        uint seed = uint.Parse(seedText.text);
        int chunkSize = int.Parse(chunkMapSizeText.text);
        _worldGenerator.GenerateChunks(setting, seed, chunkSize);
    }
    public void GenerateWorldMultipleLayer() {
        NoiseSettingData nD = GetNoiseData();
        _worldGenerator.GenerateChunks(nD.settings, nD.seed, nD.chunkSize);
    }
    public NoiseSettingData GetNoiseData() {
        NoiseSettingData data = new() {
            settings = GetWeightedNoiseArray(),
            seed = uint.Parse(seedText.text),
            chunkSize = int.Parse(chunkMapSizeText.text),
        };
        return data;
    }
    public WeightedNoiseSetting[] GetWeightedNoiseArray() {
        WeightedNoiseSetting[] wS = new WeightedNoiseSetting[layers.Count];
        for (int i = 0; i < wS.Length; i++) {
            wS[i].noiseSetting = new NoiseSetting() {
                mapSize = int.Parse(mapSizeText.text),
                scale = float.Parse(layers[i].scaleText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture),
                octaves = int.Parse(layers[i].octavesText.text),
                persistance = float.Parse(layers[i].persistanceText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture),
                lacunarity = float.Parse(layers[i].lacunarityText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture)
            };
            wS[i].weight = float.Parse(layers[i].weightText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }
        return wS;
    }

    public void SaveCurrentSetting() {
        NoiseSettingData data = GetNoiseData();
        string jsonData = JsonUtility.ToJson(data);
        
    }
    public void LoadSetting() {

    }
    public void CreateNewLayer() {
        UI_LayerHandler l = Instantiate(layerPrefab, layerHandler).GetComponent<UI_LayerHandler>();
        layers.Add(l);
        l.gameObject.SetActive(false);
        l.Setup(layers.Count - 1);
    }
    public void NextLayer() {
        layers[index].gameObject.SetActive(false);
        index++;
        if (index >= layers.Count) index = 0;
        layers[index].gameObject.SetActive(true);
    }
    public void PreviousLayer() {
        layers[index].gameObject.SetActive(false);
        index--;
        if (index < 0) index = layers.Count - 1;
        layers[index].gameObject.SetActive(true);
    }
    public void DestroyWorld() {
        _worldGenerator.DestroyWorld();
    }

}
