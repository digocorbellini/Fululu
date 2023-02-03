using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinelayerIdle : MinelayerState
{
    public float minWander = 8.0f;
    public float maxWander = 3.0f;

    public float idleTime = 1.0f;

    private float timer;
    
    public override void enter()
    {
        timer = idleTime;

        // controller.LayMine();
        controller.ani.Play("Idle");

        bool hasNextPoint = false;
        int tries = 10;

        //Generate next wander point
        while(!hasNextPoint)
        {
            Vector3 direction = Random.insideUnitCircle.normalized * Random.Range(minWander, maxWander);
            direction.z = direction.y;
            direction.y = 0;

            Vector3 target = transform.position + direction;

            if(!Physics.Linecast(transform.position, target, ~(1 << 7) ))
            {
                controller.targetPoint = target;
                hasNextPoint = true;
            }

            tries--;

            if(tries > 10)
            {
                hasNextPoint = true;
                timer = 7;
                target = transform.position;
            }
        }
    }

    public override void run()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            controller.switchState("MLWander");
        }
    }

    public override string getStateName()
    {
        return "MLIdle";
    }
}
