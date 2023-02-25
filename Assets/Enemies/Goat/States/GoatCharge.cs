using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatCharge : GoatState
{
    public float maxTime = 5.0f;
    public float chargeSpeed = 10f;
    public GameObject hitbox;

    private float timer;
    private Vector3 target;

    public override void enter()
    {
        timer = maxTime;
        target = controller.targetPos;
        hitbox.SetActive(true);
    }

    public override void run()
    {
        timer -= Time.deltaTime;
        controller.transform.position = Vector3.MoveTowards(transform.position, target, chargeSpeed * Time.deltaTime);

        if(timer <= 0 || Vector3.Distance(transform.position, target) < .2f)
        {
            controller.rb.velocity = Vector3.zero;
            controller.switchState("GoatIdle");
        }
    }

    public override void exit()
    {
        hitbox.GetComponent<AttackHitbox>().ResetAlreadyHit();
        hitbox.SetActive(false);
    }

    public override string getStateName()
    {
        return "GoatCharge";
    }
}
