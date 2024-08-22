using MyUtils.Structs;
using TMPro;
using UnityEngine;

public class UI_LayerHandler : MonoBehaviour {

    public TextMeshProUGUI _layerIdText;
    public TMP_InputField _scaleText;
    public TMP_InputField _octavesText;
    public TMP_InputField _persistanceText;
    public TMP_InputField _lacunarityText;
    public TMP_InputField _weightText;

    public void Setup(int id) {
        _layerIdText.text = id.ToString();
    }

    public void Setup(int id, NoiseLayerSetting s, float weight) {
        _layerIdText.text = id.ToString();
        Setup(s, weight);
    }
    public void Setup(NoiseLayerSetting s, float weight) {
        _scaleText.text = s._scale.ToString();
        _octavesText.text = s._octaves.ToString();
        _persistanceText.text = s._persistance.ToString();
        _lacunarityText.text = s._lacunarity.ToString();
        _weightText.text = weight.ToString();
        foreach (var t in GetComponentsInChildren<UI_InputSliderSync>()) {
            t._inputField.onDeselect.Invoke(t._inputField.text);
        }
    }
    public NoiseLayerSetting GetSettings() {
        return new NoiseLayerSetting() {
            _scale = float.Parse(_scaleText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture),
            _octaves = int.Parse(_octavesText.text),
            _persistance = float.Parse(_persistanceText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture),
            _lacunarity = float.Parse(_lacunarityText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture)
        };
    }
    public float GetWeight() {
        return float.Parse(_weightText.text.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
    }
}
