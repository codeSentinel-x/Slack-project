using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    private MouseController mouseController;
    void Start() {
        mouseController = MouseController._instance;
        mouseController.OnMouseClick += GetClickedCell;
    }


    private void GetClickedCell(Vector2 vector) {
        Vector2Int chunkPos = new() {
            x = Mathf.FloorToInt(vector.x) / WorldGeneration.chunkSize,
            y = Mathf.FloorToInt(vector.y) / WorldGeneration.chunkSize,
        };
        // WorldGeneration._instance.
    }

}
