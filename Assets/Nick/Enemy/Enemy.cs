using System.Collections;
using System;
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

    // [SerializeField] private float startingHealth;
    [SerializeField] private Rifle rifle;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private GameObject meshHolder;
    [SerializeField] private GameObject cryptidObject;
    [SerializeField] private MeshFilter bodyMesh;
    [SerializeField] private Renderer shader;
    [SerializeField] private Light bellyLight;
    private Vector3 meshPivotPointCorrection;

    [SerializeField] private GameObject weakpointObject;
    [SerializeField] private int numWeakpoints;
    private int startWeakpointsCount;

    [SerializeField] private AudioObject firstReactionToBear;
    [SerializeField] private AudioObject secondReactionToBear;
    [SerializeField] private AudioObject thirdReactionToBear;

    private List<GameObject> weakpoints;

    // [Header("Afterimage trail")]
    // [SerializeField] private float trailRate;
    // [SerializeField] private float trailTime;

    private MeshRenderer [] meshRenderers;

    private int lives = 3;


    private void Awake()
    {
        startWeakpointsCount = numWeakpoints;
    }

    private void OnDisable()
    {
        EventManager.Instance.talismanUsed -= AddWeakpoints;
        EventManager.Instance.rifleHit -= HitWeakpoint;

        
    }

    private void OnEnable()
    {
        EventManager.Instance.talismanUsed += AddWeakpoints;
        EventManager.Instance.rifleHit += HitWeakpoint;
        
        // meshPivotPointCorrection = meshHolder.transform.localPosition;
        //AddWeakpoints();      
    }

    // Add the weakpoints to the verticies of the cryptid
    private void AddWeakpoints()
    {
        if (lives == 3)
            Vocals.instance.Say(firstReactionToBear);
        else if (lives == 2)
            Vocals.instance.Say(secondReactionToBear);
        else if (lives == 1)
            Vocals.instance.Say(thirdReactionToBear);

        print("SPAWNING WEAKPOINTS");
        print(numWeakpoints);

        // Audio shii
        float tempSpatialBlend = audioSource.spatialBlend; 
        audioSource.pitch = 1.2f;
        audioSource.spatialBlend = 0;
        audioSource.Play();
        audioSource.spatialBlend = tempSpatialBlend;

        float difficultyScale = numWeakpoints / lives;
        numWeakpoints = Mathf.RoundToInt(difficultyScale);
        print(numWeakpoints);
        health = numWeakpoints;

        weakpoints = new List<GameObject>(numWeakpoints);

        for (int i = 0; i < numWeakpoints; i++)
        {
            int len = bodyMesh.sharedMesh.vertices.Length;
            int attempts = 0;
            
            Vector3 randomVertexPos = bodyMesh.sharedMesh.vertices[UnityEngine.Random.Range(0, len)];
            
            // Keep generating a random vertex for the weakpoint until it's on the visible side
            while (!(randomVertexPos.x > -0.035f && randomVertexPos.x < 0.045f) || randomVertexPos.y < -0.01f || !(randomVertexPos.z > 0.005f))
            {
                randomVertexPos = bodyMesh.sharedMesh.vertices[UnityEngine.Random.Range(0, len)];

                // Infinite loop protection
                if (attempts++ > len) break;
            }

            // Correct the random vertex
            Vector3 weakpointPosition = meshHolder.transform.position
                + Quaternion.Euler(meshHolder.transform.eulerAngles) * randomVertexPos * meshHolder.transform.localScale.x;
            
            // Instantiate the weakpoint
            GameObject wp = Instantiate(weakpointObject, weakpointPosition, Quaternion.identity, cryptidObject.transform);
            weakpoints.Add(wp);
        }

        //return weakpoints;
    }

    private void HitWeakpoint()
    {
        audioSource.pitch = UnityEngine.Random.Range(1, 1.25f);
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


    private void Die()
    {
        

        audioSource.pitch = 0.6f;
        audioSource.clip = deathSound;
        audioSource.Play();

        bellyLight.enabled = false;
        numWeakpoints = startWeakpointsCount;

        foreach (GameObject weakpoint in weakpoints)
            weakpoint.gameObject.SetActive(false);

        weakpoints.Clear();

        if (EventManager.Instance.enemyDie != null)
            EventManager.Instance.enemyDie.Invoke();

        if (EventManager.Instance.leavePlayer != null)
            EventManager.Instance.leavePlayer.Invoke();

        StartCoroutine(Dissolve(5.0f));

        // GameObject explosion = ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity, transform.parent.transform);
        // if (explosion.GetComponent<ParticleSystem>() != null)
        // {
        //     var explosionMain = explosion.GetComponent<ParticleSystem>().main;
        //     explosionMain.startSize = 50f;
        // }
    }

    IEnumerator Dissolve(float dissolveTime)
    {
        cryptidObject.GetComponent<SphereCollider>().enabled = false;
        cryptidObject.GetComponent<AudioSource>().enabled = false;

        float curTime = 0;
        while (curTime < dissolveTime)
        {
            float lerp = Mathf.Lerp(2, -10, curTime/dissolveTime);
            shader.material.SetFloat("_DissolveHeight", lerp);
            curTime += Time.deltaTime;
            yield return null;
        }
        

        // // Disable all children of the AI parent
        // foreach (Transform child in transform)
        //     child.gameObject.SetActive(false);


        // reset back
        cryptidObject.GetComponent<SphereCollider>().enabled = true;
        cryptidObject.GetComponent<AudioSource>().enabled = true;
        shader.material.SetFloat("_DissolveHeight", 2);
        bellyLight.enabled = true;
        cryptidObject.GetComponent<CoreAI>()._AIState = CoreAI.AIState.Passive;


        cryptidObject.SetActive(false);

        if (--lives <= 0 && EventManager.Instance.fadeToBlack != null)
            EventManager.Instance.fadeToBlack.Invoke(true);

        yield return null;
    }
}
