using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathHolder : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
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
}
