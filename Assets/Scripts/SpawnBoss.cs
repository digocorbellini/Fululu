using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public GameObject boss;
    public EnemySpawner bossMinionSpawner;

    public GameObject SpawnTheBoss()
    {
        GameObject b = Instantiate(boss, transform.position, Quaternion.identity);

        if (bossMinionSpawner)
        {
            bossMinionSpawner.RegisterToBoss(b.GetComponentInChildren<EntityHitbox>());
        }
        return b;
    }
}
