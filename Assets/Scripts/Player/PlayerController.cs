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

    private bool isGrounded;

    // expose for tracking bullets
    public Vector3 velocity;

    void Awake()
    {
        mainCam = Camera.main.transform;

        controller = GetComponent<CharacterController>();
        stateManager = GetComponent<PlayerStateManager>();

        // setup intpu
        input = GetComponent<PlayerInput>();
        input.actions["Jump"].started += jumpStarted;
        moveAction = input.actions["Move"];
        //jumpAction = input.actions["Jump"];
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // used for input system callback
    private void jumpStarted(InputAction.CallbackContext context)
    {
        print("JUMPED. Is grounded: " + controller.isGrounded);
        if (controller.isGrounded && stateManager.CanChangeState(PlayerState.Jumping))
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);

            //controller.Move(velocity);
        }
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

        controller.Move(newVelocity);

        newVelocity.y = velocity.y;
        velocity = newVelocity;
    }

    void Update()
    {
        performMovement();

        // handle gravity
        if(!controller.isGrounded)
            velocity.y += gravity * Time.deltaTime;

        // ensure we stop falling once we are grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        // y velocity
        Vector3 yVelocity = new Vector3(0, velocity.y, 0);
        controller.Move(yVelocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(mesh.position, mesh.forward * 5);
    }
}
