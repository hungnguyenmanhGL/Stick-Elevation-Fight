using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledEffect : PooledObject
{
    [SerializeField] protected ParticleSystem particleSys;
    [SerializeField] protected float liveTime = 2f;

    private void OnEnable() {
        particleSys.Play(true);
        StartCoroutine(DisableSelf());
    }

    protected override void OnDisable() {
        
    }

    private IEnumerator DisableSelf() {
        yield return new WaitForSeconds(liveTime);
        gameObject.SetActive(false);
        transform.parent = parent;
    }
}
