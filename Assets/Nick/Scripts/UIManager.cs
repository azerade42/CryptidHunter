using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private NickPlayerController playerController;
    [SerializeField] private Image crosshair;
    private bool crosshairActive;

    private void Awake()
    {
        playerController.equipRightAction += SwitchCrosshair;
    }

    private void SwitchCrosshair()
    {
        crosshairActive = !crosshairActive;
        crosshair.gameObject.SetActive(crosshairActive);
    }


}
