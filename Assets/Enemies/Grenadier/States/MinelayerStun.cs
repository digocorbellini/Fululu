using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinelayerStun : MinelayerState
{
    public float stunTime = 3.0f;
    public GameObject stunIndicator;

    private float timer;
    public override void enter()
    {
        timer = stunTime;
        controller.isStunned = true;
        controller.ani.Play("Stun");
    }

    public override void run()
    {
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            //controller.switchState("MLWander");      

            timer = stunTime;
        }
    }

    public override void exit()
    {
        controller.isStunned = false;
        stunIndicator.SetActive(false);
        controller.ani.Play("Idle");
    }

    public override string getStateName()
    {
        return "MLStun";
    }
}
