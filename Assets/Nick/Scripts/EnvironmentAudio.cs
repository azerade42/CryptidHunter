using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAudio : MonoBehaviour
{
    [SerializeField] private AudioClip crickets;
    [SerializeField] private AudioClip bossMusic;

    [SerializeField] private CoreAI cryptid;
    [SerializeField] private Enemy enemy;

    private AudioSource audioSource;

    // private bool soundsDisabled;
    // private float timeSinceLastPlay;
    // private float lastPitch = 1f;
    // private float delay;

    void OnEnable()
    {
        cryptid.nearPlayer += EnableCryptidSounds;
        cryptid.leavePlayer += DisableCryptidSounds;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        audioSource.volume = 0.25f;
        audioSource.Play();
    }

    void DisableCryptidSounds()
    {
        audioSource.clip = crickets;
        audioSource.volume = 0.003f;
        audioSource.Play();
    }
}
