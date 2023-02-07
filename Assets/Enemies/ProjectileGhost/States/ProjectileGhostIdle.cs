using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGhostIdle : ProjectileGhostState
{
    public float idleDuration = 5.0f;
    public float alternateDirTime = 1;
    public float rotateSpeed = 3;

    private float timer = 0;

    public bool isGoingRight = true;
    private float alternateTimer = 0;

    public override string getStateName()
    {
        return "PGIdle";
    }

    public override void enter()
    {
        controller.rb.velocity = Vector3.zero;
        controller.ani.Play("Idle");

        timer = 0;
        alternateTimer = 0;
    }

    public override void run()
    {
        // TODO: maybe if player gets too close, start encircling instead
        controller.rb.velocity = Vector3.zero;

        if (timer >= idleDuration)
        {
            if (Vector3.Distance(controller.transform.position, controller.player.transform.position) 
                > controller.MaxDistanceFromPlayer)
            {
                controller.switchState("PGChase");
            }
            else
            {
                controller.switchState("PGEncircle");
            }

            return;
        }

        timer += Time.deltaTime;

        // handle "strafing"
        alternateTimer += Time.deltaTime;
        if (alternateTimer > alternateDirTime)
        {
            isGoingRight = !isGoingRight;
            alternateTimer = 0;
        }
        float sign = (isGoingRight) ? 1 : -1;
        controller.transform.RotateAround(controller.player.transform.position, Vector3.up, sign * rotateSpeed * Time.deltaTime);

        // look at player
        controller.transform.LookAt(controller.player.transform.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }
}
