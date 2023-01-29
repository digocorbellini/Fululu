using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostAttack : MeleeGhostState
{
    public float endLag = .8f;
    public AudioClip attackSound;
    public AttackHitbox hitbox;
    private float timer;
    private bool hasAttacked = false;

    public override void enter()
    {
        hasAttacked = false;
        timer = endLag;
        controller.ani.Play("Attack");
        controller.audioSource.PlayOneShot(attackSound);
        hitbox.gameObject.SetActive(true);
    }

    public override void run()
    {
        if (!hasAttacked)
        {
            if (controller.isAnimationDone("Attack"))
            {
                hasAttacked = true;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            controller.switchState("MGChase");
            hitbox.ResetAlreadyHit();
            hitbox.gameObject.SetActive(false);
        }
    }

    public override string getStateName()
    {
        return "MGAttack";
    }
}
