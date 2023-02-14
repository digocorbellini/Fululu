using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericGhostController : ControllerBase
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
        // TODO: implement stun
    }

    private void OnHurt(float damage, bool isExplosive)
    {
        // TODO: damage 
        audioSource.PlayOneShot(hurtSFX);
    }

    private void OnDeath()
    {
        switchState("PGDeath");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
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
