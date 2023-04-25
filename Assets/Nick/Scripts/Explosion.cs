using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    ParticleSystem particles;

    public void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }
    public void OnObjectSpawn()
    {
        particles.Play();
        StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1f);
        gameObject.transform.SetParent(ObjectPooler.Instance.gameObject.transform);
        gameObject.SetActive(false);
    }
}
