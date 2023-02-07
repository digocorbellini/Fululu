using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGhostStun : ProjectileGhostState
{
    public float StunDuration;
    public GameObject stun;
    // TODO: add stun object 

    private float timer;

    public override string getStateName()
    {
        return "PGStun";
    }

    public override void enter()
    {
        timer = StunDuration;
        controller.isStunned = true;
        controller.rb.velocity = Vector3.zero;
        stun.SetActive(true);
        controller.ani.Play("Stun");
    }

    public override void run()
    {
        if (timer <= 0)
        {
            controller.isStunned = false;
            stun.SetActive(false);

            // decide which state to change to
            if (Vector3.Distance(controller.transform.position, controller.player.transform.position)
                > controller.MaxDistanceFromPlayer)
            {
                controller.switchState("PGChase");
            }
            else
            {
                controller.switchState("PGEncircle");
            }
        }

        timer -= Time.deltaTime;
    }

    public override void exit()
    {
        controller.ani.Play("Idle");
    }
}
