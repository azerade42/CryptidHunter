using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image crosshair;
    private bool crosshairActive;

    private void OnEnable()
    {
        playerController.equipRightAction += SwitchCrosshair;
    }

    private void SwitchCrosshair()
    {
        crosshairActive = !crosshairActive;
        crosshair.gameObject.SetActive(crosshairActive);
    }

    private void OnDisable()
    {
        playerController.equipRightAction -= SwitchCrosshair;
    }


}
