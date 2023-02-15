using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyIdle : MummyState
{
    [Header("Movement")]
    public float IdleDuration = 5.0f;
    public float alternateDirTime = 1;
    public float rotateSpeed = 3;

    [Header("Attack")]
    public EnemyFireControl shotController;

    private float timer = 0;
    private bool isGoingRight = true;
    private float alternateTimer = 0;

    public override string getStateName()
    {
        return "BIdle";
    }

    public override void enter()
    {
        shotController.ResetTimer();
        shotController.autoFire = true;

        controller.rb.velocity = Vector3.zero;

        timer = 0;
        alternateTimer = 0;
    }

    public override void run()
    {
        controller.rb.velocity = Vector3.zero;

        if (timer >= IdleDuration)
        {
            // go to a random state
            controller.switchState(controller.GetRandomState());
        }

        timer += Time.deltaTime;

        // handle strafing
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

    public override void exit()
    {
        shotController.autoFire = false;
    }

}
