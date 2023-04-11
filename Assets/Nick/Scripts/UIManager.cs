using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private NickPlayerController playerController;
    [SerializeField] private Image crosshair;
    [SerializeField] private Animator healthbar;
    [SerializeField] private Sprite [] healthbarBGs;
    [SerializeField] private Image healthBarBG;
    private bool crosshairActive;

    private void OnEnable()
    {
        playerController.equipRightAction += SwitchCrosshair;
        playerController.aimAction += SwitchCrosshair;
        playerController.damageAction += Damaged;
    }

    private void Awake()
    {
        healthbar.SetFloat("Health", playerController.Health);
    }

    private void SwitchCrosshair()
    {
        crosshairActive = !crosshairActive;
        crosshair.gameObject.SetActive(crosshairActive);
    }

    private void Damaged()
    {
        float health = playerController.Health;
        healthbar.SetFloat("Health", health);
        
        if (health > 66)
            healthBarBG.sprite = healthbarBGs[0];
        else if (health > 33)
            healthBarBG.sprite = healthbarBGs[1];
        else
            healthBarBG.sprite = healthbarBGs[2];
    }

    private void OnDisable()
    {
        playerController.equipRightAction -= SwitchCrosshair;
        playerController.aimAction -= SwitchCrosshair;
    }
}
