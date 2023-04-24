using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSAnimationEvents : MonoBehaviour
{
    private Animator anim;
    
    [SerializeField] NickPlayerController playerController;
    [SerializeField] private Camera scopeCamera;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // this was a bad idea
    public void ADSFinished()
    {
        // if (!playerController.isAiming)
        //     scopeCamera.gameObject.SetActive(false);
    }
}
