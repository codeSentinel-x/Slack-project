using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGeneration))]
public class WorldGeneratorEditor : Editor {
    public override void OnInspectorGUI() {

        WorldGeneration worldGen = (WorldGeneration)target;

        if (DrawDefaultInspector()) {
            //Maybe later
        }

        GUIStyle buttonStyle = new(GUI.skin.button) {
            richText = true
        };

        if (GUILayout.Button("<b>\nGenerate world\n</b>", buttonStyle)) {
            worldGen.GenerateAdvancedChunks();
        }
        if (GUILayout.Button("<b>\nGenerateEnviVisual\n</b>", buttonStyle)) {
            worldGen.GenerateEnviVisual();
            worldGen.ChangeWorldVisibility();
        }

        if (GUILayout.Button("<b>\nChangeVisual\n</b>", buttonStyle)) {

            worldGen.ChangeWorldVisibility();
        }

        if (GUILayout.Button("<b>\nTest delay\n</b>", buttonStyle)) {
            worldGen.TestSpeed();
        }
    }
}
