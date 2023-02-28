using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeController : ControllerBase
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public AudioSource audioSource;

    public override void init()
    {
        base.init();
        
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<EntityHitbox>();
        ani = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        hitbox.OnDeath += HandleOnDeath;
        hitbox.OnHurt += HandleOnHurt;
    }

    private void HandleOnDeath()
    {
        print("MONKE DIED");
        Destroy(this.gameObject);
    }

    private void HandleOnHurt(float damage, bool isExplosive, Collider other)
    {
        if (hurtSFX)
            audioSource.PlayOneShot(hurtSFX);
        print("monke hurt. Took " + damage + " damage");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        hitbox.OnDeath -= HandleOnDeath;
        hitbox.OnHurt -= HandleOnHurt;
    }
}
