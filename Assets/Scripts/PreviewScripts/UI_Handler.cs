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

    public void GenerateAdvanceWorldMultipleLayer() {
        NoiseSettingData nD = GetNoiseData();
        _worldGenerator.GenerateAdvancedChunks(nD);
    }
    public NoiseSettingData GetNoiseData() {
        NoiseSettingData data = new() {
            _settings = GetMultipleNoiseSettingArray(),
            _temperatureNoise = _tempNoiseLayer.GetSettings(),
            _humidityNoise = _humidityNoiseLayer.GetSettings(),
            _seed = uint.Parse(_seedText.text),
        };
        return data;
    }
    public MultipleLayerNoiseSetting GetMultipleNoiseSettingArray() {
        MultipleLayerNoiseSetting mLNS = new() {
            _weightedNoiseSettings = new WeightedNoiseSetting[_layers.Count],
            _chunkSize = int.Parse(_chunkSizeText.text),
            _chunkRenderDistance = int.Parse(_chunkCountText.text),
        };
        for (int i = 0; i < mLNS._weightedNoiseSettings.Length; i++) {
            mLNS._weightedNoiseSettings[i]._noiseSetting = _layers[i].GetSettings();
            mLNS._weightedNoiseSettings[i]._weight = _layers[i].GetWeight();
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
        _chunkCountText.text = loadedData._settings._chunkRenderDistance.ToString();
        _chunkCountText.onDeselect.Invoke(_chunkCountText.text);
        _chunkSizeText.text = loadedData._settings._chunkSize.ToString();
        _chunkSizeText.onDeselect.Invoke(_chunkSizeText.text);

        _layers.ForEach((x) => Destroy(x.gameObject));
        _layers.Clear();
        _layers = new();
        _tempNoiseLayer.Setup(loadedData._temperatureNoise, 1);
        _humidityNoiseLayer.Setup(loadedData._humidityNoise, 1);
        foreach (var w in loadedData._settings._weightedNoiseSettings) {
            CreateNewLayer(w);
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
    public void CreateNewLayer(WeightedNoiseSetting wSettings) {
        UI_LayerHandler l = Instantiate(_layerPrefab, _layerHandler).GetComponent<UI_LayerHandler>();
        _layers.Add(l);
        l.gameObject.SetActive(true);
        _layers[_index].gameObject.SetActive(false);
        l.Setup(_layers.Count - 1, wSettings._noiseSetting, wSettings._weight);
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
