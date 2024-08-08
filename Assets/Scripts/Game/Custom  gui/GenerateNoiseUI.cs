using UnityEngine;

public class GenerateNoiseUI : MonoBehaviour {

    public Rect _customWindowRect = new(Screen.width / 2, Screen.height / 2, 600, 600);
    public GUIStyle _customWindowStyle;
    void OnEnable() {
        InitializeStyles();
    }

    private void InitializeStyles() {
        // _customWindowStyle = skin.window;
    }

    void OnGUI() {
        GeneratePreview();
    }

    private void GeneratePreview() {
        GUI.skin = null;
        _customWindowStyle = GUI.skin.window;
        _customWindowRect = GUI.Window(0, _customWindowRect, CustomWindowFunction, "My Noise Generation");
    }

    private void CustomWindowFunction(int id) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
