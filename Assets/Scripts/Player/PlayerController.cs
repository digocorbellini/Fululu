using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 5.0f;
    [Tooltip("threshold value for amount of movement required to change to running state")]
    [SerializeField] private float moveThresh = 0.1f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private Transform mesh;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 8.0f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private List<TrailRenderer> dashTrails;
    [Tooltip("The layers which hurt the player")]
    [SerializeField] private LayerMask hurtLayers;

    // helper variables
    private CharacterController controller;
    private PlayerStateManager stateManager;

    private Transform mainCam;

    private PlayerInput input;
    private InputAction moveAction;
    private PlayerFireControl fcs;

    private float lastDash;
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

        // setup input
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

    // used for input system callback
    private void StartAttackCharge(InputAction.CallbackContext context)
    {
        fcs.StartCharging();
    }

    // used for input system callback
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

    // used for input system callback
    private void dashStarted(InputAction.CallbackContext context)
    {
        // TODO: implement dash. Make sure to check in with player state manager
        // to see if a dash can be performed rn
        if(stateManager.CanChangeState(PlayerState.Dashing) && DashReady())
        {
            StartCoroutine(PerformDash());
            print("dashed");
        }
    }

    private bool DashReady()
    {
        return Time.time >= lastDash + dashCooldown;
    }

    IEnumerator PerformDash()
    {
        // update player state
        stateManager.SetState(PlayerState.Dashing);

        // get dash direction
        Vector3 direction = velocity;
        direction.y = 0;
        direction.Normalize();

        if (direction.Equals(Vector3.zero))
        {
            direction = mainCam.forward;
            direction.y = 0;
            direction.Normalize();
        }

        // ignore collision with enemies
        controller.detectCollisions = false;
        Int32 tempLayerMash = hurtLayers.value;
        int playerLayer = gameObject.layer;
        for (int layerNum = 0; layerNum < 32; layerNum++)
        {
            // check to see if current layer should be ignored
            tempLayerMash >>= 1;
            int shouldIgnoreLayer = tempLayerMash & 0x1;
            
            if (shouldIgnoreLayer == 1)
            {
                // layer should be ingored
                Physics.IgnoreLayerCollision(playerLayer, layerNum + 1, true);
            }
        }

        // enable trail renderers
        foreach (TrailRenderer trail in dashTrails)
        {
            trail.emitting = true;
        }

        // perform dash in dash direction
        velocity = direction * (dashDistance / dashDuration);
        mesh.forward = direction;

        yield return new WaitForSeconds(dashDuration);

        // stop movement
        velocity = Vector3.zero;

        // disable trail renderers
        foreach (TrailRenderer trail in dashTrails)
        {
            trail.emitting = false;
        }


        // stop ignoring collision with enemies
        // ignore collision with enemies
        controller.detectCollisions = true;
        tempLayerMash = hurtLayers.value;
        for (int layerNum = 0; layerNum < 32; layerNum++)
        {
            // check to see if current layer should be ignored
            tempLayerMash >>= 1;
            int shouldIgnoreLayer = tempLayerMash & 0x1;

            if (shouldIgnoreLayer == 1)
            {
                // layer should be ingored
                Physics.IgnoreLayerCollision(playerLayer, layerNum + 1, false);
            }
        }

        lastDash = Time.time;

        // reset state
        stateManager.SetState(PlayerState.Idle);
    }

    private void performMovement()
    {
        if (!stateManager.CanChangeState(PlayerState.Running) && stateManager.currentState != PlayerState.Jumping)
        {
            return;
        }

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

        //if (isGrounded && newVelocity.magnitude > moveThresh)
        //{
        //    print("change to running state");
        //    stateManager.SetState(PlayerState.Running);
        //}

        controller.Move(newVelocity);

        newVelocity.y = velocity.y;
        velocity = newVelocity;
    }

    void Update()
    {
        performMovement();

        // handle gravity
        if (stateManager.currentState != PlayerState.Dashing)
        {
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
        }

        controller.Move(velocity * Time.deltaTime);

        // set to idle after landing from jumping
        if (!isGrounded && controller.isGrounded)
        {
            stateManager.SetState(PlayerState.Idle);
        }

        isGrounded = controller.isGrounded;

        // see if we should be idle or run
        if (isGrounded)
        {
            Vector3 moveVel = velocity;
            moveVel.y = 0;
            if (moveVel.magnitude > moveThresh)
            {
                stateManager.SetState(PlayerState.Running);
            }
            else
            {
                stateManager.SetState(PlayerState.Idle);
            }
        }
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
