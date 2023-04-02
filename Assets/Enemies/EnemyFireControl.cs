using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireControl : MonoBehaviour
{
    public Weapon weapon;
    public Transform firePos;

    public bool leadShots = false;
    public bool autoFire = false;
    public float autoFireInterval = 5;

    private float timer;
    private Transform player;
    [Space(10)]
    public ParticleSystem telegraphParticles;
    public float telegraphTime = .5f;


    private bool hasTelegraphed;
    [SerializeField] private ControllerBase controller;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(controller == null)
            controller = GetComponent<ControllerBase>();
        timer = autoFireInterval;
    }

    private void Update()
    {
        if (autoFire)
        {
            timer -= Time.deltaTime;


            if (!hasTelegraphed && timer <= telegraphTime)
            {
                hasTelegraphed = true;
                if (telegraphParticles)
                {
                    telegraphParticles.Play();
                }
            }

            if (timer <= 0)
            {
                Fire();
                timer = autoFireInterval;
                hasTelegraphed = false;
            }
        }
    }

    // Call automatically if autoFire, or call using your state machine
    public bool Fire()
    {
        if (controller && controller.isStunned)
        {
            return false;
        }

        weapon?.Fire(firePos, null);
        return true;
    }

    // Call to reset the autofire timer 
    public void ResetTimer()
    {
        timer = autoFireInterval;
    }

    public void ClearProjectiles()
    {
        weapon?.ClearBullets();
    }
}
