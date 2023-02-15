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
    public virtual void init() {}


    public void FixedUpdate() {
        if (!isStateMachineActive) return;
        State nextState = Array.Find<State>(states, state => state.canEnter());
        if (nextState != null)
            switchState(nextState.getStateName());
        currentState.run();
        run();
    }

    private void OnReset()
    {
        GameManager.instance.OnReset -= OnReset;
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        GameManager.instance.OnReset -= OnReset;
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