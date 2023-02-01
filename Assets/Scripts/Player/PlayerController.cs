using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private Transform mesh;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 8.0f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private List<TrailRenderer> dashTrails;

    // helper variables
    private CharacterController controller;
    private PlayerStateManager stateManager;

    private Transform mainCam;

    private PlayerInput input;
    private InputAction moveAction;
    private InputAction jumpAction;
    private PlayerFireControl fcs;

    private bool isGrounded = true;
    private const float COLLSION_SPEED = -0.5f;

    // expose for tracking bullets
    [HideInInspector] public Vector3 velocity;

    void Awake()
    {
        mainCam = Camera.main.transform;

        controller = GetComponent<CharacterController>();
        stateManager = GetComponent<PlayerStateManager>();
        fcs = GetComponent<PlayerFireControl>();

        // setup intpu
        input = GetComponent<PlayerInput>();
        input.actions["Jump"].started += jumpStarted;
        input.actions["Dash"].started += dashStarted;
        input.actions["Attack"].started += StartAttackCharge;
        input.actions["Attack"].canceled += ReleaseAttackCharge;
        
        moveAction = input.actions["Move"];
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void StartAttackCharge(InputAction.CallbackContext context)
    {
        fcs.StartCharging();
    }

    private void ReleaseAttackCharge(InputAction.CallbackContext context)
    {
        fcs.StopCharging();
    }

    // used for input system callback
    private void jumpStarted(InputAction.CallbackContext context)
    {
        print("JUMPED. Is grounded: " + isGrounded);
        if (isGrounded && stateManager.CanChangeState(PlayerState.Jumping))
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            stateManager.SetState(PlayerState.Jumping);
        }
    }

    private void dashStarted(InputAction.CallbackContext context)
    {
        // TODO: implement dash. Make sure to check in with player state manager
        // to see if a dash can be performed rn

        print("dashed");
    }

    private void performMovement()
    {
        // handle movement 
        Vector2 moveAxis = moveAction.ReadValue<Vector2>();
        Vector3 forwardDirection = mainCam.forward;
        forwardDirection.y = 0;
        forwardDirection.Normalize();

        Vector3 rightDirection = mainCam.right;
        rightDirection.y = 0;
        rightDirection.Normalize();

        Vector3 moveDirection = mainCam.forward * moveAxis.y + mainCam.right * moveAxis.x;
        moveDirection.y = 0;
        moveDirection.Normalize();

        // rotate mesh to face movement direction
        // TODO: lerp rotation
        if (moveDirection != Vector3.zero)
            mesh.forward = moveDirection;

        Vector3 newVelocity = moveDirection * moveSpeed * Time.deltaTime;

        if (isGrounded)
        {
            if(newVelocity.magnitude > 0.1f)
            {
                stateManager.SetState(PlayerState.Running);
            }
            else
            {
                stateManager.SetState(PlayerState.Idle);
            }
        }

        controller.Move(newVelocity);

        newVelocity.y = velocity.y;
        velocity = newVelocity;
    }

    void Update()
    {
        performMovement();

        // handle gravity
        if (isGrounded && velocity.y < 0)
        {
            // set y velocity to minimum velocity for
            // character controller to detect ground collisions
            velocity.y = COLLSION_SPEED;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // apply y velocity
        Vector3 yVelocity = new Vector3(0, velocity.y, 0);
        controller.Move(yVelocity * Time.deltaTime);

        isGrounded = controller.isGrounded;
    }

    private void OnDestroy()
    {
        input.actions["Jump"].started -= jumpStarted;
        input.actions["Dash"].started -= dashStarted;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(mesh.position, mesh.forward * 5);
    }
}
