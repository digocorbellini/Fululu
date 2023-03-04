using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBugController : ControllerBase
{
    [Space(10)]
    public float shieldStrength = 10f;
    public GameObject shieldMesh;

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

        hitbox.GrantShieldBuff(shieldStrength, 10000);
        hitbox.OnShieldBreak += ShieldBreak;
        hitbox.OnHurt += OnHurt;
        hitbox.OnDeath += OnDeath;
        ani.Play("Idle");
    }

    private void ShieldBreak(bool timeout)
    {
        shieldMesh.SetActive(false);
    }

    private void OnHurt(float damage, bool isExplosive, Collider other)
    {
        audioSource.PlayOneShot(hurtSFX);
    }

    private void OnDeath()
    {
        audioSource.PlayOneShot(hurtSFX);
        ani.Play("Death");
        Destroy(gameObject, 1.5f);
    }

}
