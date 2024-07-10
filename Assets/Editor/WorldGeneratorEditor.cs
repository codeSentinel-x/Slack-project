using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGeneration))]
public class WorldGeneratorEditor : Editor {
    public override void OnInspectorGUI() {

        WorldGeneration worldGen = (WorldGeneration)target;

        if (DrawDefaultInspector()) {
            //Maybe later
        }
        if (GUILayout.Button("Regenerate chunks")) {
            // worldGen.GenerateChunks();
            Debug.Log("Nuh uh");
        }
    }

}
