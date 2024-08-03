using System.Collections.Generic;
using UnityEngine;

public class ChunkSystem : MonoBehaviour {

    [SerializeField] private int _chunkRenderDistance;
    [SerializeField] private Transform _spriteRenderer;
    private Vector2Int _lastChunkPosition;

    private void Start() {
        // MouseController._instance.OnMouseClick += (x) => transform.position = x;
        WorldGeneration._instance._OnNoiseSettingChange += (x) => {
            _chunkRenderDistance = x;
            GenerateAdvancedChunkInRange(true);
        };

    }

    private void Update() {
        CheckForPositionUpdate();
    }

    private void CheckForPositionUpdate() {
        Vector2Int currentChunkPosition = new(Mathf.FloorToInt(transform.position.x / WorldGeneration._chunkSize), Mathf.FloorToInt(transform.position.y / WorldGeneration._chunkSize));
        if (currentChunkPosition.x < 0 || currentChunkPosition.y < 0) return;
        if (currentChunkPosition != _lastChunkPosition) {
            _lastChunkPosition = currentChunkPosition;
            OnChunkChange();
        }
    }

    public void OnChunkChange() {
        Debug.Log("CurrentChunk: " + _lastChunkPosition);
        GenerateAdvancedChunkInRange();
    }

    public void GenerateAdvancedChunkInRange(bool doNotLookForOld = false) {
        List<Vector2Int> chunksInRange = new();
        Vector2Int chunkPos;
        for (int i = -_chunkRenderDistance; i <= _chunkRenderDistance; i++) {
            for (int j = -_chunkRenderDistance; j <= _chunkRenderDistance; j++) {
                chunkPos = new Vector2Int(_lastChunkPosition.x + i, _lastChunkPosition.y + j);
                if (chunkPos.x >= 0 && chunkPos.y >= 0) {
                    chunksInRange.Add(chunkPos);
                }
            }
        }

        if (!doNotLookForOld) {

            WorldGeneration._instance._oldChunksDict = WorldGeneration._instance._currentChunksDict;
            WorldGeneration._instance._currentChunksDict = new();

            foreach (var c in WorldGeneration._instance._oldChunksDict) {
                if (!chunksInRange.Contains(c.Key)) {
                    Destroy(c.Value);
                }
                else {
                    WorldGeneration._instance._currentChunksDict[c.Key] = c.Value;
                }
            }
        }
        else {
            WorldGeneration._instance._currentChunksDict = new();
            _spriteRenderer.transform.localPosition = WorldGeneration._currentSettingsData._settings._chunkSize % 2 == 0 ? new(0.5f, 0.5f, 0) : new(0, 0, 0);
        }
        foreach (var i in chunksInRange) {
            if (!WorldGeneration._instance._currentChunksDict.ContainsKey(i)) {
                WorldGeneration._instance.GenerateAdvancedChunkAt(i);
            }
        }

    }

    //This method is for slider
    public void ChangeRenderDist(float v) {
        _chunkRenderDistance = (int)v;
        GenerateAdvancedChunkInRange();
    }
}
