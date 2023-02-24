using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyDeath : LancerEnemyState
{
    public AudioClip deathSFX;

    public override string getStateName()
    {
        return "AOEDeath";
    }

    public override void enter()
    {
        controller.rb.velocity = Vector3.zero;
        controller.rb.angularVelocity = Vector3.zero;

        controller?.audioSource.PlayOneShot(deathSFX);
    }

    public override void run()
    {
        // only destroy lancer after death animation is done playing
        if (controller.isAnimationDone("Dead"))
        {
            Destroy(controller.gameObject);
        }
    }
}
