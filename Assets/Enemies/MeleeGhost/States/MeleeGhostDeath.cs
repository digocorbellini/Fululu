using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGhostDeath : MeleeGhostState
{
    public float duration = .4f;
    public AudioClip deathSFX;
    private float timer;

    private bool aniDone;
    public override void enter()
    {
        aniDone = false;
        timer = duration;
        controller.ani.Play("Death");
        controller.audioSource.PlayOneShot(deathSFX);
    }

    public override void run()
    {
        if (!aniDone)
        {
            if (controller.isAnimationDone("Death"))
            {
                aniDone = true;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if(timer < 0)
        {
            Destroy(controller.gameObject);
        }
        
    }

    public override string getStateName()
    {
        return "MGDeath";
    }
}
