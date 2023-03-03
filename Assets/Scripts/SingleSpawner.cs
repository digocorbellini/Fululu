using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSpawner : MonoBehaviour
{
    public GameObject entity;
    public float spawnInterval = 5.0f;

    private bool hasSpawned;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        DoRespawn(); 
    }

    private void DoDeath()
    {
        StopAllCoroutines();
        StartCoroutine(RespawnTimer());
    }

    private void DoRespawn()
    {
        GameObject obj = Instantiate(entity, transform.position, Quaternion.identity);
        obj.GetComponentInChildren<EntityHitbox>().OnDestroyed += DoDeath;
    }

    IEnumerator RespawnTimer()
    {
        Debug.Log("Respawning...");
        yield return new WaitForSecondsRealtime(spawnInterval);
        DoRespawn();
    }
}
