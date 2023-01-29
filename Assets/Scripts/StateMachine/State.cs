using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* You should create an another more specific state base class
 * that the rest of your state extend
 * i.e.
 * State
 * |- GhostState
 *    |- GhostIdleState
 *    |- GhostChaseState
 */
 
public abstract class State : MonoBehaviour {
    public void Start() {}

    public virtual void init() {}

    public virtual bool canEnter() { return false; }
    public virtual void enter() {}
    public virtual void exit() {}
    public virtual void run() {}

    public abstract string getStateName();


    protected BaseController manager;
    public void setManager(BaseController manager) {
        this.manager = manager;
    }
}