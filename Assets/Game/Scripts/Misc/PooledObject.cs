using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField] protected Transform parent;



    protected virtual void OnDisable() {
        transform.parent = parent;
    }

    public void SetParent(Transform parent) { this.parent = parent; }
}
