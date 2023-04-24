using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartbeat : MonoBehaviour
{
    [SerializeField] private NickPlayerController player;

    private AudioSource audioSource;

    void OnEnable()
    {
        // EventManager.Instance.nearPlayer += EnableHeartbeat;
        // EventManager.Instance.leavePlayer += DisableHeartbeat;
        // EventManager.Instance.damageAction += UpdateHeartbeat;
    }

    void OnDisable()
    {
        EventManager.Instance.nearPlayer -= EnableHeartbeat;
        EventManager.Instance.leavePlayer -= DisableHeartbeat;
        EventManager.Instance.damageAction -= UpdateHeartbeat;
    }

    void Start()
    {
        EventManager.Instance.nearPlayer += EnableHeartbeat;
        EventManager.Instance.leavePlayer += DisableHeartbeat;
        EventManager.Instance.damageAction += UpdateHeartbeat;
        audioSource = GetComponent<AudioSource>();
    }

    private void EnableHeartbeat()
    {
        audioSource.Play();
    }

    private void UpdateHeartbeat()
    {
        if (player.Health > 66)
        {
            audioSource.pitch = 0.85f;
            audioSource.volume = 0.3f;
        }
        else if (player.Health > 33)
        {
            audioSource.pitch = 1f;
            audioSource.volume = 0.5f;
        }
        else
        {
            audioSource.pitch = 1.15f;
            audioSource.volume = 0.7f;
        }
            
    }

     private void DisableHeartbeat()
    {
        float timeToRelax = Mathf.Pow(player.Health/200, -1);
        StartCoroutine(RelaxHeartbeat(timeToRelax));
    }

    private IEnumerator RelaxHeartbeat(float lerpTime)
    {
        float startVolume = audioSource.volume;
        float curTime = 0;

        while (curTime < lerpTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, curTime/lerpTime);
            curTime += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }
}
