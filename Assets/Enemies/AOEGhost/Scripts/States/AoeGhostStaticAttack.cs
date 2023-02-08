using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeGhostStaticAttack : AoeGhostState
{
    public float IdleDuration = 5.0f;
    public float alternateDirTime = 1;
    public float rotateSpeed = 3;

    private Transform player;
    private float alternateTimer = 0;
    private bool isGoingRight = true;

    public override string getStateName()
    {
        return "AOEStaticAttack";
    }

    public override void enter()
    {
        player = controller.player.transform;

        controller.rb.velocity = Vector3.zero;
        alternateTimer = 0;

        controller.StartAttacking();
    }

    public override void run()
    {
        // TODO: see if we want him to strafe or not during attacking
        //// handle strafing
        //alternateTimer += Time.deltaTime;
        //if (alternateTimer > alternateDirTime)
        //{
        //    isGoingRight = !isGoingRight;
        //    alternateTimer = 0;
        //}
        //float sign = (isGoingRight) ? 1 : -1;
        //controller.transform.RotateAround(controller.player.transform.position, Vector3.up, sign * rotateSpeed * Time.deltaTime);

        // look at player
        controller.transform.LookAt(player.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }

    public override void exit()
    {
        controller.StopAttacking();
    }


}
