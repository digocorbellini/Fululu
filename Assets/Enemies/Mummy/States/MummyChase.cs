using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyChase : MummyState
{
    [Header("Movement")]
    public float speed = 3;
    public float chaseDuration = 8;

    [Header("Attack")]
    public EnemyFireControl trippleAttack;

    private Transform player;
    private float timer;

    public override string getStateName()
    {
        return "BChase";
    }

    public override void enter()
    {
        player = controller.player.transform;
        controller.rb.velocity = Vector3.zero;

        trippleAttack.ResetTimer();
        trippleAttack.autoFire = true;

        timer = 0;
    }

    public override void run()
    {
        if (timer >= chaseDuration)
        {
            // go to a random state
            controller.switchState(controller.GetRandomState());
        }
        timer += Time.deltaTime;

        // handle chase
        Vector3 directionToPlayer = player.position - controller.transform.position;
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();

        controller.rb.velocity = directionToPlayer * speed;

        // look at player
        controller.transform.LookAt(controller.player.transform.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }

    public override void exit()
    {
        trippleAttack.autoFire = false;
    }
}
