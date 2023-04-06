using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public GameObject bossPrefab;
    public GameObject fakeBossPrefab;
    public EnemySpawner bossMinionSpawner;
    public GameObject[] barriers;

    private bool isSpawned = false;
    private bool isCleared = false;
    private GameObject boss;
    private GameObject fakeBoss;
    

    public GameObject SpawnTheBoss()
    {
        boss = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        EntityHitbox bossHitbox = boss.GetComponent<EntityHitbox>();

        if (bossMinionSpawner)
        {
            bossMinionSpawner.RegisterToBoss(bossHitbox);
        }

        bossHitbox.OnDeath += OnCleared;
        isSpawned = true;
        return boss;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !isSpawned)
        {
            Destroy(fakeBoss);
            barriers.ToList().ForEach(barrier => barrier.SetActive(true));
            SpawnTheBoss();
        }
    }

    private void OnCleared()
    {
        GameManager.instance.OnReset -= OnReset;
        barriers.ToList().ForEach(barrier => barrier.SetActive(false));
    }

    private void OnReset()
    {
        isSpawned = false;
        barriers.ToList().ForEach(barrier => barrier.SetActive(false));
        fakeBoss = Instantiate(fakeBossPrefab, transform.position, Quaternion.identity);
    }

    private void OnEnable()
    {
        GameManager.instance.OnReset += OnReset;
        fakeBoss = Instantiate(fakeBossPrefab, transform.position, Quaternion.identity);
    }
}
