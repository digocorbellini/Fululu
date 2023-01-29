using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostChase : MeleeGhostState
{
    public float giveUpDistance = 13.0f;
    public float attackRange = 1.5f;
    public float moveSpeed = 1.0f;

    private Transform player;

    public override void enter()
    {
        player = controller.player.transform;
        controller.ani.Play("Run");
    }

    public override void run()
    {
        if (Vector3.Distance(transform.position, player.position) > giveUpDistance)
        {
            // Give up the chase if player is too far
            controller.switchState("MGIdle");
        }
        else if(Vector3.Distance(transform.position, player.position) < attackRange)
        {
            // In range, try to attack
            controller.switchState("MGAttack");
        }
        else
        {
            controller.transform.LookAt(player.position);

            Vector3 rot = controller.transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            controller.transform.eulerAngles = rot;

            Vector3 vel = (player.transform.position - transform.position).normalized * moveSpeed;
            vel.y = 0;
            controller.rb.velocity = vel;
        }
    }

    public override void exit()
    {
        
    }

    public override string getStateName()
    {
        return "MGChase";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, giveUpDistance);
    }
}
