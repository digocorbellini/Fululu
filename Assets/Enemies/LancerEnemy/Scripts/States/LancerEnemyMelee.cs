using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyMelee : LancerEnemyState
{
    public float speed = 4;
    public float beginAttackDistance = 2f;
    public AttackHitbox meleeHitbox;
    public float startLag = .2f;
    public float activeHitboxTime;
    public float endLagTime;

    private Transform player;

    public override string getStateName()
    {
        return "AOEMelee";
    }

    public override void enter()
    {
        controller.rb.velocity = Vector3.zero;
        player = controller.player.transform;

        controller.StopAttacking();
        meleeHitbox.gameObject.SetActive(true);
        meleeHitbox.ResetAlreadyHit();
        meleeHitbox.gameObject.SetActive(false);

        StartCoroutine(chase());
    }

    public override void run()
    {
        // if we get out of melee range, go back to idle 
        if (Vector3.Distance(player.position, controller.transform.position) > controller.meleeAttackRange)
        {
            controller.switchState("AOEIdle");
        }
    }

    private IEnumerator chase()
    {
        // run after the player until they are within range for attack
        while (Vector3.Distance(player.position, controller.transform.position) > beginAttackDistance)
        {
            Vector3 directionToPlayer = player.position - controller.transform.position;
            directionToPlayer.y = 0;
            directionToPlayer.Normalize();

            controller.rb.velocity = directionToPlayer * speed;

            controller.transform.forward = directionToPlayer;

            yield return null;
        }

        controller.rb.velocity = Vector3.zero;

        StartCoroutine(performAttack());
    }

    private IEnumerator performAttack()
    {
        yield return new WaitForSeconds(startLag);

        // perform actual melee attack
        meleeHitbox.gameObject.SetActive(true);

        print("AOE melee attack!!");

        yield return new WaitForSeconds(activeHitboxTime);

        meleeHitbox.gameObject.SetActive(false);

        yield return new WaitForSeconds(endLagTime);

        controller.switchState("AOEIdle");
    }

    public override void exit()
    {
        StopAllCoroutines();
        meleeHitbox.gameObject.SetActive(false);
    }
}
