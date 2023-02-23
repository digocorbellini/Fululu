using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyMidRange : LancerEnemyState
{
    public EnemyFireControl fireControl;
    public float fireRate = 1.5f;

    private Transform player;
    private float timer;

    public override string getStateName()
    {
        return "AOEMidRange";
    }

    public override void enter()
    {
        player = controller.player.transform;
        //controller.ani.SetBool("IsMoving", false);

        timer = 0;
    }

    public override void run()
    {
        if (timer >= fireRate)
        {
            fireControl.Fire();
            controller.ani.CrossFade("Shootforward_001", 0.25f);
            timer = 0;
        }
        timer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer > controller.midRangeAttackRange)
            controller.switchState("AOEIdle");

        // TODO: see if we have to switch back to idle state between shots

        // look at player
        controller.transform.LookAt(player.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }

    //public override void exit()
    //{
    //    fireControl.autoFire = false;
    //}
}
