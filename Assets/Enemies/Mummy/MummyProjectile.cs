using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyProjectile : BulletBase
{
    public float moveDelay = .5f;
    public float pullBackTime = 1f;
    public float pullBackSpeed = 5;
    public float pullbackDistance;

    public bool shouldMoveOnPullback = true;
    public float playerHeightOffset = 1;

    private Transform player;
    private Vector3 originalPos;

    void Start()
    {
        hitbox = GetComponent<AttackHitbox>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (pullBackTime > 0.01f || moveDelay > 0.01f)
            StartCoroutine(startDelay());
        else
            rb.velocity = transform.forward * Speed;

        StartCoroutine(runLifetime());
    }

    private IEnumerator runLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        DetatchTrails();
        Destroy(this.gameObject);
    }

    // TODO: account for targetting player while waiting for bullets to pull back

    IEnumerator startDelay()
    {
        float timeElapsed = 0;

        while(timeElapsed < moveDelay)
        {
            if(shouldMoveOnPullback)
                transform.LookAt(player.position + new Vector3(0, playerHeightOffset, 0));

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        originalPos = transform.position;
        Vector3 pullbackPos = originalPos + (-1 * transform.forward * pullbackDistance);

        timeElapsed = 0;
        while (timeElapsed < pullBackTime)
        {
            //handle pull back
            transform.position = Vector3.Lerp(transform.position, pullbackPos, Time.deltaTime * pullBackSpeed);
            timeElapsed += Time.deltaTime;

            // look towards player
            // TODO: replace with leading shot
            if (shouldMoveOnPullback)
            {
                transform.LookAt(player.position + new Vector3(0, playerHeightOffset, 0));
            }

            pullbackPos = originalPos + (-1 * transform.forward * pullbackDistance);

            yield return null;
        }

        // start movement
        rb.velocity = transform.forward * Speed;
    }
}
