using System.Collections.Generic;
using MyUtils.Classes;
using MyUtils.Structs;
using TMPro;
using UnityEngine;

public class UI_Handler : MonoBehaviour {

    public static UI_Handler _instance;
    [SerializeField] private TMP_InputField _chunkSizeText;
    [SerializeField] private TMP_InputField _seedText;
    [SerializeField] private TMP_InputField _chunkCountText;
    [SerializeField] private RectTransform _layerHandler;
    [SerializeField] private GameObject _layerPrefab;
    [SerializeField] private UI_LayerHandler _tempNoiseLayer;
    [SerializeField] private UI_LayerHandler _humidityNoiseLayer;
    private WorldGeneration _worldGenerator;
    private int _index = 0;
    private List<UI_LayerHandler> _layers = new();

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        _worldGenerator = WorldGeneration._instance;
        CreateNewLayer();
        NextLayer();
    }


    public void GenerateWorldMultipleLayer() {
        NoiseSettingData nD = GetNoiseData();
        _worldGenerator.GenerateChunks(nD._settings, nD._seed);
    }
    public void GenerateAdvanceWorldMultipleLayer() {
        NoiseSettingData nD = GetNoiseData();
        _worldGenerator.GenerateAdvancedChunks(nD._settings, nD._seed);
    }
    public NoiseSettingData GetNoiseData() {
        NoiseSettingData data = new() {
            _settings = GetMultipleNoiseSettingArray(),
            _temperatureNoise =
            _seed = uint.Parse(_seedText.text),
        };
        return data;
    }
    public MultipleLayerNoiseSetting GetMultipleNoiseSettingArray() {
        MultipleLayerNoiseSetting mLNS = new() {
            _weightedNoiseSettings = new WeightedNoiseSetting[_layers.Count],
            _chunkSize = int.Parse(_chunkSizeText.text),
            _chunkCount = int.Parse(_chunkCountText.text),
        };
        for (int i = 0; i < mLNS._weightedNoiseSettings.Length; i++) {
            mLNS._weightedNoiseSettings[i]._noiseSetting = _layers[i].GetSettings();
            mLNS._weightedNoiseSettings[i]._weight = float.Parse(_layers[i]._weightText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }
        return mLNS;
    }

    public void SaveCurrentSetting(string name) {
        SaveSystem.Save<NoiseSettingData>(SaveSystem.NOISE_SETTING_DEFAULT_SAVE_PATH, name, GetNoiseData());
    }
    public void LoadSetting(string name) {

        NoiseSettingData loadedData = SaveSystem.Load<NoiseSettingData>(SaveSystem.NOISE_SETTING_DEFAULT_SAVE_PATH, name);

        _seedText.text = loadedData._seed.ToString();
        _seedText.onDeselect.Invoke(_seedText.text);
        _chunkCountText.text = loadedData._settings._chunkCount.ToString();
        _chunkCountText.onDeselect.Invoke(_chunkCountText.text);
        _chunkSizeText.text = loadedData._settings._chunkSize.ToString();
        _chunkSizeText.onDeselect.Invoke(_chunkSizeText.text);

        _layers.ForEach((x) => Destroy(x.gameObject));
        _layers.Clear();
        _layers = new();

        foreach (var w in loadedData._settings._weightedNoiseSettings) {
            CreateNewLayer(w._noiseSetting._scale, w._noiseSetting._octaves, w._noiseSetting._persistance, w._noiseSetting._lacunarity, w._weight);
        }

        _index = 0;
        _layers[0].gameObject.SetActive(true);

    }
    public void CreateNewLayer() {
        UI_LayerHandler l = Instantiate(_layerPrefab, _layerHandler).GetComponent<UI_LayerHandler>();
        _layers.Add(l);
        l.gameObject.SetActive(false);
        l.Setup(_layers.Count - 1);
    }
    public void CreateNewLayer(float scale, int octaves, float persistance, float lacunarity, float weight) {
        UI_LayerHandler l = Instantiate(_layerPrefab, _layerHandler).GetComponent<UI_LayerHandler>();
        _layers.Add(l);
        l.gameObject.SetActive(true);
        _layers[_index].gameObject.SetActive(false);
        l.Setup(_layers.Count - 1, scale, octaves, persistance, lacunarity, weight);
        _index = _layers.Count - 1;
        _layers[_index].gameObject.SetActive(false);

    }
    public void NextLayer() {
        _layers[_index].gameObject.SetActive(false);
        _index++;
        if (_index >= _layers.Count) _index = 0;
        _layers[_index].gameObject.SetActive(true);
    }
    public void PreviousLayer() {
        _layers[_index].gameObject.SetActive(false);
        _index--;
        if (_index < 0) _index = _layers.Count - 1;
        _layers[_index].gameObject.SetActive(true);
    }
    public void DestroyWorld() {
        _worldGenerator.DestroyWorld();
    }

}
