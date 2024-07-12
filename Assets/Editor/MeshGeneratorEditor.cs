using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator3D))]
public class MeshGeneratorEditor : Editor {
    public override void OnInspectorGUI() {

        MeshGenerator3D meshGen = (MeshGenerator3D)target;

        if (DrawDefaultInspector()) {
            //Maybe later
        }
        if (GUILayout.Button("Generate")) {
            // meshGen.GenerateMesh();
        }
    }

}
