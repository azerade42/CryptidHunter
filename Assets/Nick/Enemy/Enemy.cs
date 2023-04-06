using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{    
    public UnityAction nearPlayer;
    public UnityAction leavePlayer;
    
    // private float health;
    private bool hasDied;
    // public float Health
    // {
    //     get { return health; }
    // }

    // [SerializeField] private float startingHealth;
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

    private void Start()
    {
        // health = startingHealth; 
        // meshPivotPointCorrection = meshHolder.transform.localPosition;
        AddWeakpoints();      
    }

    private GameObject [] AddWeakpoints()
    {
        weakpoints = new GameObject[numWeakpoints];

        // float hX = 0;
        // float hY = 0;
        // float hZ = 0;

        for (int i = 0; i < numWeakpoints; i++)
        {
            int len = bodyMesh.sharedMesh.vertices.Length;
            
            Vector3 randomVertexPos = Vector3.zero;

            int attempts = 0;
            
            // Generate a random vertex for the weakpoint
            while (!(randomVertexPos.x > -0.03f && randomVertexPos.x < 0.04f) || randomVertexPos.y < -0.01f || (randomVertexPos.z == 0))
            {
                randomVertexPos = bodyMesh.sharedMesh.vertices[Random.Range(0, len)];
                // hX = Mathf.Max(hX, Mathf.Abs(randomVertexPos.x));
                // hY = Mathf.Min(hY, Mathf.Abs(randomVertexPos.y));
                // hZ = Mathf.Max(hZ, Mathf.Abs(randomVertexPos.z));

                // Debug.Log(i + " " + randomVertexPos.z);

                // Infinite loop protection
                if (attempts++ > 100) break;
            }

            // Random vertex position
            Vector3 weakpointPosition = transform.position //+ meshPivotPointCorrection 
                + Quaternion.Euler(meshHolder.transform.eulerAngles) * randomVertexPos * meshHolder.transform.localScale.x;
            
            GameObject wp = Instantiate(weakpointObject, weakpointPosition, Quaternion.identity, cryptidObject.transform);
            weakpoints[i] = wp;
            
            wp.transform.localScale *= 0;
        }

        //  Debug.Log(hX + " " + hY + " " + hZ);

        return weakpoints;
    }

    private void HitWeakpoint()
    {

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
        audioSource.clip = deathSound;
        audioSource.Play();

        StartCoroutine(SetInactive(deathSound.length));

        // Explosion Particle
        ParticleSystem explosion = Instantiate(deathParticle, transform.position, Quaternion.identity, gameObject.transform);
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
}
