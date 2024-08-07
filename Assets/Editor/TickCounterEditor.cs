using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TickCounter))]
public class TickCounterEditor : Editor {
    public override void OnInspectorGUI() {

        TickCounter ticker = (TickCounter)target;

        if (DrawDefaultInspector()) {
            //Maybe later
        }
        GUIStyle buttonStyle = new(GUI.skin.button) {
            richText = true
        };

        if (GUILayout.Button("<b>\nRefresh\n</b>", buttonStyle)) {
            ticker.Refresh();
        }

    }
}
