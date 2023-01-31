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
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void jumpStarted(InputAction.CallbackContext context)
    {
        print("JUMPED");
    }

    void Update()
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

        // TODO: lerp rotation
        mesh.forward = moveDirection;

        velocity = moveDirection * moveSpeed * Time.deltaTime;

        // handle gravity
        velocity.y += gravity * Time.deltaTime;

        // apply movement
        controller.Move(velocity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(mesh.position, mesh.forward * 5);
    }
}
