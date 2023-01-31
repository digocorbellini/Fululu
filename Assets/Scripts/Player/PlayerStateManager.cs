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
    public PlayerState currentState { get; private set; }

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
        if (CanChangeState(newState))
        {
            currentState = newState;
            return true;
        }

        return false;
    }

    public bool CanChangeState(PlayerState newState)
    {
        return true;
    }
}
