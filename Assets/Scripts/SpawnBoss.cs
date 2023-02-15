using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public EnemySpawner spawner;
    public GameObject boss;


    // Start is called before the first frame update
    void Start()
    {
        spawner.OnClear += SpawnTheBoss;
    }

    private void SpawnTheBoss()
    {
        Instantiate(boss, transform.position, Quaternion.identity);
    }
}
