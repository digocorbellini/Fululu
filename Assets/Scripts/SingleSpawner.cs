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
        GameObject obj = Instantiate(entity, transform.position, Quaternion.identity);
        obj.GetComponentInChildren<EntityHitbox>().OnDestroyed += DoRespawn;
    }

    private void DoRespawn()
    {
        StopAllCoroutines();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Debug.Log("Respawning...");
        yield return new WaitForSecondsRealtime(spawnInterval);
        GameObject obj = Instantiate(entity.gameObject, transform.position, Quaternion.identity);
        obj.GetComponentInChildren<EntityHitbox>().OnDeath += DoRespawn;
    }
}
