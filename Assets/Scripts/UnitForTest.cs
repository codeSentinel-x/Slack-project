using System.Collections.Generic;
using UnityEngine;

public class UnitForTest : MonoBehaviour {
    [SerializeField] private int viewRange;
    [SerializeField] private float speed;
    private int index;
    private List<Vector3> transforms;

    private void Start() {
        MouseController._OnMouseClickLeft += FindCell;
    }

    private void Update() {
        transforms ??= new();
        if (index >= transforms.Count) return;
        if (transforms.Count > 0) {
            transform.position = Vector2.MoveTowards(transform.position, transforms[index], speed * Time.deltaTime);
            if (transform.position == transforms[index]) index++;
        }
    }

    public void ChangeSpeed(float v) {
        speed = v;
    }

    private void FindCell(Vector2 vector) {
        Vector2Int startPos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        viewRange = (int)Vector2.Distance(startPos, vector) + 5;
        PathFinding pathF = new(viewRange, WorldGeneration._instance._currentChunksDict, WorldGeneration.chunkSize);
        var path = pathF.FindPath(startPos, new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y)));
        if (path == null) {
            Debug.Log("No way");
            return;
        }
        if (path != null) Debug.Log("Path generated successfully");
        index = 0;
        transforms = new();
        for (int i = 0; i < path.Count; i++) {
            transforms.Add(new Vector3(path[i]._worldPos.x, path[i]._worldPos.y));
        }
    }

}
