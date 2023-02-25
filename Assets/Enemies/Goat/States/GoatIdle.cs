using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatIdle : GoatState
{
    public float idleTime = 3.0f;
    public float range = 35.0f;

    private float timer;
    public override void enter()
    {
        timer = idleTime;
    }

    public override void run()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            if(controller.player && Vector3.Distance(transform.position, controller.player.transform.position) <= range)
            {
                controller.switchState("GoatReady");
            }
            else
            {
                timer = idleTime;
            }
        }
    }

    public override void exit()
    {
        
    }

    public override string getStateName()
    {
        return "GoatIdle";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
