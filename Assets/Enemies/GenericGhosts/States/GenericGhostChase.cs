using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericGhostChase : GenericGhostState
{
    public float speed = 10;

    private Transform player;

    public override string getStateName()
    {
        return "PGChase";
    }

    public override void enter()
    {
        player = controller.player.transform;
    }

    public override void run()
    {
        /*
         * TODO:
         * - if not further than max distance, pick between idle and rotate
         * - if past max distance, move towards player position
         */

        if (Vector3.Distance(controller.transform.position, player.position) <= controller.MinDistanceFromPlayer)
        {
            // pick randomly between idle and rotating states
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                controller.switchState("PGIdle");
            }
            else
            {
                controller.switchState("PGEncircle");
            }

            // maybe have different behaviour if distance is less than min
        }
        else
        {
            Vector3 directionToPlayer = player.position - controller.transform.position;
            directionToPlayer.y = 0;
            directionToPlayer.Normalize();

            // TODO: maybe add some random zig zagging

            controller.rb.velocity = directionToPlayer * speed;
            // look at player
            controller.transform.LookAt(controller.player.transform.position);
            Vector3 rot = controller.transform.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            controller.transform.eulerAngles = rot;
        }
    }
}
