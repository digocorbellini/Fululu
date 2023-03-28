using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerEnemyDeath : LancerEnemyState
{
    public AudioClip deathSFX;
    public GameObject deathParticlePrefab;
    public Transform deathParticleSpawnPoint;
    public float deathParticleScale = 1.5f;

    public override string getStateName()
    {
        return "AOEDeath";
    }

    public override void enter()
    {
        controller.rb.velocity = Vector3.zero;
        controller.rb.angularVelocity = Vector3.zero;

        controller?.audioSource.PlayOneShot(deathSFX);
    }

    public override void run()
    {
        // only destroy lancer after death animation is done playing
        if (controller.isAnimationDone("Dead"))
        {
            GameObject instance = Instantiate(deathParticlePrefab, deathParticleSpawnPoint.position, deathParticleSpawnPoint.rotation);
            instance.transform.localScale = Vector3.one * deathParticleScale;
            Destroy(controller.gameObject);
        }
    }
}
