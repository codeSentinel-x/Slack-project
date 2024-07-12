using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputSliderSync : MonoBehaviour {
    public TMP_InputField inputField;
    [SerializeField] private Slider slider;
    [SerializeField] private bool isUint;

    private void Awake() {
        slider.onValueChanged.AddListener((x) => {
            if (slider.wholeNumbers) {
                inputField.text = x.ToString();
            }
            else {
                inputField.text = x.ToString("f2");
            }
        });
        inputField.onDeselect.AddListener((x) => {
            if (isUint) {
                slider.value = uint.Parse(x);
            }
            else if (slider.wholeNumbers) {
                slider.value = int.Parse(x);
            }
            else {
                slider.value = float.Parse(x.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }
        });

    }


}
