using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using UnityEngine;

public class PathFinding {
    public const int DIAGONAL_COST = 15;
    public const int STRAIGHT_COST = 10;
    public List<PathNode> _openList;
    public List<PathNode> _closedList;
    public PathNode[,] _gridOfNodes;


    public PathFinding(Vector2Int startPos, int viewRange, Dictionary<Vector2Int, GameObject> allChunks, int chunkSize) {
        List<ChunkItem> chunksInRange = GetCellsInRange(startPos, viewRange, allChunks, chunkSize);
    }
    public List<ChunkItem> GetCellsInRange(Vector2Int startPos, int viewRange, Dictionary<Vector2Int, GameObject> allChunks, int chunkSize) {
        List<ChunkItem> result = new();

        int inChunkPosX, inChunkPosY;
        int chunkPosX, chunkPosY;
        for (int i = -viewRange; i <= viewRange; i++) {
            for (int j = -viewRange; j <= viewRange; j++) {
                if ()
                    Vector2Int chunkPos = new(chunkPosX, chunkPosY);
                if (allChunks.TryGetValue(chunkPos, out GameObject chunk)) {
                    if (chunk.TryGetComponent<ChunkController>(out var chunkController)) {
                        result.Add(chunkController);
                    }
                }
            }
        }

        return result;
    }
   
}