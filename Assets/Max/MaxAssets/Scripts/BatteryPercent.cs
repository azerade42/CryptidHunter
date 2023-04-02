using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryPercent : MonoBehaviour
{
    public Slider slider;
    public GameObject batteryIcon;

    void Start()
    {
        batteryIcon.SetActive(false);
    }

    public void SetBatteryPercentage(float batPercent)
    {
        slider.value = batPercent;
    }

    public void BatteryVisible(bool batVis)
    {
        if(batVis == true)
        {
            batteryIcon.SetActive(true);
        }
        else
        {
            batteryIcon.SetActive(false);
        }
    }
}
