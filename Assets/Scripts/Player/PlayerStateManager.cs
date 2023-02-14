using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Dashing,
    Jumping,
    Running,
    DamageFront,
    DamageBack,
    Attacking,
    Dead
}


public class PlayerStateManager : MonoBehaviour
{
    // show in editor for debugging
    [SerializeField] private PlayerState _currentState;
    [SerializeField] public Animator animator;

    private bool IsAttackAnim;

    public PlayerState currentState { 
        get
        {
            return _currentState;
        }
        private set
        {
            _currentState = value;

            if (_currentState == PlayerState.Dead)
            {
                animator.Play("Death");
                return;
            }

            if (IsAttackAnim)
            {
                return;
            }

            switch (_currentState)
            {
                case PlayerState.Idle:
                    animator.Play("Idle");
                    break;
                case PlayerState.Running:
                    animator.Play("Walk");
                    break;
                case PlayerState.Jumping:
                    animator.Play("Jump");
                    break;
                case PlayerState.Dead:
                    animator.Play("Death");
                    break;
                default:
                    animator.Play("Idle");
                    break;
            }
        }
    }

    [SerializeField] private PlayerState startingState = PlayerState.Idle;

    private void Awake()
    {
        currentState = startingState;
    }

    /// <summary>
    /// Set the current player state to the given state. Will only
    /// occur if it is possible to change to the given state given
    /// the current state.
    /// </summary>
    /// <param name="newState">The new player state</param>
    /// <returns>Returns true if the state was succesfully changed
    /// and false otherwise</returns>
    public bool SetState(PlayerState newState)
    {
        if (!CanChangeState(newState))
        {
            
            return false;
        }

        currentState = newState;
        return false;
    }

    // Throwaway method for play test. Will probably remove later
    public void Revive()
    {
        currentState = PlayerState.Idle;
    }

    public void StartAttack()
    {
        if (currentState != PlayerState.Dead)
        {
            animator.Play("Attack");
            IsAttackAnim = true;
        }
    }

    private void Update()
    {
        if (IsAttackAnim)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                IsAttackAnim = false;
            }
        }
    }

    public bool CanChangeState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Idle:
                if (currentState != PlayerState.Dead)
                    return true;
                return false;
                break;
            case PlayerState.Dashing:
                if (currentState != PlayerState.Dead && currentState != PlayerState.DamageBack
                    && currentState != PlayerState.DamageFront && currentState != PlayerState.Dashing)
                    return true;
                return false;
                break;
            case PlayerState.Jumping:
                if (currentState != PlayerState.Dead && currentState != PlayerState.Dashing && currentState != PlayerState.Jumping
                    && currentState != PlayerState.DamageBack && currentState != PlayerState.DamageFront)
                {
                    return true;
                }
                return false;
                break;
            case PlayerState.Running:
                if (currentState != PlayerState.Dead && currentState != PlayerState.Dashing
                    && currentState != PlayerState.DamageBack && currentState != PlayerState.DamageFront
                    && currentState != PlayerState.Jumping)
                {
                    return true;
                }
                return false;
                break;
            case PlayerState.DamageFront:
                if (currentState != PlayerState.Dead && currentState != PlayerState.DamageBack)
                {
                    return true;
                }
                return false;
                break;
            case PlayerState.DamageBack:
                if (currentState != PlayerState.Dead && currentState != PlayerState.DamageFront)
                {
                    return true;
                }
                return false;
                break;
            case PlayerState.Attacking:
                if (currentState != PlayerState.Dead && currentState != PlayerState.DamageBack
                    && currentState != PlayerState.DamageFront && currentState != PlayerState.Dashing)
                    return true;
                return false;
                break;
            case PlayerState.Dead:
                return true;
                break;
            default:
                return false;
                break;
        }


        return true;
    }
}
