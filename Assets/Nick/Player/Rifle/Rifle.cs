using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [SerializeField] private NickPlayerController playerController;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    
    [SerializeField] private ParticleSystem muzzleFlash;
    
    [SerializeField] private ParticleSystem hitParticle;

    void Awake()
    {
        playerController.fireAction += FireRifle;
    }

    void FireRifle()
    {
        // Changes based on PS1 render texture
        Vector2 screenCenterPoint = new Vector2(256f/2, 224f/2);
        
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, aimColliderMask))
        {
            playerController.StartShotTime = Time.time;

            // debugTransform.position = raycastHit.point;

            muzzleFlash.Play();
            playerController.anim.SetTrigger("RifleShot");

            GameObject whatIsHit = raycastHit.transform.gameObject;

            if (whatIsHit.CompareTag("Enemy"))
            {
                Debug.Log("HIT TARGET");

                bool died = whatIsHit.GetComponent<Enemy>()!.ChangeHealth(-1);

                if (!died)
                {
                    ParticleSystem explosion = Instantiate(hitParticle, raycastHit.point, Quaternion.identity);
                    Destroy(explosion.gameObject, 1f);
                }
            }
            else
            {
                Debug.Log("HIT OBJECT");
                ParticleSystem explosion = Instantiate(hitParticle, raycastHit.point, Quaternion.identity);
                Destroy(explosion.gameObject, 1f);
            }
        }
    }
}
