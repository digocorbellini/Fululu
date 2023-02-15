using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyRingAttack : MummyState
{
    public float duration = 5;
    public float shotDelay = 1f;

    public EnemyFireControl fireControl;
    public Transform ringSpawn;

    private float timer;

    public override string getStateName()
    {
        return "BRingAttack";
    }

    public override void enter()
    {
        controller.rb.velocity = Vector3.zero;
        controller.rb.angularVelocity = Vector3.zero;

        timer = 0;

        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(shotDelay);
        fireControl.Fire();
    }

    public override void run()
    {
        controller.rb.velocity = Vector3.zero;

        if (timer >= duration)
        {
            // turn off attack
            //projetileAttack.StopShooting();
            //projetileAttack.enabled = false;

            // go to a random state
            controller.switchState(controller.GetRandomState());
        }

        timer += Time.deltaTime;

        // look at player
        controller.transform.LookAt(controller.player.transform.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;

        // make ring point at player
        ringSpawn.LookAt(controller.player.transform.position);
    }
}
