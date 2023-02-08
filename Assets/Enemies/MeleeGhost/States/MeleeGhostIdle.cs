using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostIdle : MeleeGhostState
{
    public float playerDetectionRadius = 10.0f;
    public float checkInterval = 0.1f;

    private float timer;
    public override void enter()
    {
        // Make sure to reset values such as timers, counters, etc.
        // They will presist across "runs" of a state
        timer = checkInterval;
        controller.ani.Play("Idle");
    }

    public override void run()
    {
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            if (controller.player && Vector3.Distance(transform.position, controller.player.transform.position) < playerDetectionRadius)
            {
                controller.switchState("MGChase");
            }
            else
            {
                timer = checkInterval;
            }          
        }
    }

    public override void exit()
    {
        
    }

    public override string getStateName()
    {
        return "MGIdle";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

}
