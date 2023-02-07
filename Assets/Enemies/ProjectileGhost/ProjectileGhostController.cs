using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGhostController : ControllerBase
{
    public GameObject player;
    [HideInInspector] public AudioSource audioSource;

    public float MinDistanceFromPlayer = 10;
    public float MaxDistanceFromPlayer = 20;

    public override void init()
    {
        base.init();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<EntityHitbox>();
        ani = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        isStunned = false;

        hitbox.OnHurt += this.OnHurt;
        hitbox.OnDeath += this.OnDeath;
        hitbox.OnStun += this.OnStun;

        // TODO: maybe find player instead of having a public reference
    }

    private void OnStun()
    {
        // change to stun state
        if(!isStunned)
            switchState("PGStun");
    }

    private void OnHurt(float damage, bool isExplosive)
    {
        // TODO: damage 
        print("hurt projectile ghost. Damage done: " + damage);
    }

    private void OnDeath()
    {
        switchState("PGDeath");
    }

    private void OnDestroy()
    {
        hitbox.OnHurt -= this.OnHurt;
        hitbox.OnStun -= this.OnStun;
        hitbox.OnDeath -= this.OnDeath;
    }

    private void OnDrawGizmosSelected()
    {
        if(player)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.transform.position, MaxDistanceFromPlayer);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, MinDistanceFromPlayer);
        }
    }
}
