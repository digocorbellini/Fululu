using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] public float moveSpeed = 5.0f;
    [SerializeField] public float chargingMoveModifier = 1.0f;
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

    [Header("Charge")]
    [SerializeField] private float maxCharge = 20f;
    [SerializeField] private float capturingMoveModifier = .6f;
    [SerializeField] private float hitChargeAmount = .1f;
    [SerializeField] private ChargeEffects chargeEffects;
    private bool isCapturing;
    public float currCharge = 0.0f;
    [SerializeField] private float startingCharge = 4.0f;

    [Header("Shield")]
    [SerializeField] private GameObject shieldMesh;
    private bool isShielded;

    [Header("Camera")]
    public CinemachineFreeLook cinemachine;
    public float hurtCameraShakeAmplitude = 4f;
    public float hurtCameraShakeFrequency = 2f;
    public float hurtCameraShakeDuration = 0.2f;

    [Header("Effects")]
    public ParticleSystem hurtEffects;
    public float hurtScreenTintAlpha = 0.1f;
    public float deathScreenTintDuration = 0.15f;

    // helper variables
    [HideInInspector]
    public CharacterController controller;
    private PlayerStateManager stateManager;
    private Animator anim;
    [HideInInspector]
    public EntityHitbox hitbox;

    private Transform mainCam;

    private PlayerInput input;
    private InputAction moveAction;
    private PlayerFireControl fcs;
    private GrazeZone graze;

    private bool isDashing = false;
    private bool isDashReset = true;
    private float dashTimer = 0.0f;
    private bool isGrounded = true;
    private bool attackAni;
    private float moveSpeedMod;
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
        if (stateManager)
        {
            anim = stateManager.animator;
        }
        fcs = GetComponent<PlayerFireControl>();
        graze = GetComponentInChildren<GrazeZone>();
        chargeEffects = GetComponent<ChargeEffects>();
        hitbox = GetComponentInChildren<EntityHitbox>();
        hitbox.OnDeath += HandleOnDeath;
        hitbox.OnShieldBreak += DisableShield;
        hitbox.OnHurt += HandleOnHurt;



        // setup input
        input = GetComponent<PlayerInput>();
        input.actions["Jump"].started += jumpStarted;
        input.actions["Dash"].started += dashStarted;
        input.actions["Attack"].started += StartAttackCharge;
        input.actions["Attack"].canceled += ReleaseAttackCharge;
        input.actions["AltFire"].started += HandleAltHold;
        input.actions["AltFire"].canceled += HandleAltFire;
        input.actions["Pause"].started += HandlePauseInput;
        input.actions["Ultimate"].started += HandleUltimate;
        
        moveAction = input.actions["Move"];

        GameManager.instance.OnUnpause += OnUnpause;
    }
    private void Start()
    {
        currCharge = startingCharge;
        UpdateGrazeUI();

        GameManager.instance.LockCursor();
    }

    public void SetReticleRing(Image ring)
    {
        fcs.recticleRing = ring;
    }

    public void SetGrazeChargeBar(Image bar)
    {
        chargeEffects.InitBar(bar);
        UpdateGrazeUI();
    }

    private void HandleOnHurt(float damage, bool isExplosive, Collider other)
    {
        // Determine if other is in front/behind of mesh direction
        // Probably a more efficient way to do this. I'm rusty on linear algebra :(
        Vector3 offset = other.transform.position - transform.position;
        Vector3 projection = Vector3.Project(offset, mesh.forward);
        Vector3 scale = new Vector3(projection.x / mesh.forward.x, projection.y / mesh.forward.y, projection.z / mesh.forward.z);
        bool inFront = true;
        if (scale.x < 0 || scale.y < 0 || scale.z < 0)
        {
            inFront = false;
        }

        stateManager.PlayDamageAnim(inFront);
        ShakeCamera(hurtCameraShakeAmplitude, hurtCameraShakeFrequency, hurtCameraShakeDuration);
        hurtEffects.Stop();
        hurtEffects.Play();
        UIManager.instance.TintScreen(Color.red, hurtScreenTintAlpha, hitbox.iFrameTime);
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        StartCoroutine(CameraShakeRoutine(amplitude, frequency, duration));
    }

    private IEnumerator CameraShakeRoutine(float amplitude, float frequency, float duration)
    {
        for (int i = 0; i < 3; i++)
        {
            CinemachineBasicMultiChannelPerlin noise = cinemachine.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise)
            {
                noise.m_AmplitudeGain = amplitude;
                noise.m_FrequencyGain = frequency;
            }
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < 3; i++)
        {
            CinemachineBasicMultiChannelPerlin noise = cinemachine.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise)
            {
                noise.m_AmplitudeGain = 0;
            }
        }
    }

    private void HandleOnDeath()
    {
        // TODO: handle player death (animations, sounds, etc)
        StopCapturing();
        stateManager.SetState(PlayerState.Dead);
        anim.SetBool("IsDead", true);
        ShakeCamera(hurtCameraShakeAmplitude, hurtCameraShakeFrequency, hurtCameraShakeDuration);
        hurtEffects.Stop();
        hurtEffects.Play();
        UIManager.instance.TintScreen(Color.red, hurtScreenTintAlpha, deathScreenTintDuration);
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
            moveSpeedMod = MathF.Min(chargingMoveModifier, moveSpeedMod);

            StopCapturing();
        }
    }

    // used for input system callback
    private void ReleaseAttackCharge(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused && fcs.isCharging && stateManager.CanChangeState(PlayerState.Attacking))
        {
            fcs.StopCharging();
            stateManager.StartAttack();
            moveSpeedMod = 1.0f;

            StopCapturing();
        }
    }

    private void HandleAltHold(InputAction.CallbackContext context)
    {
        // cancel attack charge if it is charging
        if (fcs.isCharging)
        {
            fcs.StopCharging(false);
        }

        if (!GameManager.isPaused && stateManager.CanChangeState(PlayerState.Attacking))
        {
            fcs.ShowCaptureZone(true);
            isCapturing = true;
            moveSpeedMod = MathF.Min(moveSpeedMod, capturingMoveModifier);
        }
        
    }

    // used for input system callback
    private void HandleAltFire(InputAction.CallbackContext context)
    {
        StopCapturing();
    }

    private void StopCapturing()
    {
        if (isCapturing)
        {
            fcs.ShowCaptureZone(false);
            isCapturing = false;
            moveSpeedMod = 1.0f;
        }
    }

    public bool DoCapture(ControllerBase capturedEntity)
    {
        if (isCapturing)
        {
            fcs.OnCapture(capturedEntity);
            UseCharge(capturedEntity.captureCost);
            hitbox.GiveIFrames(1.5f);
            StopCapturing();
            return true;
        }

        return false;
    }

    private void HandleUltimate(InputAction.CallbackContext context)
    {
        if (!GameManager.isPaused && stateManager.CanChangeState(PlayerState.Attacking))
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
            isDashReset = false;
            StartCoroutine(PerformDash());
            if (fcs.isCharging)
            {
                fcs.CancelCharge();
            }
            isCapturing = false;
            fcs.ShowCaptureZone(false);
            moveSpeedMod = 1.0f;
            print("dashed");
        }
    }

    public float ChargeGraze(float amount)
    {
        if (!isShielded)
        {
            // Disable graze charge when shielded
            currCharge += amount;
            UpdateGrazeUI();
        }

        return currCharge / maxCharge;
    }

    public void OnHitCharge(float mult)
    {
        currCharge += hitChargeAmount * mult;
        UpdateGrazeUI();
    }

    public bool UseCharge(float amt = 1.0f)
    {
        float chargePercent = Math.Min(1, currCharge / maxCharge);
        if(amt > chargePercent)
        {
            return false;
        }
        currCharge = Math.Min(maxCharge, currCharge);
        currCharge -= (maxCharge * amt);

        UpdateGrazeUI();
        chargeEffects.OnChargeConsumed(currCharge / maxCharge);

        return true;
    }

    public float GetChargePercent()
    {
        return currCharge / maxCharge;
    }

    private void UpdateGrazeUI()
    {
        float fillAmount = currCharge / maxCharge;
        chargeEffects.UpdateGrazeUI(fillAmount);
    }

    public void SetChargeParticles(ParticleSystem burst, ParticleSystem glow)
    {
        chargeEffects.InitUIParticles(burst, glow);
    }


    private bool DashReady()
    {
        return !isDashing && isDashReset && Time.time >= dashTimer + dashCooldown;
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

        dashTimer = Time.time;
    }

    public void Revive()
    {
        stateManager.Revive();
        DisableShield(false);
        //graze.Reset();
        hitbox.health = hitbox.maxHealth;
        hitbox.alreadyDead = false;
        fcs.SwitchWeapon(fcs.defaultWeapon);
        fcs.CancelCharge();
        isDashing = false;
        anim.SetBool("IsDead", false);
        currCharge = startingCharge;
        UpdateGrazeUI();
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

        Vector3 moveDirection = mainCam.forward * moveAxis.y + mainCam.right * moveAxis.x;
        moveDirection.y = 0;
        moveDirection.Normalize();

        // rotate mesh to face movement direction
        Vector3 direction = mesh.forward;
        if (input.actions["Attack"].IsPressed() || input.actions["AltFire"].IsPressed() || stateManager.IsAttackAnim)
        {
            // TODO: probably want to replace this since this is so choppy, maybe only upper body?
            Vector3 end = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
            direction = Vector3.RotateTowards(mesh.forward, end, meshRotationSpeed * Time.deltaTime, 0.0f);
        } else if (moveDirection != Vector3.zero)
        {
            direction = Vector3.RotateTowards(mesh.forward, moveDirection, meshRotationSpeed * Time.deltaTime, 0.0f);
        }

        mesh.forward = direction;

        Vector3 newVelocity = moveDirection * moveSpeed * moveSpeedMod * Time.deltaTime;

        float forwardSpeed = Vector3.Dot(moveDirection, mesh.forward) / (mesh.forward.sqrMagnitude);
        float rightSpeed = Vector3.Dot(moveDirection, mesh.right) / (mesh.right.sqrMagnitude);

        anim.SetFloat("Forward", forwardSpeed);
        anim.SetFloat("Right", rightSpeed);

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
        if (stateManager.currentState != PlayerState.Dashing && !isGrounded && controller.isGrounded)
        {
            stateManager.SetState(PlayerState.Idle);
        }

        isGrounded = controller.isGrounded;
        anim.SetBool("IsGrounded", isGrounded);

        if (isGrounded)
        {
            // reset dash
            isDashReset = true;

            // see if we should be idle or run
            Vector3 moveVel = velocity;
            moveVel.y = 0;
            if (moveVel.magnitude > moveThresh)
            {
                stateManager.SetState(PlayerState.Running);
                anim.SetBool("IsMoving", true);
            }
            else
            {
                stateManager.SetState(PlayerState.Idle);
                anim.SetBool("IsMoving", false);
            }
        }

        anim.SetFloat("YVelocity", velocity.y);
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
