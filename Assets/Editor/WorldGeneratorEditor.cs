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
        
        if (GUILayout.Button("\nGenerate world\n")) {
            worldGen.GenerateAdvancedChunks();
        }
        if (GUILayout.Button("\nGenerateEnviVisual\n")) {
            worldGen.GenerateEnviVisual();
            worldGen.ChangeWorldVisibility();
        }

        if (GUILayout.Button("\nChangeVisual\n")) {

            worldGen.ChangeWorldVisibility();
        }

        if (GUILayout.Button("\nTest delay\n")) {
            worldGen.TestSpeed();
        }
    }
}
