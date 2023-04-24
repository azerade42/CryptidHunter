using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System;

public class Dialogue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleText = default;
    public static Dialogue instance;
    public GameObject dialogue;
    public AudioObject clip;
    private void Awake()
    {
        instance = this;
        ClearSubtitle();
    }

    public void SetSubtitle(string[] subtitles, float delay, float clipLength)
    {
        StartCoroutine(ProcessScript(subtitles, delay, clipLength));
    }


    public void ClearSubtitle()
    {
        subtitleText.text = "";
    }

    private IEnumerator ClearAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearSubtitle();
    }
    private IEnumerator ProcessScript(string[] subtitles, float delay, float clipLength)
    {
        StartCoroutine(ClearAfterSeconds(clipLength));
        int index = 0;
        foreach (string subtitle in subtitles)
        {
            subtitleText.text = subtitles[index];
            yield return new WaitForSecondsRealtime(delay);
            index++;
        }
    }
}
