using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostStun : MeleeGhostState
{
    public float stunTime = 3.0f;
    public GameObject stunIndicator;

    private float timer;
    public override void enter()
    {
        timer = stunTime;
        controller.isStunned = true;
        stunIndicator.SetActive(true);
    }

    public override void run()
    {
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            controller.switchState("MGIdle");      
        }
    }

    public override void exit()
    {
        controller.isStunned = false;
        stunIndicator.SetActive(false);
    }

    public override string getStateName()
    {
        return "MGStun";
    }
}
