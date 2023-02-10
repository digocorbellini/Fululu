using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericGhostDeath : GenericGhostState
{
    public float duration = .4f;
    public AudioClip deathSFX;
    private float timer;

    private bool aniDone;
    public override void enter()
    {
        aniDone = false;
        timer = duration;
        if(controller)
        {

        }
        controller.ani?.Play("Death");
        //controller.audioSource?.PlayOneShot(deathSFX);
    }

    public override void run()
    {
        // TODO: implement ghost animations

        //if (!aniDone)
        //{
        //    if (controller.isAnimationDone("Death"))
        //    {
        //        aniDone = true;
        //    }
        //}
        //else
        //{
        //    timer -= Time.deltaTime;
        //}

        //if(timer < 0)
        //{
        //    Destroy(controller.gameObject);
        //}

        Destroy(controller.gameObject);
    }

    public override string getStateName()
    {
        return "PGDeath";
    }
}
