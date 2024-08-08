using UnityEngine;

public class GenerateNoiseUI : MonoBehaviour {

    public Rect _customWindowRect = new(Screen.width / 2, Screen.height / 2, 600, 600);
    public GUIStyle _customWindowStyle;
    public GUIStyle _labelStyle;
    public GUIStyle _boxStyle;
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

    private void CustomWindowFunction(int id) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
        GUI.Box(new Rect(10, 100, _customWindowRect.width - 20, 40), "Test1", _boxStyle);
        GUI.HorizontalSlider(new Rect(50, 100, _customWindowRect.width - 100, 40), 3, 0, 10);
        GUI.Label(new Rect(10, 200, _customWindowRect.width - 20, 40), "Test", _labelStyle);
    }

}
