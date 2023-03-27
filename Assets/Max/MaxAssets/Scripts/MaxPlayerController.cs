using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MaxPlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform camHolder;
    private Vector2 move, look;
    public float speed, sens, maxForce, jumpForce, crouchSpeed;
    private float curSpeed, curJumpForce;
    private float lookRotation;
    public bool grounded;
    public bool nightVisToggle;
    public float batteryPercent;
    public float totalBatteryLife = 10.0f;

    public BatteryPercent batBar;
    [SerializeField] private Volume nightVisComponent;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Mesh gizmoMesh;

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
    public void OnNightVision(InputAction.CallbackContext context)
    {
        NightVision();
        NightVisionBattery();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (grounded && context.performed)
        {
            Crouch();
        }
        else if (context.canceled)
        {
            Uncrouch();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        curSpeed = speed;
        curJumpForce = jumpForce;

        Cursor.lockState = CursorLockMode.Locked;
        nightVisComponent.enabled = false;
        nightVisToggle = false;
        batteryPercent = totalBatteryLife;
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
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * look.x * sens);

        lookRotation += -(look.y) * sens;
        lookRotation = Mathf.Clamp(lookRotation, -90, 70);
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
        while(nightVisToggle == true)
        {
            if (batteryPercent >= 0.0)
            {
                batteryPercent = totalBatteryLife/Time.deltaTime;
            }
            batBar.SetBatteryPercentage(totalBatteryLife);
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
