using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class NickPlayerController : MonoBehaviour
{

    public UnityAction fireAction;
    public UnityAction aimAction;
    public UnityAction damageAction;
    public UnityAction equipRightAction;

    private Rigidbody rb;
    private Vector2 move, look;
    private float curSpeed, curJumpForce;
    private float lookRotation;
    private bool isHoldingGun, isHoldingFlashlight;
    public bool isPickingUp, isAiming;

    [Header("Player Controller")]
    public float speed;
    public float sens, maxForce, jumpForce, crouchSpeed;
    public bool grounded;

    private float health = 100f;

    public float Health
    {
        get { return health; }
    }
    [SerializeField] private float invulnerabilityTime;
    private float startHitTime, timeSinceHit;

    [SerializeField] private Transform camHolder;
    [SerializeField] public Camera scopeCamera;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Mesh gizmoMesh;
    public Animator anim;

    [SerializeField] private GameObject flashlight;

    // [Space(5)]
    // [Header("Rifle")]
    [SerializeField] private GameObject gun;

    [SerializeField] private AudioSource footSteps;

    bool footstepsEnabled;

    private float startShotTime, timeSinceShot;
    private bool isShooting;
    
    public float StartShotTime
    {
        get { return startShotTime; }
        set { startShotTime = value; }
    }

    public bool Grounded
    {
        get { return grounded; }
    }

    // public Transform debugTransform;

    // /////////////////// MAX /////////////////
    public BatteryPercent batBar;
    [SerializeField] private Volume nightVisComponent;
    public bool nightVisToggle;
    public float batteryPercent;
    public float totalBatteryLife = 10.0f;
    // ////////////////////////////////////////////

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void OnEquipLeft(InputAction.CallbackContext context)
    {
        EquipLeft();
    }

    public void OnEquipRight(InputAction.CallbackContext context)
    {
        EquipRight();
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        Pickup();
    }

    public void OnAimDownSight(InputAction.CallbackContext context)
    {
        if (!isShooting && context.started)
        {
            scopeCamera.gameObject.SetActive(true);

            isAiming = true;
            anim.SetBool("isAimingGun", true);
            
            AimDownSight();

            if (aimAction != null)
                aimAction.Invoke();
        }
        else if (!isShooting && context.canceled)
        {
            anim.SetBool("isAimingGun", false);
            isAiming = false;

            if (aimAction != null)
                aimAction.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!isShooting && context.performed && !isPickingUp)
        {
            Fire();
            isShooting = true;
        }
        else if (context.canceled)
        {
            isShooting = false;
        }       
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (grounded && context.performed)
        {
            Crouch();
        }
        else if (grounded && context.canceled)
        {
            Uncrouch();
        }
    }

    public void OnNightVision(InputAction.CallbackContext context)
    {
        NightVision();
        NightVisionBattery();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        curSpeed = speed;
        curJumpForce = jumpForce;

        timeSinceHit = Mathf.Infinity;
        timeSinceShot = Mathf.Infinity;
        gun.gameObject.SetActive(false);
        isHoldingFlashlight = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Move();
        grounded = isGrounded();
    }

    void LateUpdate()
    {
        Look();
    }

    private void Move()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity = targetVelocity.normalized;
        targetVelocity *= curSpeed;

        // rb.MovePosition(transform.position + Time.deltaTime * targetVelocity * curSpeed);

        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        float hozSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        anim.SetFloat("Speed", hozSpeed);

        if (!footstepsEnabled && grounded && hozSpeed > 0.5f)
            footSteps.enabled = true;
        else if (footstepsEnabled || !grounded || hozSpeed <= 0.5f)
            footSteps.enabled = false;
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * look.x * sens);

        lookRotation += -(look.y) * sens;
        lookRotation = Mathf.Clamp(lookRotation, -90, 50);
        camHolder.eulerAngles = new Vector3(lookRotation, camHolder.eulerAngles.y, camHolder.eulerAngles.z);

    }

    private void Jump()
    {
        Vector3 jF = Vector3.zero;

        if (grounded)
        {
            jF = Vector3.up * jumpForce;
        }

        rb.AddForce(jF, ForceMode.Impulse);
    }

    private void Crouch()
    {
        curSpeed = crouchSpeed;
        camHolder.position -= Vector3.up * 0.75f;
    }
    private void Uncrouch()
    {
        curSpeed = speed;
        camHolder.position += Vector3.up * 0.75f;
    }

    bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.5f, whatIsGround);
    }


    private void AimDownSight()
    {
        
    }

    private void Fire()
    {
        if (!isHoldingGun) return;

        timeSinceShot = Time.time - startShotTime;

        if (timeSinceShot < 1f) return;

        CameraShake.Shake(1f, 1.5f);

        if (fireAction != null)
            fireAction.Invoke();
    }

    // Cycles between flashlight and rifle
    private void EquipRight()
    {
        timeSinceShot = Time.time - startShotTime;
        if (timeSinceShot < 1f) return;

        if (equipRightAction != null)
            equipRightAction.Invoke();

        isHoldingGun = !isHoldingGun;
        isHoldingFlashlight = !isHoldingFlashlight;
        gun.SetActive(isHoldingGun);
        flashlight.SetActive(isHoldingFlashlight);
        anim.SetBool("isHoldingGun", isHoldingGun);
    }

    // Cycles between talisman and consumables
    private void EquipLeft()
    {
        
    }

    private void Pickup()
    {
        anim.SetTrigger("Pickup");
    }

    private void NightVision()
    {

        if(nightVisToggle != true)
        {
            nightVisComponent.enabled =  !nightVisComponent.enabled;
            nightVisToggle = true;
        }
        else
        {
            nightVisComponent.enabled = !nightVisComponent.enabled;
            nightVisToggle = false;
        }
    }
    public void NightVisionBattery()
    {
        if (nightVisToggle == true)
        {
            if (batteryPercent >= 0.0)
            {
                batteryPercent = totalBatteryLife/Time.deltaTime;
            }
            batBar.SetBatteryPercentage(totalBatteryLife);
        }
    }  

    public void Damage(float damageTaken)
    {
        timeSinceHit = Time.time - startHitTime;
        if (timeSinceHit < invulnerabilityTime)
        {
            return;
        }

        startHitTime = Time.time;

        health -= damageTaken;
        health = Mathf.Clamp(health, 0, 100);

        damageAction.Invoke();

        Debug.Log("OUCH!");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("dead");
    }

    // Yellow Gizmo capsule to see the player bettewr

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        CapsuleCollider cc = GetComponent<CapsuleCollider>();
        Gizmos.DrawWireMesh(gizmoMesh, -1, transform.position, Quaternion.identity,
            new Vector3(cc.radius*2, cc.height/2, cc.radius*2));
    }
    #endif
}
