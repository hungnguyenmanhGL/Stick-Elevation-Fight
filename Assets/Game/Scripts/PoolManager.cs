using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [SerializeField] private List<ObjectPool> poolList;
    private Dictionary<GameObject, ObjectPool> poolDict = new Dictionary<GameObject, ObjectPool>();
    
    void Start()
    {
        if (!instance) instance = this;
        else Destroy(this);

        foreach (ObjectPool pool in poolList) {
            poolDict.Add(pool.Prefab, pool);
            pool.Init();
        }
    }

    public GameObject GetObject(GameObject obj) {
        if (!poolDict.ContainsKey(obj)) {
            ObjectPool pool = gameObject.AddComponent<ObjectPool>();
            pool.InitWithPrefab(obj);
            poolDict.Add(obj, pool);
        }

        GameObject re = poolDict[obj].GetSpawn();
        return re;
    }
}
