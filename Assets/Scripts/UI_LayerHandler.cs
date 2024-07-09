using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_LayerHandler : MonoBehaviour {

    public TextMeshProUGUI layerIdText;
    public TMP_InputField scaleText;
    public TMP_InputField octavesText;
    public TMP_InputField persistanceText;
    public TMP_InputField lacunarityText;
    public TMP_InputField weightText;

    public void Setup(int id) {
        layerIdText.text = id.ToString();
    }


    internal void Setup(int id, float scale, int octaves, float persistance, float lacunarity, float weight) {
        layerIdText.text = id.ToString();
        scaleText.text = scale.ToString();
        octavesText.text = octaves.ToString();
        persistanceText.text = persistance.ToString();
        lacunarityText.text = lacunarity.ToString();
        weightText.text = weight.ToString();
        foreach (var t in GetComponentsInChildren<InputScrollUpdate>()) {
            t.inputField.onValueChanged.Invoke(t.inputField.text);
        }
    }
}
