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
}
