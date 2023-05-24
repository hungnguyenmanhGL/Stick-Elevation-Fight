using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathHolder : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<Vector2Int> pathData = new List<Vector2Int>();
    private Dictionary<Vector2Int, bool> pathDict = new Dictionary<Vector2Int, bool>();

    private static readonly string objName = "Elevator Path";

    public Tilemap Tilemap => tilemap;
    public Dictionary<Vector2Int, bool> PathDict => pathDict;

    private void Start() {
        if (!tilemap) tilemap = GetComponent<Tilemap>();

        Transform[] childArr = GetComponentsInChildren<Transform>();
        foreach (Transform t in childArr) {
            if (t.gameObject.name.Equals(objName)) {
                Vector2Int cell = (Vector2Int)tilemap.WorldToCell(t.position);
                pathDict.Add(cell, true);
            }
        }
    }

    public bool CheckCell(Vector2Int cell) {
        if (!pathDict.ContainsKey(cell)) return false;

        return pathDict[cell];
    }

    public List<Vector2Int> GetPath(Vector2Int startCell, Vector2Int endCell) {
        if (!CheckCell(startCell) || !CheckCell(endCell)
            || startCell == endCell) return null;

        //init list(s) to save paths
        List<List<Vector2Int>> pathList = new List<List<Vector2Int>>();
        List<Vector2Int> initList = new List<Vector2Int>();
        pathList.Add(initList);
        pathList[0].Add(startCell);

        //init direction for more convenient use
        Vector2Int up = Vector2Int.up;
        Vector2Int down = Vector2Int.down;
        Vector2Int left = Vector2Int.left;
        Vector2Int right = Vector2Int.right;
        Dictionary<Vector2Int, bool> directionDict = new Dictionary<Vector2Int, bool>() {
            {up, false },
            {down, false },
            {left, false },
            {right, false },
        };

        //init queue for BFS
        Queue<PathCellData> cellQueue = new Queue<PathCellData>();
        cellQueue.Enqueue(new PathCellData(startCell, new Vector2Int(-9999,-9999)));
        while (cellQueue.Count > 0) {
            PathCellData temp = cellQueue.Dequeue();
            //start cell already added
            if (temp.cell != startCell) {
                foreach (List<Vector2Int> path in pathList) {
                    if (path.Count > 0 && path[path.Count - 1] == temp.lastCell) {
                        path.Add(temp.cell);
                        //if added to 1 path already -> break to preserve duplicate lists (in cases of multiple directions)
                        break;
                    }
                }
            }

            int adjacentCount = 0;
            //reset dict values to check for adjacent cell(s)
            directionDict[up] = false;
            directionDict[down] = false;
            directionDict[left] = false;
            directionDict[right] = false;
            
            if (CheckCell(temp.cell + up) && temp.cell + up != temp.lastCell) {
                directionDict[up] = true;
                adjacentCount++;
            }
            if (CheckCell(temp.cell + down) && temp.cell + down != temp.lastCell) {
                directionDict[down] = true;
                adjacentCount++; 
            }
            if (CheckCell(temp.cell + left) && temp.cell + left != temp.lastCell) {
                directionDict[left] = true;
                adjacentCount++;
            }
            if (CheckCell(temp.cell + right) && temp.cell + right != temp.lastCell) {
                directionDict[right] = true;
                adjacentCount++;
            }

            if (adjacentCount == 0) { }
            else if (adjacentCount == 1) {
                //if no new path -> add the next cell to last path list and queue
                foreach (KeyValuePair<Vector2Int,bool> pair in directionDict) {
                    if (pair.Value) {
                        PathCellData data = new PathCellData(pair.Key + temp.cell, temp.cell);
                        cellQueue.Enqueue(data);
                        //pathList[pathList.Count - 1].Add(data);
                        //Debug.Log(temp.cell + ": " + pair.Key + "->" + (pair.Key + temp.cell));
                        break;
                    }
                }
            }
            else {
                //duplicate the list with the last cell match current cell for future path addition
                List<Vector2Int> pathToDuplicate = new List<Vector2Int>();
                foreach (List<Vector2Int> path in pathList) {
                    if (path[path.Count - 1] == temp.cell) {
                        pathToDuplicate = path;
                        break;
                    }
                }

                adjacentCount = adjacentCount - 1;
                for (int i=0; i<adjacentCount; i++) {
                    pathList.Add(new List<Vector2Int>(pathToDuplicate));
                }

                foreach (KeyValuePair<Vector2Int, bool> pair in directionDict) {
                    if (pair.Value) {
                        PathCellData data = new PathCellData(temp.cell + pair.Key, temp.cell);
                        cellQueue.Enqueue(data);
                        //Debug.Log(temp.cell + ": " + pair.Key + "->" + (pair.Key + temp.cell));
                        //pathList[pathList.Count - 1].Add(data);
                    }
                }
            }
        }

        int pathLength = int.MaxValue;
        foreach (List<Vector2Int> path in pathList) {
            if (path.Contains(endCell)) {
                while (path[path.Count -1] != endCell) {
                    path.RemoveAt(path.Count - 1);
                }
                if (path.Count < pathLength) {
                    pathLength = path.Count;
                    pathData.Clear();
                    pathData = path;
                }
            }
        }
        return pathData;
    }
}

[System.Serializable]
public struct PathCellData {
    public Vector2Int cell;
    public Vector2Int lastCell;

    public PathCellData(Vector2Int cell, Vector2Int lastCell) {
        this.cell = cell;
        this.lastCell = lastCell;
    }
}

