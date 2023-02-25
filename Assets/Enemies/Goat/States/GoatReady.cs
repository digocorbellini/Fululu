using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatReady : GoatState
{
    public float readyTime = 2.0f;
    public Transform front;
    public LayerMask blockers;

    private float timer;
    private bool isReady;
    private GameObject player;

    public override void enter()
    {
        timer = readyTime;
        isReady = false;
        player = controller.player;
    }

    public override void run()
    {
        timer -= Time.deltaTime;

        controller.transform.LookAt(player.transform.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
        // For some reason rotating also moves the rigidbody??
        controller.rb.velocity = Vector3.zero;

        if (timer <= 0)
        {
            // Raycast to check line of sight to player
            if(!Physics.Linecast(front.position, player.transform.position, blockers))
            {
                
                Vector3 dir = (player.transform.position - controller.transform.position).normalized;
                dir.y = 0;

                Vector3 pos = player.transform.position + (dir * 2);
                pos.y = controller.transform.position.y;
                controller.targetPos = pos;


                controller.switchState("GoatCharge");
            }
            else
            {
                timer = .5f;
            }
        }
    }

    public override void exit()
    {
       
    }

    public override string getStateName()
    {
        return "GoatReady";
    }
}
