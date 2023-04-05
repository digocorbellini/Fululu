using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MummyController : ControllerBase
{
    [HideInInspector] public GameObject player;

    public AudioSource source;
    public AudioClip deathSound;
    public AudioClip spawnSound;
    public EnemyFireControl ringController;
    public GameObject deathParticlePrefab;
    public Transform deathParticleSpawnPoint;
    public float deathParticleScale = 3f;
    public GameObject mummyDeathHandler;

    private bool HasDoneFlamethrower = false;
    private float flamethrowerThreshold = -1;
    public override void init()
    {
        base.init();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<EntityHitbox>();

        hitbox.OnHurt += this.OnHurt;
        hitbox.OnDeath += this.OnDeath;

        foreach(State state in states)
        {
            if(state is MummyFlamethrower)
            {
                flamethrowerThreshold = ((MummyFlamethrower)state).startHP;
                break;
            }
        }

        source.PlayOneShot(spawnSound);
    }

    private void OnHurt(float damage, bool isExplosive, Collider other)
    {
        source.PlayOneShot(hurtSFX);

        if(hitbox.HealthPercent() <= flamethrowerThreshold && !HasDoneFlamethrower)
        {
            HasDoneFlamethrower = true;
            switchState("BFlamethrower");
        }
    }

    private void OnDeath()
    {
        currentState.exit();
        if (currentState is MummyState mumState)
        {
            mumState.enemyFireControl.ClearProjectiles();
        }
        source.PlayOneShot(hurtSFX);
        print("Boss killed!!");
        isStateMachineActive = false;
        source.PlayOneShot(deathSound);

        rb.velocity = Vector3.zero;

        ringController.autoFire = false;
        Instantiate(mummyDeathHandler, deathParticleSpawnPoint.position, Quaternion.identity);

        Destroy(this.gameObject, deathSound.length);
    }

    private void OnDestroy()
    {
        hitbox.OnHurt -= this.OnHurt;
        hitbox.OnDeath -= this.OnDeath;
        GameObject instance = Instantiate(deathParticlePrefab, deathParticleSpawnPoint.position, deathParticleSpawnPoint.rotation);
        instance.transform.localScale = Vector3.one * deathParticleScale;
    }

    /// <summary>
    /// Get a random state. Will try to avoid getting the same state.
    /// Does not return "Flamethrower" state
    /// </summary>
    /// <returns>A random state</returns>
    public State GetRandomState()
    {
        State randomState = currentState;

        int depth = 0;
        while(randomState == currentState && depth <= 5)
        {
            int randomIndex = Random.Range(0, states.Length);

            randomState = states[randomIndex];

            if(randomState is MummyFlamethrower)
            {
                randomState = states[0]; 
            }
            depth++;
        }

        return randomState;
    }

}
