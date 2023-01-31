using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinelayerWander : MinelayerState
{
    public float speed = 0.1f;
    public float distanceThreshold = .3f;
    public float maxTime = 12f;
    Vector3 target;

    private float timer;
    public override void enter()
    {
        target = controller.targetPoint;
        timer = maxTime;
    }

    public override void run()
    {
        Vector3 newPos = Vector3.MoveTowards(controller.transform.position, target, speed);
        controller.transform.position = newPos;

        controller.transform.LookAt(controller.player.transform);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;

        timer -= Time.deltaTime;

        if(Vector3.Distance(controller.transform.position, target) < distanceThreshold || timer <= 0)
        {
            controller.switchState("MLIdle");
        }
    }

    public override string getStateName()
    {
        return "MLWander";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(target, .1f);
    }
}
