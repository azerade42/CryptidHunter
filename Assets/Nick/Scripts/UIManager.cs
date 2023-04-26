using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private NickPlayerController playerController;
    [SerializeField] private Image crosshair;
    [SerializeField] private Animator healthbar;
    [SerializeField] private Sprite [] healthbarBGs;
    [SerializeField] private Image healthBarBG;
    [SerializeField] private Image fadeOutImage;

    [SerializeField] private AudioObject samWinDialogue;
    [SerializeField] private AudioObject samLoseDialogue;

    [SerializeField] private Canvas pauseMenu;

    private bool crosshairActive;
    private bool pausemenuActive;

    private void OnEnable()
    {
        // EventManager.Instance.equipRightAction += SwitchCrosshair;
        // EventManager.Instance.aimAction += SwitchCrosshair;
        EventManager.Instance.crosshairFalse += CrossHairFalse;
        EventManager.Instance.crosshairTrue += CrossHairTrue;
        EventManager.Instance.damageAction += Damaged;
        EventManager.Instance.fadeToBlack += FadeToBlack;
        EventManager.Instance.paused += PauseMenu;

    }

    private void OnDisable()
    {
        // EventManager.Instance.equipRightAction -= SwitchCrosshair;
        // EventManager.Instance.aimAction -= SwitchCrosshair;
        EventManager.Instance.crosshairTrue -= CrossHairTrue;
        EventManager.Instance.crosshairFalse -= CrossHairFalse;
        EventManager.Instance.damageAction -= Damaged;
        EventManager.Instance.fadeToBlack -= FadeToBlack;
        EventManager.Instance.paused -= PauseMenu;
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

    // could be replaced by a single variable, just ignore all this I was in too deep
    private void CrossHairTrue()
    {
        crosshairActive = true;
        crosshair.gameObject.SetActive(crosshairActive);
    }

    private void CrossHairFalse()
    {
        crosshairActive = false;
        crosshair.gameObject.SetActive(crosshairActive);
    }

    private void PauseMenu()
    {
        if (pausemenuActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            pausemenuActive = false;
            pauseMenu.gameObject.SetActive(false);
        }
        else
        {
            CrossHairFalse();
            Cursor.lockState = CursorLockMode.Confined;
            pausemenuActive = true;
            pauseMenu.gameObject.SetActive(true);
        }
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

    private void FadeToBlack(bool win)
    {
        if (win)
        {
            StartCoroutine(FadeOut(10.0f));
            Vocals.instance.Say(samWinDialogue);
        }
        else
        {
            StartCoroutine(FadeOut(5.0f));
            Vocals.instance.Say(samLoseDialogue);
        }
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
            curTime += Time.deltaTime;
            yield return null;
        }

        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadScene(0);

        yield return null;
    }

    
}
