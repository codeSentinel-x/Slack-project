using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using UnityEngine;

public class PathFinding {
    public const int DIAGONAL_COST = 15;
    public const int STRAIGHT_COST = 10;


    public List<PathFindingCellItem> _openList;
    public List<PathFindingCellItem> _closedList;
    public PathFindingCellItem[,] _gridOfCellItem;

    public PathFinding(Vector2Int startPos, int viewRange, Dictionary<Vector2Int, GameObject> allChunks, int chunkSize) {
        _gridOfCellItem = GetCellsInRange(startPos, viewRange, allChunks, chunkSize);
    }

    //This function won't work because I should take position from world origin not current chunk
    public PathFindingCellItem[,] GetCellsInRange(Vector2Int startPos, int viewRange, Dictionary<Vector2Int, GameObject> allChunks, int chunkSize) {
        PathFindingCellItem[,] result = new PathFindingCellItem[viewRange * viewRange + 1, viewRange * viewRange + 1];

        Vector2Int currentChunk = new() {
            x = Mathf.FloorToInt(startPos.x / chunkSize),
            y = Mathf.FloorToInt(startPos.y / chunkSize),
        };
        int inChunkPosX, inChunkPosY;
        int chunkPosX, chunkPosY;
        for (int i = -viewRange; i <= viewRange; i++) {
            for (int j = -viewRange; j <= viewRange; j++) {
                inChunkPosX = startPos.x + i;
                inChunkPosY = startPos.y + j;
                chunkPosX = currentChunk.x;
                chunkPosY = currentChunk.y;
                if (inChunkPosX > 0) {
                    chunkPosX -= Mathf.CeilToInt((float)Mathf.Abs(inChunkPosX) / chunkSize);
                    inChunkPosX += Mathf.Abs(chunkPosX * chunkSize);
                }
                else {
                    chunkPosX += Mathf.CeilToInt((float)Mathf.Abs(inChunkPosX) / chunkSize);
                    inChunkPosX -= Mathf.Abs(chunkPosX * chunkSize);
                }
                if (inChunkPosY > 0) {
                    chunkPosY -= Mathf.CeilToInt((float)Mathf.Abs(inChunkPosY) / chunkSize);
                    inChunkPosY += Mathf.Abs(chunkPosY * chunkSize);
                }
                else {
                    chunkPosY += Mathf.CeilToInt((float)Mathf.Abs(inChunkPosY) / chunkSize);
                    inChunkPosY -= Mathf.Abs(chunkPosX * chunkSize);
                }

                Vector2Int chunkPos = new(chunkPosX, chunkPosY);
                if (allChunks.TryGetValue(chunkPos, out GameObject chunk)) {
                    if (chunk.TryGetComponent<ChunkController>(out var chunkController)) {
                        result[i, j] = new PathFindingCellItem() {
                            _cell = chunkController.chunkH[inChunkPosX, inChunkPosY],
                            _x = chunkPosX,
                            _y = chunkPosY
                        };
                    }
                }
            }

        }
        return result;
    }
    public List<PathFindingCellItem> FindPath(Vector2Int startPos, Vector2Int endPos) {
        PathFindingCellItem start = _gridOfCellItem[startPos.x, startPos.y];
        PathFindingCellItem end;
        try {
            end = _gridOfCellItem[endPos.x, endPos.y];
        }
        catch {
            Debug.Log("cell cant be reached, out o view");
            return null;
        }

        _openList = new() { start };
        _closedList = new();

        for (int i = 0; i < _gridOfCellItem.GetLength(0); i++) {
            for (int j = 0; j < _gridOfCellItem.GetLength(0); j++) {
                PathFindingCellItem cellItem = _gridOfCellItem[i, j];
                cellItem._gCost = int.MaxValue;
                cellItem._previous = null;
                cellItem.CalculateFCost();
                _gridOfCellItem[i, j] = cellItem;
            }
        }

        start._gCost = 0;
        start._hCost = CalculateDistance(start, end);
        start.CalculateFCost();

        while (_openList.Count > 0) {
            PathFindingCellItem current = GetLowestFCost(_openList);
            if (current._x == end._x && current._y == end._y) {
                return CalculatePath(current);
            }
            _openList.Remove(current);
            _closedList.Add(current);

            foreach (PathFindingCellItem cell in GetNeighborList(current)) {
                if (_closedList.Contains(cell)) continue;
                if (!cell._cell.isWalkable) {
                    _closedList.Add(cell);
                    continue;
                }
                int tentativeCost = current._gCost + CalculateDistance(current, cell);
                if (tentativeCost < cell._gCost) {
                    cell._previous = current;
                    cell._gCost = tentativeCost;
                    cell._hCost = CalculateDistance(cell, end);
                    cell.CalculateFCost();
                }
                if (!_openList.Contains(cell)) _openList.Add(cell);
            }
        }
        return null;


    }
    private List<PathFindingCellItem> GetNeighborList(PathFindingCellItem currentNode) {
        List<PathFindingCellItem> neighborList = new();
        if (currentNode._x - 1 >= 0) {
            neighborList.Add(GetNode(currentNode._x - 1, currentNode._y));
            if (currentNode._y - 1 >= 0) neighborList.Add(GetNode(currentNode._x - 1, currentNode._y - 1));
            if (currentNode._y + 1 < _gridOfCellItem.GetLength(0)) neighborList.Add(GetNode(currentNode._x - 1, currentNode._y + 1));
        }
        if (currentNode._x + 1 < _gridOfCellItem.GetLength(1)) {
            neighborList.Add(GetNode(currentNode._x + 1, currentNode._y));
            if (currentNode._y - 1 >= 0) neighborList.Add(GetNode(currentNode._x + 1, currentNode._y - 1));
            if (currentNode._y + 1 < _gridOfCellItem.GetLength(0)) neighborList.Add(GetNode(currentNode._x + 1, currentNode._y + 1));
        }
        if (currentNode._y - 1 >= 0) neighborList.Add(GetNode(currentNode._x, currentNode._y - 1));
        if (currentNode._y + 1 < _gridOfCellItem.GetLength(0)) neighborList.Add(GetNode(currentNode._x, currentNode._y + 1));
        return neighborList;
    }

    private PathFindingCellItem GetNode(int x, int y) {
        return _gridOfCellItem[x, y];
    }

    private List<PathFindingCellItem> CalculatePath(PathFindingCellItem node) {
        List<PathFindingCellItem> path = new() {
             node
         };
        PathFindingCellItem current = node;
        while (current._previous != null) {
            path.Add(current._previous);
            current = current._previous;
        }
        path.Reverse();
        return path;

    }

    private int CalculateDistance(PathFindingCellItem a, PathFindingCellItem b) {
        int xDist = Mathf.Abs(a._x - b._x);
        int yDist = Mathf.Abs(a._y - b._y);
        int remain = Mathf.Abs(xDist - yDist);
        return DIAGONAL_COST * Mathf.Min(xDist, yDist) + STRAIGHT_COST * remain;
    }
    private PathFindingCellItem GetLowestFCost(List<PathFindingCellItem> pathNodes) {
        PathFindingCellItem lowestF = pathNodes[0];
        for (int i = 0; i < pathNodes.Count; i++) {
            if (pathNodes[i]._fCost < lowestF._fCost) lowestF = pathNodes[i];
        }
        return lowestF;
    }





}