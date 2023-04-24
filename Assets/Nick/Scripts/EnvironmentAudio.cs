using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAudio : MonoBehaviour
{
    [SerializeField] private AudioClip crickets;
    [SerializeField] private AudioClip bossMusic;

    //[SerializeField] private Enemy enemy;

    private static AudioSource audioSource;

    // private bool soundsDisabled;
    // private float timeSinceLastPlay;
    // private float lastPitch = 1f;
    // private float delay;

    void Start()
    {
        EventManager.Instance.nearPlayer += EnableCryptidSounds;
        EventManager.Instance.leavePlayer += DisableCryptidSounds;
        audioSource = GetComponent<AudioSource>();
        DisableCryptidSounds();
    }

    void OnDisable()
    {
        EventManager.Instance.nearPlayer -= EnableCryptidSounds;
        EventManager.Instance.leavePlayer -= DisableCryptidSounds;
    }

    // void Update()
    // {
    //     if (!soundsDisabled && timeSinceLastPlay > delay)
    //     {
    //         timeSinceLastPlay = 0;
    //         PlayCryptidSound(distantCryptid, 0.15f, distantCryptid.clip.length);
    //     }
    //     timeSinceLastPlay += Time.deltaTime;
    // }

    // private void PlayCryptidSound(AudioSource sound, float pitchRange, float delay)
    // {
    //     float loPitch = Mathf.Max(sound.pitch - pitchRange, 0);
    //     float hiPitch = Mathf.Min(sound.pitch + pitchRange, 1);

    //     lastPitch = Random.Range(loPitch, hiPitch);
    //     sound.pitch = lastPitch;
    //     delay = sound.clip.length * 1/lastPitch;
    //     sound.Play();
    // }

    void EnableCryptidSounds()
    {
        audioSource.clip = bossMusic;
        audioSource.volume = 0.20f;
        audioSource.Play();
    }

    void DisableCryptidSounds()
    {
        StartCoroutine(LerpAudio(5f, crickets, 0.008f));
    }

    private static IEnumerator LerpAudio(float lerpTime, AudioClip newClip, float endVolume)
    {
        float startVolume = audioSource.volume;
        float curTime = 0;

        lerpTime *= 0.5f;

        while (curTime < lerpTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, curTime/lerpTime);
            curTime += Time.deltaTime;

            yield return null;
        }

        curTime = 0;
        audioSource.clip = newClip;
        audioSource.Play();

        while (curTime < lerpTime)
        {
            audioSource.volume = Mathf.Lerp(0f, endVolume, curTime/lerpTime);
            curTime += Time.deltaTime;

            yield return null;
        }

        

        yield return null;
    }
}
