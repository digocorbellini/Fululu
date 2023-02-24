using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Vector2 spawnInterval = new Vector2(8.0f, 12.0f);
    public bool isActive = true;
    public Transform[] spawnpoints;
    [Range(0, 1)]
    public float SpawnChance= 1.0f;

    public int totalEnemyCount = 30;
    public int minEnemyCount = 5;
    public GameObject[] barriers;
    public SpawnBoss bossSpawner;

    [Header("Spawn Lists")]
    public GameObject[] enemies;
    public int[] weights;

    public AudioSource spawnSFX;

    private int enemiesLeft;
    private int activeEnemies = 0;
    private bool proximity;
    private bool initialSpawn = false;

    private float timer;
    private int weightSum;

    private bool bossSpawned = false;

    private void Start()
    {
        GameManager.instance.OnReset += OnReset;
        if(enemies.Length != weights.Length)
        {
            Debug.LogError("Enemy and SpawnWeights list must be same length!");
        }

        weightSum = weights.Sum();
        enemiesLeft = totalEnemyCount;
        timer = int.MaxValue;

        ToggleBarriers(false);
    }

    private void ToggleBarriers(bool active)
    {
        barriers.ToList().ForEach(barrier => barrier.SetActive(active));
    }

    private void Update()
    {
        /* if(initialSpawn)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                DoSpawn();
            } 
        } */
    }

    private void OnReset()
    {
        initialSpawn = false;
        enemiesLeft = totalEnemyCount;
        activeEnemies = 0;
        bossSpawned = false;
        ToggleBarriers(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            proximity = true;

            if (!initialSpawn)
            {
                DoSpawn();
                initialSpawn = true;
                ToggleBarriers(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            proximity = false;
        }
    }

    private void DoSpawn(bool inital = false)
    {

        /* if (!isActive || !proximity || totalEnemyCount > 0)
        {
            // This spawn cycle failed. Try again later
            print("Spawn cycle failed!");
            timer = Random.Range(spawnInterval.x, spawnInterval.y);
            return;
        } */
        bool didSpawn = false;

        foreach (Transform point in spawnpoints){
            if(Random.Range(0,1) < SpawnChance)
            {
                if (enemiesLeft < 0)
                {
                    break;
                }

                GameObject spawned = Instantiate(WeightedRandomSpawn(), point.position, Quaternion.identity);
                spawned.GetComponentInChildren<EntityHitbox>().OnDeath += OnEnemyDefeated;
                enemiesLeft--;
                activeEnemies++;
                didSpawn = true;
            }
        }

        if(didSpawn)
        {
            spawnSFX.Play();
        }
    }

    private GameObject WeightedRandomSpawn()
    {
        int randomWeight = Random.Range(0, weightSum);
        int currWeight = 0;
        
        for(int i = 0; i < enemies.Length; i++)
        {
            currWeight += weights[i];
            if(randomWeight <= currWeight)
            {
                return enemies[i];
            }
        }

        return enemies[0];
    }

    private void OnEnemyDefeated()
    {
        activeEnemies--;

        print("Enemies left: " + (enemiesLeft + activeEnemies));

        if((enemiesLeft + activeEnemies) <= 0)
        {
            if (!bossSpawned)
            {
                if (bossSpawner)
                {
                    // Spawn boss, wait for boss to be defeated
                    GameObject boss = bossSpawner.SpawnTheBoss();
                    boss.GetComponentInChildren<EntityHitbox>().OnDeath += OnCleared;
                }
                else
                {
                    // No boss to spawn
                    OnCleared();
                }

                bossSpawned = true;
            }
            return;
        }

        if(activeEnemies <= minEnemyCount && !bossSpawned)
        {
            DoSpawn();
        }
    }

    private void OnCleared()
    {
        isActive = false;
        ToggleBarriers(false);
    }

}
