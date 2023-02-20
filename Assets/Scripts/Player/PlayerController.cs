using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] public float moveSpeed = 5.0f;
    [Tooltip("threshold value for amount of movement required to change to running state")]
    [SerializeField] private float moveThresh = 0.1f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private Transform mesh;
    [Tooltip("In radians per second")]
    [SerializeField] private float meshRotationSpeed = 1.5f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 8.0f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private List<TrailRenderer> dashTrails;
    [SerializeField] private AudioClip dashSFX;
    [Tooltip("The layers which hurt the player")]
    [SerializeField] private LayerMask hurtLayers;

    [Header("Graze")]
    [SerializeField] private float grazeChargeTime = 20f;
    [SerializeField] private float hitChargeAmount = .1f;
    [SerializeField] private Image grazeChargeBar;
    [SerializeField] private ParticleSystem chargeBurst;
    [SerializeField] private ParticleSystem chargeGlow;
    [SerializeField] private ParticleSystem chargePlayerParticles;
    private bool alreadyCharged;
    private float currGrazeCharge = 0.0f;

    [Header("Shield")]
    [SerializeField] private GameObject shieldMesh;
    private bool isShielded;

    // helper variables
    [HideInInspector]
    public CharacterController controller;
    private PlayerStateManager stateManager;
    [HideInInspector]
    public EntityHitbox hitbox;

    private Transform mainCam;

    private PlayerInput input;
    private InputAction moveAction;
    private PlayerFireControl fcs;
    private GrazeZone graze;

    private bool isDashing = false;
    private bool isGrounded = true;
    private bool attackAni;
    private const float COLLSION_SPEED = -0.5f;

    private AudioSource audioSource;

    // expose for leading bullets
    public float GetMoveSpeed() { return moveSpeed; }
    public Vector3 GetPlayerMoveDirection()
    {
        Vector3 dir = velocity;
        dir.Normalize();

        return dir;
    }    
    private Vector3 velocity;

    void Awake()
    {
        mainCam = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        stateManager = GetComponent<PlayerStateManager>();
        fcs = GetComponent<PlayerFireControl>();
        graze = GetComponentInChildren<GrazeZone>();
        hitbox = GetComponentInChildren<EntityHitbox>();
        hitbox.OnDeath += HandleOnDeath;
        hitbox.OnShieldBreak += DisableShield;



        // setup input
        input = GetComponent<PlayerInput>();
        input.actions["Jump"].started += jumpStarted;
        input.actions["Dash"].started += dashStarted;
        input.actions["Attack"].started += StartAttackCharge;
        input.actions["Attack"].canceled += ReleaseAttackCharge;
        input.actions["AltFire"].started += HandleAltFire;
        input.actions["Pause"].started += HandlePauseInput;
        input.actions["Ultimate"].started += HandleUltimate;
        
        moveAction = input.actions["Move"];

        GameManager.instance.OnUnpause += OnUnpause;
    }
    private void Start()
    {
        GameManager.instance.LockCursor();
    }

    public void SetReticleRing(Image ring)
    {
        fcs.recticleRing = ring;
    }

    public void SetGrazeChargeBar(Image bar)
    {
        grazeChargeBar = bar;
        UpdateGrazeUI();
    }

    public void SetCaptureImage(Image cap)
    {
        fcs.captureImage = cap;
    }

    private void HandleOnDeath()
    {
        // TODO: handle player death (animations, sounds, etc)
        stateManager.SetState(PlayerState.Dead);
    }

    private void HandlePauseInput(InputAction.CallbackContext context)
    {
        GameManager.instance.TogglePause();
    }
    private void OnUnpause()
    {
        if(fcs.isCharging && !input.actions["Attack"].IsPressed())
        {
            fcs.StopCharging();
            stateManager.StartAttack();
        }
    }

    // used for input system callback
    private void StartAttackCharge(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused && stateManager.CanChangeState(PlayerState.Attacking))
        {
            fcs.StartCharging();
        }
    }

    // used for input system callback
    private void ReleaseAttackCharge(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused && fcs.isCharging && stateManager.CanChangeState(PlayerState.Attacking))
        {
            fcs.StopCharging();
            stateManager.StartAttack();
        }
    }

    // used for input system callback
    private void HandleAltFire(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused)
        {
            if (IsGrazeCharged())
            {
                // TODO: Perform gourd swipe to capture enemies
                if (fcs.CaptureAttack())
                {
                    UseCharge();
                }
            }
        }
    }

    private void HandleUltimate(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused)
        {
            // Sacrifice weapon if equipped
            if (fcs.weapon != fcs.defaultWeapon)
            {
                fcs.SacrificeWeapon();
            }
        }
    }

    // used for input system callback
    private void jumpStarted(InputAction.CallbackContext context)
    {
        print("JUMPED. Is grounded: " + isGrounded);
        if (!GameManager.isPaused && isGrounded && stateManager.CanChangeState(PlayerState.Jumping))
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
        if(!GameManager.isPaused && stateManager.CanChangeState(PlayerState.Dashing) && DashReady())
        {
            isDashing = true;
            StartCoroutine(PerformDash());
            if (fcs.isCharging)
            {
                fcs.CancelCharge();
            }
            print("dashed");
        }
    }

    public float ChargeGraze(float amount)
    {
        if (!isShielded)
        {
            // Disable graze charge when shielded
            currGrazeCharge += amount;
            UpdateGrazeUI();
        }

        return currGrazeCharge / grazeChargeTime;
    }

    public void HitGrazeCharge(float mult)
    {
        currGrazeCharge += hitChargeAmount * mult;
        UpdateGrazeUI();
    }

    public void UseCharge()
    {
        chargeGlow.enableEmission = false;
        alreadyCharged = false;
        currGrazeCharge = 0.0f;
        graze.Reset();
        UpdateGrazeUI();
    }

    public bool IsGrazeCharged()
    {
        return currGrazeCharge >= grazeChargeTime;
    }

    private void UpdateGrazeUI()
    {
        float fillAmount = currGrazeCharge / grazeChargeTime;

        if(fillAmount >= 1 && !alreadyCharged)
        {
            alreadyCharged = true;
            chargeGlow.enableEmission = true;
            chargeBurst.Play();
            chargePlayerParticles.Play();
        }
        grazeChargeBar.fillAmount = fillAmount;
    }

    public void SetChargeParticles(ParticleSystem burst, ParticleSystem glow)
    {
        chargeBurst = burst;
        chargeGlow = glow;
        chargeGlow.enableEmission = false;
    }


    private bool DashReady()
    {
        return !isDashing;
    }

    IEnumerator PerformDash()
    {
        audioSource.PlayOneShot(dashSFX);

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

        // stop movement
        velocity = Vector3.zero;

        // disable trail renderers
        foreach (TrailRenderer trail in dashTrails)
        {
            trail.emitting = false;
        }

        isDashing = false;

        // reset state
        stateManager.SetState(PlayerState.Idle);
    }

    public void Revive()
    {
        stateManager.Revive();
        alreadyCharged = false;
        graze.Reset();
        hitbox.health = hitbox.maxHealth;
        hitbox.alreadyDead = false;
        currGrazeCharge = 0.0f;
        fcs.SwitchWeapon(fcs.defaultWeapon);
        fcs.CancelCharge();
        isDashing = false;
    }

    private void performMovement()
    {
        if (!stateManager.CanChangeState(PlayerState.Running) && stateManager.currentState != PlayerState.Jumping)
        {
            return;
        }

        mainCam = Camera.main.transform;

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
        Vector3 direction = mesh.forward;
        if (input.actions["Attack"].IsPressed() || input.actions["AltFire"].IsPressed())
        {
            // TODO: probably want to replace this since this is so choppy, maybe only upper body?
            Vector3 end = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
            direction = Vector3.RotateTowards(mesh.forward, end, meshRotationSpeed * Time.deltaTime, 0.0f);
        } else if (moveDirection != Vector3.zero)
        {
            direction = Vector3.RotateTowards(mesh.forward, moveDirection, meshRotationSpeed * Time.deltaTime, 0.0f);
        }

        mesh.forward = direction;

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

    public void EnableShield()
    {
        shieldMesh.SetActive(true);
        isShielded = true;
    }

    private void DisableShield(bool wasTimeout)
    {
        if (!wasTimeout)
        {
            // Play some sort of shield break effect here
        }

        shieldMesh.SetActive(false);
        isShielded = false;
    }

    public bool isAnimationDone(string animationName)
    {
        return stateManager.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && stateManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    private void OnDestroy()
    {
        input.actions["Jump"].started -= jumpStarted;
        input.actions["Dash"].started -= dashStarted;
        input.actions["Attack"].started -= StartAttackCharge;
        input.actions["Attack"].canceled -= ReleaseAttackCharge;
        input.actions["AltFire"].started -= HandleAltFire;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(mesh.position, mesh.forward * 5);
    }
}
