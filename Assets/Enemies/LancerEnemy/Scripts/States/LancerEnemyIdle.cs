using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyIdle : LancerEnemyState
{
    private Transform player;

    public override string getStateName()
    {
        return "AOEIdle";
    }

    public override void enter()
    {
        controller.StopAttacking();
        controller.rb.velocity = Vector3.zero;

        player = controller.player.transform;

        controller.ani.Play("Idle");
    }


    public override void run()
    {
        // see if player is within detection range
        if (Vector3.Distance(player.position, controller.transform.position) < controller.detectionRadius)
        {
            // start chasing player
            controller.switchState("AOEChase");
        }
    }

}
