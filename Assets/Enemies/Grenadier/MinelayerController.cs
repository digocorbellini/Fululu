using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinelayerController : ControllerBase
{
    [HideInInspector]
    public AudioSource audioSource;

    public GameObject player;
    public GameObject mine;
    public Transform minePoint;

    public Vector3 targetPoint;

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

    public void LayMine()
    {
        GameObject m = Instantiate(mine, minePoint.transform.position, Quaternion.identity);
    }

    private void OnHurt(float damage, bool isExplosive, Collider other)
    {
        audioSource.PlayOneShot(hurtSFX);
    }

    private void OnStun()
    {
        if (!isStunned)
        {
            switchState("MLStun");
        }
    }

    private void OnDeath()
    {
        if (audioSource)
        {
            audioSource?.PlayOneShot(hurtSFX);
        }
        switchState("MLDeath");
    }
}
