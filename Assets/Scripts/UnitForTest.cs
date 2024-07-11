using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitForTest : MonoBehaviour {
    public int viewRange;
    public int speed;
    int index;
    public List<Vector3> transforms;
    void Start() {
        MouseController._instance.OnMouseClick += FindCell;
    }

    void Update() {
        transforms ??= new();
        if (index >= transforms.Count) return;
        if (transforms.Count > 0) {
            transform.position = Vector2.MoveTowards(transform.position, transforms[index], speed);
            if (transform.position == transforms[index]) index++;
        }
    }

    private void FindCell(Vector2 vector) {
        Vector2Int startPos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        PathFinding pathF = new(startPos, viewRange, WorldGeneration._instance.chunks, WorldGeneration.chunkSize);
        var path = pathF.FindPath(startPos, new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y)));
        if (path == null) {
            Debug.Log("No way");
            return;
        }
        index = 0;
        transforms = new();
        for (int i = 0; i < path.Count; i++) {
            transforms[i] = new Vector3(path[i]._x, path[i]._y);
        }
    }

}
