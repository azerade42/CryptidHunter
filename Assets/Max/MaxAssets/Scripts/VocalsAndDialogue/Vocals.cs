using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vocals : MonoBehaviour
{
    private AudioSource source;
    public static Vocals instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void Say(AudioObject clip)
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.PlayOneShot(clip.clip);
        Dialogue.instance.SetSubtitle(clip.subtitles, clip.delayBetweenLines, clip.clip.length);
    }
}
