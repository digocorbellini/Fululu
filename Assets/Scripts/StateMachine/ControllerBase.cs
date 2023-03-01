using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/* StateMachine adapted from ShanThatos' 
 * Extend this class to make the controller
 * state machine for your more specific enemy
 * i.e.
 * ControllerBase
 * |-GhostController
 * |-Ghost2Controller
 */
public abstract class ControllerBase : MonoBehaviour {

    public bool isStateMachineActive = true;
    public State currentState;

    public bool isCapturable = true;
    public Material outlineMaterial;
    [Range(0f, 1f)]
    public float captureCost = 1.0f;
    public Weapon captureWeapon;

    protected State[] states;

    [HideInInspector]
    public EntityHitbox hitbox;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public Animator ani;

    [HideInInspector]
    public bool isStunned = false;

    protected static AudioClip hurtSFX;
    private SkinnedMeshRenderer meshRenderer;
    private List<Material> materials = new List<Material>();
    private Material outlineReference;
    private bool isSucking;

    [RuntimeInitializeOnLoadMethodAttribute]
    static void LoadHurtSFX()
    {
        Debug.Log("Loading hurt SFX");
        hurtSFX = Resources.Load<AudioClip>("Audio/GhostHit");
    }

    public void Start() {
        GameManager.instance.OnReset += OnReset;
        states = transform.Find("States").GetComponentsInChildren<State>();
        init();
        foreach (State state in states) {
            state.setManager(this);
            state.init();
        }
        currentState?.enter();
    }
    public virtual void init()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (!outlineMaterial)
            Debug.LogError("missing outline material in enemy");
        
        // get material references
        meshRenderer.GetMaterials(materials);
        outlineReference = new Material(outlineMaterial);

        GameManager.instance.OnStartCaptureSucking += HandleStartCaptureSucking;
        GameManager.instance.OnStopCaptureSucking += HandleStopCaptureSucking;

        isSucking = false;
    }

    private void HandleStartCaptureSucking()
    {
        if (!isCapturable)
            return;

        if(!materials.Contains(outlineReference))
            materials.Add(outlineReference);
        meshRenderer.materials = materials.ToArray();
        isSucking = true;
    }

    private void HandleStopCaptureSucking()
    {
        if (!isCapturable)
            return;

        isSucking = false;
        if (materials.Contains(outlineReference))
            materials.Remove(outlineReference);
        meshRenderer.materials = materials.ToArray();
    }

    public void FixedUpdate() {
        if (!isStateMachineActive) return;
        State nextState = Array.Find<State>(states, state => state.canEnter());
        if (nextState != null)
            switchState(nextState.getStateName());
        currentState.run();
        run();

        // handle sucking outline
        if (isSucking)
        {
            print("handling sucking");
            if (GameManager.instance.player.GetChargePercent() >= captureCost)
            {
                outlineReference.SetInt("_IsCapturable", 1);
                meshRenderer.materials = materials.ToArray();
            }
            else
            {
                outlineReference.SetInt("_IsCapturable", 0);
                meshRenderer.materials = materials.ToArray();
            }
        }
    }

    private void OnReset()
    {
        GameManager.instance.OnReset -= OnReset;
        if (gameObject)
        {
            gameObject.GetComponentInChildren<EntityHitbox>().wasReset = true;
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        GameManager.instance.OnReset -= OnReset;
        GameManager.instance.OnStartCaptureSucking -= HandleStartCaptureSucking;
        GameManager.instance.OnStopCaptureSucking -= HandleStopCaptureSucking;
    }

    public virtual void run() {}

    public bool isAnimationDone(string animationName)
    {
        return ani.GetCurrentAnimatorStateInfo(0).IsName(animationName) && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public bool isCurrentStateIn(params String[] stateNames) {
        return Array.Exists(stateNames, element => element == currentState.getStateName());
    }

    public void switchState(State state) {
        currentState?.exit();
        currentState = state;
        currentState?.enter();
    }
    public void switchState(string stateName) {
        switchState(findState(stateName));
    }
    public State findState(string stateName) {
        return Array.Find<State>(states, state => state.getStateName() == stateName);
    }
}