using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UltEvents;

static class RandomExtensions
{
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}

[System.Serializable]
public class Wave
{
    public EnemySpawn[] enemies;
}

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemy;
    public int count = 1;
}

public class WaveSpawner : MonoBehaviour
{
    public bool isActive = true;
    public Transform[] spawnpoints;

    public int minEnemyCount = 5;
    public GameObject[] barriers;

    public Wave[] waves;

    public AudioSource spawnSFX;

    public GameObject[] activateOnClear;

    public GameObject spawnParticles;

    [Header("Events")]
    public UltEvent OnActivate;
    public UltEvent OnClear;

    private int activeEnemies = 0;
    private int waveNum = 0;
    private bool initialSpawn = false;
    private bool isCleared = false;
    private void Start()
    {
        GameManager.instance.OnReset += OnReset;

        ToggleBarriers(false);

        waveNum = 0;
    }

    private void ToggleBarriers(bool active)
    {
        barriers.ToList().ForEach(barrier => barrier.SetActive(active));
    }

    private void OnReset()
    {
        if (isCleared)
        {
            return;
        }

        initialSpawn = false;
        activeEnemies = 0;
        waveNum = 0;
        ToggleBarriers(false);

        activateOnClear.ToList().ForEach(obj => obj.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!initialSpawn)
            {
                OnActivate.Invoke();
                DoSpawn();
                initialSpawn = true;
                ToggleBarriers(true);
            }
        }
    }

    private void DoSpawn(bool inital = false)
    {
        bool didSpawn = false;

        if (waveNum < waves.Length)
        {
            Wave wave = waves[waveNum];
            int spawnIndex = 0;
            spawnpoints.Shuffle();

            foreach (EnemySpawn enemy in wave.enemies)
            {
                for (int i = 0; i < enemy.count; i++)
                {
                    Transform spawn = spawnpoints[spawnIndex];
                    GameObject spawned = Instantiate(enemy.enemy, spawn.position, spawn.rotation);
                    spawned.GetComponentInChildren<EntityHitbox>().OnDestroyed += OnEnemyDefeated;
                    activeEnemies++;
                    didSpawn = true;

                    Instantiate(spawnParticles, spawn.position, spawn.rotation);

                    spawnIndex++;
                    if (spawnIndex >= spawnpoints.Length)
                    {
                        spawnIndex = 0;
                        spawnpoints.Shuffle();
                    }
                }
            }
        }

        if (didSpawn)
        {
            spawnSFX.Play();
            waveNum++;
        }
    }

    private void OnEnemyDefeated()
    {
        activeEnemies--;

        if (activeEnemies <= 0 && waveNum >= waves.Length)
        {
            // No boss to spawn
            OnCleared();

            return;
        }

        if (activeEnemies <= minEnemyCount)
        {
            DoSpawn();
        }
    }

    private void OnCleared()
    {
        OnClear.Invoke();
        isActive = false;
        isCleared = true;
        ToggleBarriers(false);

        activateOnClear.ToList().ForEach(obj => obj.SetActive(true));

        // destory all enemy projectiles
        GameObject.FindObjectsOfType<BulletBase>().ToList().ForEach(bullet =>
        {
            if (bullet)
            {
                Destroy(bullet.gameObject);
            }
        });
    }
}
