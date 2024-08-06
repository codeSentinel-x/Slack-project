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
        if (GUILayout.Button("Generate world")) {
            worldGen.GenerateAdvancedChunks();
            // Debug.Log("Nuh uh");
        }
        if (GUILayout.Button("Test delay")) {
            worldGen.TestSpeed();
        }
        if (GUILayout.Button("GenerateEnviNoiseVisual")) {
            worldGen.GenerateEnviVisual();
        }
    }

}
