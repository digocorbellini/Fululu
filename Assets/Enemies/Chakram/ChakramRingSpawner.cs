using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramRingSpawner : MonoBehaviour
{

    public GameObject ringPrefab;
    public Transform spawnPoint;
    public float spawnTimer = 8.0f;
    public float launchAfter = 4.0f;

    private ChakramRing ring;
    void Start()
    {
        StartCoroutine(SpawnCycle(.1f, launchAfter));
    }

    IEnumerator SpawnCycle(float spawn, float launch)
    {
        // Spawn after X seconds
        yield return new WaitForSecondsRealtime(spawn);
        GameObject obj = Instantiate(ringPrefab, spawnPoint);
        ring = obj.GetComponent<ChakramRing>();

        // Launch projectile Y seconds
        yield return new WaitForSecondsRealtime(launch);
        ring.PreLaunch();

        StartCoroutine(SpawnCycle(spawnTimer, launchAfter));

        yield return null;
    }
}
