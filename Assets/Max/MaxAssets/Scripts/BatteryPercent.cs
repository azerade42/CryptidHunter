using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryPercent : MonoBehaviour
{
    public Slider slider;

    public void SetBatteryPercentage(float batPercent)
    {
        slider.value = batPercent;
    }
}
