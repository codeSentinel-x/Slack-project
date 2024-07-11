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

    public PathFindingCellItem[,] GetCellsInRange(Vector2Int startPos, int viewRange, Dictionary<Vector2Int, GameObject> allChunks, int chunkSize) {
        PathFindingCellItem[,] result = new PathFindingCellItem[viewRange * viewRange + 1, viewRange * viewRange + 1];

        Vector2Int currentChunk = new() {
            x = Mathf.FloorToInt(startPos.x / chunkSize),
            y = Mathf.FloorToInt(startPos.y / chunkSize),
        };
        int cellPosX, cellPosY;
        int chunkPosX, chunkPosY;
        for (int i = -viewRange; i <= viewRange; i++) {
            for (int j = -viewRange; j <= viewRange; j++) {

                chunkPosX = Mathf.FloorToInt((startPos.x - i) / WorldGeneration.chunkSize);
                chunkPosY = Mathf.FloorToInt((startPos.y - j) / WorldGeneration.chunkSize);

                cellPosX = Mathf.FloorToInt(startPos.x - chunkPosX * WorldGeneration.chunkSize);
                cellPosY = Mathf.FloorToInt(startPos.y - chunkPosY * WorldGeneration.chunkSize);

                if (chunkPosX < 0 || chunkPosY < 0 || cellPosY < 0 || cellPosX < 0) {
                    result[viewRange + i, viewRange + j] = null;
                    continue;
                }
                Vector2Int chunkPos = new(chunkPosX, chunkPosY);
                if (allChunks.TryGetValue(chunkPos, out GameObject chunk)) {
                    if (chunk.TryGetComponent<ChunkController>(out var chunkController)) {
                        result[viewRange + i, viewRange + j] = new PathFindingCellItem() {
                            _cell = chunkController.chunkH[cellPosX, cellPosY],
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
        //this code block make errors because result[x + viewRange] is not equal result[x]
        PathFindingCellItem start = _gridOfCellItem[startPos.x, startPos.y];
        PathFindingCellItem end;
        try {
            end = _gridOfCellItem[endPos.x, endPos.y];
        }
        catch {
            Debug.Log("cell cant be reached, out o view");
            return null;
        }
        //end

        _openList = new() { start };
        _closedList = new();

        for (int i = 0; i < _gridOfCellItem.GetLength(0); i++) {
            for (int j = 0; j < _gridOfCellItem.GetLength(0); j++) {
                if (_gridOfCellItem[i, j] == null) continue;
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