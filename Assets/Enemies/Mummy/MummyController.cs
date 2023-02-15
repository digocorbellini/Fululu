using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EnemyFireControl))]
public class MummyController : ControllerBase
{
    [HideInInspector] public GameObject player;


    public AudioSource source;
    public AudioClip deathSound;
    public AudioClip spawnSound;
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

        source.PlayOneShot(spawnSound);
    }

    private void OnHurt(float damage, bool isExplosive)
    {
        print("Boss hurt. " + damage + " damage taken");
    }

    private void OnDeath()
    {
        print("Boss killed!!");
        isStateMachineActive = false;
        source.PlayOneShot(deathSound);

        rb.velocity = Vector3.zero;

        Destroy(this.gameObject, deathSound.length);

    }

    private void OnDestroy()
    {
        hitbox.OnHurt -= this.OnHurt;
        hitbox.OnDeath -= this.OnDeath;
    }

    /// <summary>
    /// Get a random state. Will try to avoid getting the same state.
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

            depth++;
        }

        return randomState;
    }

}
