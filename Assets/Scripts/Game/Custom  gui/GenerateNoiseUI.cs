using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class GenerateNoiseUI : MonoBehaviour {

    public Rect _customWindowRect = new(Screen.width / 2, Screen.height / 2, 600, 600);
    public GUIStyle _customWindowStyle;
    public GUIStyle _labelStyle;
    public GUIStyle _boxStyle;
    public GUIStyle _sliderStyle;
    private void InitializeStyles() {
        _labelStyle = new() {
            normal = { textColor = Color.white },
            fontSize = 30,
            alignment = TextAnchor.MiddleCenter,
        };
        _boxStyle = GUI.skin.box;
        _boxStyle.fontSize = 30;
        _boxStyle.alignment = TextAnchor.MiddleLeft;
    }

    void OnGUI() {
        InitializeStyles();
        GeneratePreview();
    }

    private void GeneratePreview() {
        // GUI.skin = null;

        _customWindowRect = GUI.Window(0, _customWindowRect, CustomWindowFunction, "My Noise Generation", _customWindowStyle);
    }
    private float _sliderValue;
    private void CustomWindowFunction(int id) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
        BuildSettingBox(100);
        // GUI.Label(new Rect(10, 200, _customWindowRect.width - 20, 40), "Test", _labelStyle);
    }

    private void BuildSettingBox(float y) {
        float w = _customWindowRect.width;
        Rect groupRect = new(10, y, w - 20, 200);
        GUI.BeginGroup(groupRect);
        GUI.Box(new Rect(0, 0, groupRect.width, groupRect.height), "Test1", _boxStyle);
        _sliderValue = GUI.HorizontalSlider(new Rect(groupRect.width / 2 + 10, groupRect.height / 2, 200, groupRect.height), _sliderValue, 0, 10, _sliderStyle, GUI.skin.horizontalSliderThumb);
        GUI.Label(new Rect(groupRect.width - 40, 0, groupRect.width, groupRect.height), _sliderValue.ToString(), _labelStyle);
        GUI.EndGroup();
    }
}
