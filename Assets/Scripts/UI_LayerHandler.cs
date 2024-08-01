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


    public void Setup(int id, float scale, int octaves, float persistance, float lacunarity, float weight) {
        _layerIdText.text = id.ToString();
        _scaleText.text = scale.ToString();
        _octavesText.text = octaves.ToString();
        _persistanceText.text = persistance.ToString();
        _lacunarityText.text = lacunarity.ToString();
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
}
