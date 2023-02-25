using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatController : ControllerBase
{
    [HideInInspector]
    public AudioSource audioSource;

    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    public Vector3 targetPos;

    public override void init()
    {
        base.init();
        rb = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        hitbox = GetComponent<EntityHitbox>();
        audioSource = GetComponent<AudioSource>();
        player = GameManager.instance.player.gameObject;
    }

    private void OnHurt(float damage, bool isExplosive, Collider other)
    {
        audioSource.PlayOneShot(hurtSFX);
    }

    private void OnDeath()
    {
        audioSource.PlayOneShot(hurtSFX);
        Destroy(gameObject);
        //switchState("GoatDeath");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPos, .5f);
    }
}
