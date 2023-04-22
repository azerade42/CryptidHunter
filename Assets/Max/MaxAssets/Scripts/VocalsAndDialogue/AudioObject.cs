using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Object", menuName = "AudioObject/Create Audio Object")]
public class AudioObject : ScriptableObject
{
    public AudioClip clip;
    public string[] subtitles;
    public float delayBetweenLines;
}
