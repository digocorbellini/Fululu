using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyRingProjectile : BulletBase
{
    private void Start()
    {
        // rotate to look at player on projectile spawn
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 posToLookAt = player.position;
        posToLookAt.y = transform.position.y;

        transform.LookAt(posToLookAt);
    }
}
