using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputScrollUpdate : MonoBehaviour {
    public TMP_InputField inputField;
    public Slider slider;
    public bool isUint;

    void Awake() {
        slider.onValueChanged.AddListener((x) => {
            if (slider.wholeNumbers) {
                inputField.text = x.ToString();
            }
            else {
                inputField.text = x.ToString("f2");
            }
        });
        inputField.onValueChanged.AddListener((x) => {


            if (isUint) {
                slider.value = uint.Parse(x);
            }
            else if (slider.wholeNumbers) {
                slider.value = int.Parse(x);
            }
            else {
                slider.value = float.Parse(x.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }

            // slider.onValueChanged.Invoke(slider.value);

        });

    }


}
