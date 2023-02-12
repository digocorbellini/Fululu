using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyMidRange : LancerEnemyState
{
    public EnemyFireControl fireControl;

    private Transform player;

    public override string getStateName()
    {
        return "AOEMidRange";
    }

    public override void enter()
    {
        player = controller.player.transform;

        fireControl.autoFire = true;
    }

    public override void run()
    {
        // look at player
        controller.transform.LookAt(player.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }

    public override void exit()
    {
        fireControl.autoFire = false;
    }
}
