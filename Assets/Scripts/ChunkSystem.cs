using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSystem : MonoBehaviour {

    public Vector2Int lastChunkPosition;
    public int chunkRenderDistance;
    public Transform spriteRenderer;

    void Start() {
        // MouseController._instance.OnMouseClick += (x) => transform.position = x;
        WorldGeneration._instance.OnNoiseSettingChange += (x) => {
            chunkRenderDistance = x;
            GenerateChunkInRange(true);
        };

    }
    public void ChangeRenderDist(float v) {
        chunkRenderDistance = (int)v;
        GenerateChunkInRange();
    }

    void Update() {
        // transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int currentChunkPosition = new(Mathf.FloorToInt(transform.position.x / WorldGeneration.chunkSize), Mathf.FloorToInt(transform.position.y / WorldGeneration.chunkSize));
        if (currentChunkPosition.x < 0 || currentChunkPosition.y < 0) return;
        if (currentChunkPosition != lastChunkPosition) {
            lastChunkPosition = currentChunkPosition;
            OnChunkChange();
        }
    }

    public void OnChunkChange() {
        Debug.Log("CurrentChunk: " + lastChunkPosition);
        GenerateChunkInRange();
    }
    public void GenerateChunkInRange(bool doNotLookForOld = false) {
        List<Vector2Int> chunksInRange = new();
        Vector2Int chunkPos;
        for (int i = -chunkRenderDistance; i <= chunkRenderDistance; i++) {
            for (int j = -chunkRenderDistance; j <= chunkRenderDistance; j++) {
                chunkPos = new Vector2Int(lastChunkPosition.x + i, lastChunkPosition.y + j);
                if (chunkPos.x >= 0 && chunkPos.y >= 0) {
                    chunksInRange.Add(chunkPos);
                }
            }
        }

        if (!doNotLookForOld) {

            WorldGeneration._instance.oldChunks = WorldGeneration._instance.chunks;
            WorldGeneration._instance.chunks = new();

            foreach (var c in WorldGeneration._instance.oldChunks) {
                if (!chunksInRange.Contains(c.Key)) {
                    Destroy(c.Value);
                    // Debug.Log("Destroying");
                }
                else {
                    WorldGeneration._instance.chunks[c.Key] = c.Value;
                }
            }
        }
        else {
            WorldGeneration._instance.chunks = new();
            spriteRenderer.transform.localPosition = WorldGeneration.currentSettings.chunkSize % 2 == 0 ? new(0.5f, 0.5f, 0) : new(0, 0, 0);
        }
        foreach (var i in chunksInRange) {
            if (!WorldGeneration._instance.chunks.ContainsKey(i)) {
                WorldGeneration._instance.GenerateChunkAt(i);
            }
        }

    }
}
