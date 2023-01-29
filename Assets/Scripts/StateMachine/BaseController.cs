using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/* StateMachine adapted from ShanThatos' 
 * Extend this class to make the controller
 * state machine for your more specific enemy
 * i.e.
 * StateManager
 * |-GhostController
 * |-Ghost2Controller
 */
public class BaseController : MonoBehaviour {

    public bool isStateMachineActive = true;
    public State currentState;

    protected State[] states;

    [HideInInspector]
    public EntityHitbox hitbox;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public Animator ani;

    [HideInInspector]
    public bool isStunned = false;
    
    public void Start() {
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