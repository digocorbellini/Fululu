using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyDeath : LancerEnemyState
{
    public override string getStateName()
    {
        return "AOEDeath";
    }

    public override void enter()
    {
        controller.rb.velocity = Vector3.zero;
        controller.rb.angularVelocity = Vector3.zero;

        controller.ani.Play("LancerDeath_001");
    }

    public override void run()
    {
        if (controller.isAnimationDone("LancerDeath_001"))
        {
            Destroy(controller.gameObject);
        }
        
        // TODO: see if we need to have a timer
    }
}