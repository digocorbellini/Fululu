using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeGhostChase : AoeGhostState
{
    [Header("Movement")]
    public float speed = 10;
    public float minRadiusFromPlayer = 10f;
    public float accelerationTime = 1f;

    private Transform player;
    private float timer;

    public override string getStateName()
    {
        return "AOEChase";
    }

    public override void enter()
    {
        player = controller.player.transform;
        timer = 0;

        controller.rb.velocity = Vector3.zero;

        controller.StopAttacking();
    }

    public override void run()
    {
        // while player is not within min distnace, keep moving towards player
        if (Vector3.Distance(controller.transform.position, player.position) > minRadiusFromPlayer)
        {
            Vector3 directionToPlayer = player.position - controller.transform.position;
            directionToPlayer.y = 0;
            directionToPlayer.Normalize();

            if(controller.rb.velocity.magnitude < speed)
            {
                // accelerate to full speed
                controller.rb.velocity = Vector3.Lerp(controller.rb.velocity, directionToPlayer * speed, timer / accelerationTime);

            }
            else
            {
                // move at constant speed;
                controller.rb.velocity = directionToPlayer * speed;
            }
        }
        else
        {
            controller.switchState("AOEStaticAttack");

        }

        timer += Time.deltaTime;

        // look at player
        controller.transform.LookAt(player.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRadiusFromPlayer);
    }
}
