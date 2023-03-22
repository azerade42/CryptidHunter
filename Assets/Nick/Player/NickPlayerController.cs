using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class NickPlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 move, look;
    private float curSpeed, curJumpForce;
    private float lookRotation;
    private bool isHoldingGun;

    [Header("Player Controller")]
    public float speed;
    public float sens, maxForce, jumpForce, crouchSpeed;
    public bool grounded;

    [SerializeField] private Transform camHolder;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Mesh gizmoMesh;
    [SerializeField] private Animator anim;

    [Space(5)]
    [Header("Rifle")]
    [SerializeField] private GameObject gun;
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem hitParticle;

    private float startShotTime, timeSinceShot;
    private bool isShooting;

    // public Transform debugTransform;

    public bool Grounded
    {
        get { return grounded; }
    }

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

    public void OnEquip(InputAction.CallbackContext context)
    {
        Equip();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!isShooting && context.performed)
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        curSpeed = speed;
        curJumpForce = jumpForce;
        timeSinceShot = Mathf.Infinity;

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

        anim.SetFloat("Speed", rb.velocity.magnitude);
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

    private void Equip()
    {
        isHoldingGun = !isHoldingGun;
        gun.SetActive(isHoldingGun);
        anim.SetBool("isHoldingGun", isHoldingGun);
    }

    private void Fire()
    {
        // maybe call event so rifle behavior isn't in player controller

        if (!isHoldingGun) return;

        timeSinceShot = Time.time - startShotTime;

        if (timeSinceShot < 1f) return;

        // Changes based on PS1 render texture
        Vector2 screenCenterPoint = new Vector2(256f/2, 224f/2);
        
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, aimColliderMask))
        {
            startShotTime = Time.time;

            // debugTransform.position = raycastHit.point;

            muzzleFlash.Play();
            anim.SetTrigger("RifleShot");

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
        }
    }

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
