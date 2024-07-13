using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputSliderSync : MonoBehaviour {
    public TMP_InputField _inputField;
    [SerializeField] private Slider _slider;
    [SerializeField] private bool _isUint;

    private void Awake() {
        _slider.onValueChanged.AddListener((x) => {
            if (_slider.wholeNumbers) {
                _inputField.text = x.ToString();
            }
            else {
                _inputField.text = x.ToString("f2");
            }
        });
        _inputField.onDeselect.AddListener((x) => {
            if (_isUint) {
                _slider.value = uint.Parse(x);
            }
            else if (_slider.wholeNumbers) {
                _slider.value = int.Parse(x);
            }
            else {
                _slider.value = float.Parse(x.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }
        });

    }


}
