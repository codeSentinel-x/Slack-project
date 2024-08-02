using System;
using System.Collections.Generic;
using MyUtils.Classes;
using UnityEngine;

public class PathFinding {
    public const int DIAGONAL_COST = 15;
    public const int STRAIGHT_COST = 10;


    public List<PathFindingCellItem> _openList;
    public List<PathFindingCellItem> _closedList;
    public PathFindingCellItem[,] _gridOfCellItem;
    public Dictionary<Vector2Int, GameObject> _allChunks;
    public int _chunkSize;
    public int _viewRange;
    public Vector2Int _endInArrayPos;

    public PathFinding(int viewRange, Dictionary<Vector2Int, GameObject> allChunks, int chunkSize) {
        _viewRange = viewRange;
        _allChunks = allChunks;
        _chunkSize = chunkSize;
    }

    private PathFindingCellItem[,] GetCellsInRange(Vector2Int startPos, Vector2Int endPos) {
        PathFindingCellItem[,] result = new PathFindingCellItem[(2 * _viewRange) + 1, (2 * _viewRange) + 1];
        Debug.Log("X = " + result.GetLength(0) + "|| Y = " + result.GetLength(1));
        for (int i = -_viewRange; i <= _viewRange; i++) {
            for (int j = -_viewRange; j <= _viewRange; j++) {

                int chunkPosY = Mathf.FloorToInt((startPos.y + j) / _chunkSize);
                int chunkPosX = Mathf.FloorToInt((startPos.x + i) / _chunkSize);

                if (chunkPosX < 0 || chunkPosY < 0) {
                    result[_viewRange + i, _viewRange + j] = null;
                    continue;
                }

                int cellPosX = (startPos.x + i) % _chunkSize;
                int cellPosY = (startPos.y + j) % _chunkSize;
                Vector2Int chunkPos = new(chunkPosX, chunkPosY);

                try {

                    if (_allChunks.TryGetValue(chunkPos, out GameObject chunk)) {
                        if (chunk.TryGetComponent<ChunkController>(out var chunkController)) {
                            if (cellPosX >= 0 && cellPosX < _chunkSize && cellPosY >= 0 && cellPosY < _chunkSize) {
                                result[_viewRange + i, _viewRange + j] = new PathFindingCellItem() {
                                    _cell = chunkController._chunks[cellPosX, cellPosY],
                                    _x = _viewRange + i,
                                    _y = _viewRange + j,
                                    _worldPos = new Vector2Int(startPos.x + i, startPos.y + j)
                                };
                            }
                            else {
                                result[_viewRange + i, _viewRange + j] = null;
                            }

                        }

                        if (startPos.x + i == endPos.x && endPos.y == startPos.y + j) _endInArrayPos = new Vector2Int(_viewRange + i, _viewRange + j);
                    }
                }
                catch (SystemException e) {
                    Debug.Log("viewRange = " + _viewRange + ", j = " + j + ", i = " + i + '\n' + "Error: " + e);
                }

            }

        }
        return result;
    }
    public List<PathFindingCellItem> FindPath(Vector2Int startPos, Vector2Int endPos) {
        _gridOfCellItem = GetCellsInRange(startPos, endPos);
        PathFindingCellItem start = _gridOfCellItem[_viewRange, _viewRange];
        PathFindingCellItem end;
        try {
            end = _gridOfCellItem[_endInArrayPos.x, _endInArrayPos.y];
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
                if (cell == null) continue;
                if (_closedList.Contains(cell)) continue;

                if (!cell._cell._isWalkable) {
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
        int xDist = Mathf.Abs(a._worldPos.x - b._worldPos.x);
        int yDist = Mathf.Abs(a._worldPos.y - b._worldPos.y);
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

    //TODO later
    // [BurstCompile]
    // public struct FinsPath : IJob {
    //     public NativeList<ItemForJob> openList;
    //     public NativeList<ItemForJob> closeList;
    //     public void Execute() {


    //     }
    // }
    // public struct ItemForJob {
    //     public int _fCost;
    //     public int _hCost;
    //     public int _gCost;
    // }




}