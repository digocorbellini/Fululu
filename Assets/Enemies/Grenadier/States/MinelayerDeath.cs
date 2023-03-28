using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinelayerDeath : MinelayerState
{
    public float duration = .4f;
    public AudioClip deathSFX;
    public GameObject deathParticlePrefab;
    public Transform deathParticleSpawnPoint;
    public float deathParticleScale = 1.5f;
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
            GameObject instance = Instantiate(deathParticlePrefab, deathParticleSpawnPoint.position, deathParticleSpawnPoint.rotation);
            instance.transform.localScale = Vector3.one * deathParticleScale;
            Destroy(controller.gameObject);
        }
        
    }

    public override string getStateName()
    {
        return "MLDeath";
    }
}
