using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private List<GameObject> spawnList = new List<GameObject>();

    public GameObject Prefab => prefab;

    public void Init() {
        for (int i=0; i<10; i++) {
            Spawn();
        }
    }

    public void InitWithPrefab(GameObject prefab) {
        this.prefab = prefab;
        Init();
    }

    public GameObject Spawn() {
        GameObject spawn = GameObject.Instantiate(prefab);
        spawn.SetActive(false);
        spawn.transform.parent = this.transform;

        PooledObject pooledObj = spawn.GetComponent<PooledObject>();
        if (pooledObj) {
            pooledObj.SetParent(this.transform);
        }
        spawnList.Add(spawn);
        return spawn;
    }

    public GameObject GetSpawn() {
        foreach (GameObject obj in spawnList) {
            if (!obj.activeInHierarchy) {
                obj.transform.parent = null;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject spawn = Spawn();
        spawn.transform.parent = null;
        spawn.SetActive(true);
        return spawn; 
    }

}
