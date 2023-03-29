using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAudio : MonoBehaviour
{
    [SerializeField] private AudioSource crickets;
    [SerializeField] private AudioSource distantCryptid;

    [SerializeField] private Enemy cryptid;

    private bool soundsDisabled;
    private float timeSinceLastPlay;
    private float lastPitch = 1f;
    private float delay;

    void OnEnable()
    {
        cryptid.nearPlayer += DisableEnvironmentAudio;
        cryptid.leavePlayer += EnableEnvironmentAudio;
    }

    void Update()
    {
        if (!soundsDisabled && timeSinceLastPlay > delay)
        {
            timeSinceLastPlay = 0;
            PlayCryptidSound(distantCryptid, 0.15f, distantCryptid.clip.length);
        }
        timeSinceLastPlay += Time.deltaTime;
    }

    private void PlayCryptidSound(AudioSource sound, float pitchRange, float delay)
    {
        float loPitch = Mathf.Max(sound.pitch - pitchRange, 0);
        float hiPitch = Mathf.Min(sound.pitch + pitchRange, 1);

        lastPitch = Random.Range(loPitch, hiPitch);
        sound.pitch = lastPitch;
        delay = sound.clip.length * 1/lastPitch;
        sound.Play();
    }

    void DisableEnvironmentAudio()
    {
        soundsDisabled = true;
        crickets.enabled = false;
        distantCryptid.enabled = false;
    }

    void EnableEnvironmentAudio()
    {
        soundsDisabled = false;
        crickets.enabled = true;
        distantCryptid.enabled = true;
        crickets.Play();
    }
}
