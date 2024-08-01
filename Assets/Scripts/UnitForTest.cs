using System.Collections.Generic;
using UnityEngine;

public class UnitForTest : MonoBehaviour {
    [SerializeField] private int _viewRange;
    [SerializeField] private float _speed;
    private int _index;
    private List<Vector3> _transforms;

    private void Start() {
        MouseController._OnMouseClickLeft += FindCell;
    }

    private void Update() {
        _transforms ??= new();
        if (_index >= _transforms.Count) return;
        if (_transforms.Count > 0) {
            transform.position = Vector2.MoveTowards(transform.position, _transforms[_index], _speed * Time.deltaTime);
            if (transform.position == _transforms[_index]) _index++;
        }
    }

    public void ChangeSpeed(float v) {
        _speed = v;
    }

    private void FindCell(Vector2 vector) {
        Vector2Int startPos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        _viewRange = (int)Vector2.Distance(startPos, vector) + 5;
        PathFinding pathF = new(_viewRange, WorldGeneration._instance._currentChunksDict, WorldGeneration._chunkSize);
        var path = pathF.FindPath(startPos, new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y)));
        if (path == null) {
            Debug.Log("No way");
            return;
        }
        if (path != null) Debug.Log("Path generated successfully");
        _index = 0;
        _transforms = new();
        for (int i = 0; i < path.Count; i++) {
            _transforms.Add(new Vector3(path[i]._worldPos.x, path[i]._worldPos.y));
        }
    }

}
