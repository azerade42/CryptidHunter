using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    
    [SerializeField] private NickPlayerController playerController;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private AudioSource gunshotNoise;

    void OnEnable()
    {
        EventManager.Instance.fireAction += FireRifle;
    }

    void OnDisable()
    {
        EventManager.Instance.fireAction -= FireRifle;
    }

    void FireRifle()
    {
        playerController.StartShotTime = Time.time;
        muzzleFlash.Play();
        gunshotNoise.Play();

        Camera currentCam;
        float resolutionX, resolutionY;

        if (playerController.isAiming)
        {
            currentCam = playerController.scopeCamera;
            resolutionX = resolutionY = 216f;
        }
        else
        {
            currentCam = Camera.main;
            resolutionX = 384f;
            resolutionY = 216f;
        }

        // Changes based on PS1 render texture & scope render texture
        Vector2 screenCenterPoint = new Vector2(resolutionX/2, resolutionY/2);
        
        Ray ray = currentCam.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, aimColliderMask))
        {
            playerController.anim.SetTrigger("RifleShot");

            GameObject whatIsHit = raycastHit.transform.gameObject;

            if (whatIsHit.CompareTag("Weakpoint"))
            {
                Debug.Log("HIT TARGET");

                //bool died = whatIsHit.GetComponent<Enemy>().ChangeHealth(-1);
                if (EventManager.Instance.rifleHit != null)
                    EventManager.Instance.rifleHit.Invoke();

                 ObjectPooler.Instance.SpawnFromPool("Explosion", raycastHit.point, Quaternion.identity, whatIsHit.transform);

                 whatIsHit.GetComponent<MeshRenderer>().enabled = false;
                 whatIsHit.GetComponent<SphereCollider>().enabled = false;

                 // Instantiate blue explosion or something

                // if (!died)
                // {
                //     ParticleSystem explosion = Instantiate(hitParticle, raycastHit.point, Quaternion.identity, whatIsHit.transform);
                //     Destroy(explosion.gameObject, 1f);
                // }
            }
            else
            {
                Debug.Log("HIT OBJECT");
                // ParticleSystem explosion = Instantiate(hitParticle, raycastHit.point, Quaternion.identity);
                ObjectPooler.Instance.SpawnFromPool("Explosion", raycastHit.point, Quaternion.identity, ObjectPooler.Instance.gameObject.transform);
                // Destroy(explosion.gameObject, 1f);
            }
        }
    }
}
