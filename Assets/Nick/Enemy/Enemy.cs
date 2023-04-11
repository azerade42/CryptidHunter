using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{    
    public UnityAction nearPlayer;
    public UnityAction leavePlayer;
    
    private float health;
    private bool hasDied;
    public float Health
    {
        get { return health; }
    }

    // [SerializeField] private float startingHealth;
    [SerializeField] private Rifle rifle;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private GameObject meshHolder;
    [SerializeField] private GameObject cryptidObject;
    [SerializeField] private MeshFilter bodyMesh;
    private Vector3 meshPivotPointCorrection;

    [SerializeField] private GameObject weakpointObject;
    [SerializeField] private int numWeakpoints;

    private GameObject [] weakpoints;

    [Header("Afterimage trail")]
    [SerializeField] private float trailRate;
    [SerializeField] private float trailTime;

    private MeshRenderer [] meshRenderers;

    private void OnEnable()
    {
        rifle.rifleHit += HitWeakpoint;
    }

    private void Start()
    {
        health = numWeakpoints;
        // meshPivotPointCorrection = meshHolder.transform.localPosition;
        AddWeakpoints();      
    }

    private GameObject [] AddWeakpoints()
    {
        weakpoints = new GameObject[numWeakpoints];

        for (int i = 0; i < numWeakpoints; i++)
        {
            int len = bodyMesh.sharedMesh.vertices.Length;
            int attempts = 0;
            
            Vector3 randomVertexPos = bodyMesh.sharedMesh.vertices[Random.Range(0, len)];
            
            // Generate a random vertex for the weakpoint
            while (!(randomVertexPos.x > -0.035f && randomVertexPos.x < 0.045f) || randomVertexPos.y < -0.01f || !(randomVertexPos.z > 0.005f))
            {
                randomVertexPos = bodyMesh.sharedMesh.vertices[Random.Range(0, len)];

                // Infinite loop protection
                if (attempts++ > len) break;
            }

            // Correct the random vertex
            Vector3 weakpointPosition = transform.position //+ meshPivotPointCorrection 
                + Quaternion.Euler(meshHolder.transform.eulerAngles) * randomVertexPos * meshHolder.transform.localScale.x;
            
            GameObject wp = Instantiate(weakpointObject, weakpointPosition, Quaternion.identity, cryptidObject.transform);
            weakpoints[i] = wp;
            
            wp.transform.localScale *= 0;
        }

        return weakpoints;
    }

    private void HitWeakpoint()
    {
        audioSource.pitch = Random.Range(1, 1.25f);
        audioSource.Play();
        if (--health <= 0)
        {
            Die();
        }
    }

    

    // private void Update()
    // {
    //     StartCoroutine(ActivateTrail(trailTime));
    // }

    // IEnumerator ActivateTrail(float activeTime)
    // {
    //     while (activeTime > 0)
    //     {
    //         activeTime -= trailRate;

    //         if (meshRenderers == null)
    //             meshRenderers = GetComponentsInChildren<MeshRenderer>();
            
    //         foreach (MeshRenderer m in meshRenderers)
    //         {
    //             GameObject gObj = new GameObject();

    //             MeshRenderer meshRenderer = gObj.AddComponent<MeshRenderer>();
    //             MeshFilter meshFilter = gObj.AddComponent<MeshFilter>();

    //             Mesh mesh = new Mesh();
    //         }
    //     }
    // }

    // public bool ChangeHealth(float healthChange)
    // {
    //     if (hasDied) return true;

    //     health += healthChange;
    //     health = Mathf.Clamp(health, 0, startingHealth);

    //     if (health <= 0)
    //     {
    //         Die();
    //         hasDied = true;
    //     }

    //     return hasDied;
    // }

    private void Die()
    {
        audioSource.pitch = 1f;
        audioSource.clip = deathSound;
        audioSource.Play();

        StartCoroutine(SetInactive(deathSound.length));

        GameObject explosion = ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity, meshHolder.transform);
        if (explosion.GetComponent<ParticleSystem>() != null)
        {
            var explosionMain = explosion.GetComponent<ParticleSystem>().main;
            explosionMain.startSize = 50f;
        }
    }

    IEnumerator SetInactive(float timeBeforeDestroying)
    {
        foreach (Transform child in meshHolder.transform)
            child.gameObject.SetActive(false);
            
        cryptidObject.GetComponent<SphereCollider>().enabled = false;
        cryptidObject.GetComponent<AudioSource>().enabled = false;

        yield return new WaitForSeconds(timeBeforeDestroying);
        gameObject.SetActive(false);
    }
}
