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
    private ControllerBase controller;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<ControllerBase>();
        timer = autoFireInterval;
    }

    private void Update()
    {
        if (autoFire)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                Fire();
                timer = autoFireInterval;
            }
        }
    }

    // Call automatically if autoFire, or call using your state machine
    public bool Fire()
    {
        if (controller.isStunned)
        {
            return false;
        }

        weapon.Fire(firePos, null);
        return true;
    }
}
