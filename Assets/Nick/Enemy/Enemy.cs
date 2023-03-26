using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{    
    private float health;
    private bool hasDied;
    public float Health
    {
        get { return health; }
    }

    [SerializeField] private float startingHealth;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private GameObject meshHolder;

    public bool ChangeHealth(float healthChange)
    {
        if (hasDied) return true;

        health += healthChange;
        health = Mathf.Clamp(health, 0, startingHealth);

        if (health <= 0)
        {
            Die();
            hasDied = true;
        }

        return hasDied;
    }

    private void Die()
    {
        audioSource.clip = deathSound;
        audioSource.Play();

        StartCoroutine(SetInactive(deathSound.length));

        ParticleSystem explosion = Instantiate(deathParticle, transform.position, Quaternion.identity);
        var explosionMain = explosion.main;
        explosionMain.startSize = 50f;
        Destroy(explosion.gameObject, 1f);
    }

    IEnumerator SetInactive(float timeBeforeDestroying)
    {
        meshHolder.SetActive(false);
        yield return new WaitForSeconds(timeBeforeDestroying);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        health = startingHealth;       
    }
}
