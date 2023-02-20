using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostController : ControllerBase
{
    [HideInInspector]
    public AudioSource audioSource;

    [HideInInspector]
    public GameObject player;



    
    public override void init()
    {
        base.init();

        rb = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        hitbox = GetComponent<EntityHitbox>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");

        hitbox.OnHurt += this.OnHurt;
        hitbox.OnDeath += this.OnDeath;
        hitbox.OnStun += this.OnStun;
    }

    private void OnHurt(float damage, bool isExplosive, Collider other)
    {
        audioSource.PlayOneShot(hurtSFX);
    }

    private void OnStun()
    {
        if (!isStunned)
        {
            switchState("MGStun");
        }
    }

    private void OnDeath()
    {
        audioSource.PlayOneShot(hurtSFX);
        switchState("MGDeath");
    }
}
