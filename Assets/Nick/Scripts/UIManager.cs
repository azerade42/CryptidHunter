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
    [SerializeField] private Image fadeOutImage;
    private bool crosshairActive;

    private void OnEnable()
    {
        EventManager.Instance.equipRightAction += SwitchCrosshair;
        EventManager.Instance.aimAction += SwitchCrosshair;
        EventManager.Instance.damageAction += Damaged;
        EventManager.Instance.fadeToBlack += FadeToBlack;

    }

    private void OnDisable()
    {
        EventManager.Instance.equipRightAction -= SwitchCrosshair;
        EventManager.Instance.aimAction -= SwitchCrosshair;
        EventManager.Instance.damageAction -= Damaged;
        EventManager.Instance.fadeToBlack -= FadeToBlack;
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

    private void FadeToBlack()
    {
        // Time.timeScale = 0;

        StartCoroutine(FadeOut(5.0f));

        Application.Quit();
    }

    IEnumerator FadeOut(float fadeTime)
    {
        float curTime = 0;
        while (curTime < fadeTime)
        {
            float lerp = Mathf.Lerp(0, 1, curTime/fadeTime);
            Color imgColor = fadeOutImage.color;
            imgColor.a = lerp;
            fadeOutImage.color = imgColor;
            // equippedObj.GetComponent<MeshRenderer>().material.SetFloat("_DissolveHeight", lerp);
            curTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    
}
