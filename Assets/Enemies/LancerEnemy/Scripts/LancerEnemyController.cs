using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityHitbox))]
[RequireComponent(typeof(Rigidbody))]
public class LancerEnemyController : ControllerBase
{
    public float detectionRadius = 15f;
    public float longRangeAttackRange = 25f;
    public float meleeAttackRange = 5f;
    public float midRangeAttackRange = 10f;

    [Header("Rain Attack Stats")]
    public AOERainAttack aoeAttack;
    public float timeBetwenAttacks;

    [HideInInspector] public GameObject player;

    private bool isAttacking;
    public override void init()
    {
        base.init();

        isAttacking = false;

        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<EntityHitbox>();
        ani = GetComponentInChildren<Animator>();

        hitbox.OnDeath += HandleOnDeath;
        hitbox.OnHurt += HandleOnHurt;
    }

    public override void run()
    {

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // check for player being within either mid range or melee range
        string currentStateName = currentState.getStateName();
        if (currentStateName != "AOEMelee" && distanceToPlayer < meleeAttackRange)
        {
            switchState("AOEMelee");
        }
        else if(currentStateName != "AOEMidRange" && currentStateName != "AOEMelee" && distanceToPlayer < midRangeAttackRange)
        {
            switchState("AOEMidRange");
        }

        // see if player is out of range so that we go to idle (if not already in idle)
        if (currentStateName != "AOEIdle" && distanceToPlayer > detectionRadius)
        {
            switchState("AOEIdle");
        }
    }

    private void HandleOnDeath()
    {
        print("AOE enemy killed");
        //Destroy(this.gameObject);
        switchState("AOEDeath");
    }

    private void HandleOnHurt(float daamage, bool isExplosive)
    {
        //uncomment when audiosource added to this enemy
        //audioSource.PlayOneShot(hurtSFX);
        print("AOE enemy took: " + daamage + " damage");
    }

    /// <summary>
    /// Start performing the AOE rain attack until "StopAttacking" is called.
    /// </summary>
    public void StartAttacking()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        StartCoroutine(performAttack());
    }

    private IEnumerator performAttack()
    {
        while (isAttacking)
        {
            aoeAttack.attack();
            ani.Play("ShootUp");
            yield return new WaitForSeconds(aoeAttack.getTotalAttackTime() + timeBetwenAttacks);
        }  
    }

    /// <summary>
    /// Stop performing the AOE rain attack
    /// </summary>
    public void StopAttacking()
    {
        if (!isAttacking)
            return;

        isAttacking = false;
        StopAllCoroutines();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        hitbox.OnDeath -= HandleOnDeath;
        hitbox.OnHurt -= HandleOnHurt;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midRangeAttackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, longRangeAttackRange);
    }
}
