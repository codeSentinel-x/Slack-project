using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSystem : MonoBehaviour {

    public Vector2Int lastChunkPosition;
    public int chunkRenderDistance;
    void Start() {
        MouseController._instance.OnMouseClick += (x) => transform.position = x;
    }


    void Update() {
        Vector2Int currentChunkPosition = new(Mathf.FloorToInt(transform.position.x / WorldGeneration.chunkSize), Mathf.FloorToInt(transform.position.y / WorldGeneration.chunkSize));
        if (currentChunkPosition != lastChunkPosition) {
            lastChunkPosition = currentChunkPosition;
            OnChunkChange();
        }
    }

    public void OnChunkChange() {
        Debug.Log("CurrentChunk: " + lastChunkPosition);
    }
    public void GenerateChunkInRange() {
        List<Vector2Int> chunksInRange = new();
        for (int i = 0; i < chunkRenderDistance; i++) {
            for (int j = 0; j < chunkRenderDistance; j++) {
                if (lastChunkPosition.y - j >= 0 && lastChunkPosition.x - i >= 0) {
                    if (!chunksInRange.Contains(new Vector2Int(lastChunkPosition.x - i, lastChunkPosition.y - j))) chunksInRange.Add(new Vector2Int(lastChunkPosition.x - i, lastChunkPosition.y - j));
                }
                if (lastChunkPosition.y + j >= 0 && lastChunkPosition.x + i >= 0) {
                    if (!chunksInRange.Contains(new Vector2Int(lastChunkPosition.x + i, lastChunkPosition.y + j))) chunksInRange.Add(new Vector2Int(lastChunkPosition.x + i, lastChunkPosition.y + j));
                }
                if (lastChunkPosition.y - j >= 0 && lastChunkPosition.x + i >= 0) {
                    if (!chunksInRange.Contains(new Vector2Int(lastChunkPosition.x + i, lastChunkPosition.y - j))) chunksInRange.Add(new Vector2Int(lastChunkPosition.x + i, lastChunkPosition.y - j));
                }
                if (lastChunkPosition.y + j >= 0 && lastChunkPosition.x - i >= 0) {
                    if (!chunksInRange.Contains(new Vector2Int(lastChunkPosition.x - i, lastChunkPosition.y + j))) chunksInRange.Add(new Vector2Int(lastChunkPosition.x - i, lastChunkPosition.y + j));
                }
            }
        }
        foreach (var c in WorldGeneration._instance.chunks) {
            if (!chunksInRange.Contains(c.Key)) {
                Destroy(c.Value);
            }
        }
        Dictionary<Vector2Int, GameObject> newDict = new();
        foreach (var i in chunksInRange) {
            if (WorldGeneration._instance.chunks.ContainsKey(i)) continue;

        }

    }
}
