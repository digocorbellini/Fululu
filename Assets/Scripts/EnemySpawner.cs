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

    [Header("Spawn Lists")]
    public GameObject[] enemies;
    public int[] weights;

    public AudioSource spawnSFX;

    private int currentEnemyCount;
    private bool proximity;
    private bool initialSpawn = false;

    private float timer;
    private int weightSum;

    public delegate void ClearEvent();
    public event ClearEvent OnClear;
    public void CallOnClear() => OnClear?.Invoke();

    private void Start()
    {
        if(enemies.Length != weights.Length)
        {
            Debug.LogError("Enemy and SpawnWeights list must be same length!");
        }

        weightSum = weights.Sum();
        timer = int.MaxValue;
    }

    private void Update()
    {
        if(initialSpawn && currentEnemyCount <= 0)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                DoSpawn();
            }
        }
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

        foreach (Transform point in spawnpoints){
            if(Random.Range(0,1) < SpawnChance)
            {
                GameObject spawned = Instantiate(WeightedRandomSpawn(), point.position, Quaternion.identity);
                spawned.GetComponentInChildren<EntityHitbox>().OnDeath += OnEnemyDefeated;
                currentEnemyCount++;
                totalEnemyCount--;

                if(totalEnemyCount <= 0)
                {
                    break;
                }
            }
        }

        if(currentEnemyCount > 0)
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
        currentEnemyCount--;

        print("Enemies left: " + (currentEnemyCount+ totalEnemyCount));

        if(totalEnemyCount <= 0)
        {
            CallOnClear();
            return;
        }

        if(currentEnemyCount <= minEnemyCount)
        {
            DoSpawn();
        }
    }

}
