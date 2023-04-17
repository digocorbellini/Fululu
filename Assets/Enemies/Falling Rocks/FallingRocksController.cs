using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FallingRocksController : MonoBehaviour
{
    [Tooltip("zone which falling rocks will spawn from")]
    public Vector3 rockSpawningArea;
    public Transform rockSpawnCenter;
    public AOECircle fallingRockObject;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    [Header("Attack stats")]
    public float secsBetweenSpawnPhases = 1f;
    public int rocksPerSpawnPhase = 3;
    public float circleGrowthTime = 3;
    public float circleRadius = 2;

    private Coroutine rockRoutine;

    private void Start()
    {
        if (rockRoutine != null)
            StopCoroutine(rockRoutine);
        rockRoutine = StartCoroutine(spawnRocks());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        // start spawning rocks
        if (rockRoutine != null)
            StopCoroutine(rockRoutine);
        rockRoutine = StartCoroutine(spawnRocks());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        // stop spawning rocks
        if (rockRoutine != null)
            StopCoroutine(rockRoutine);
    }

    private IEnumerator spawnRocks()
    {
        while (true)
        {
            for (int i = 0; i < rocksPerSpawnPhase; i++)
            {
                Vector3 halfedSpawnArea = rockSpawningArea / 2;
                float spawnX = Random.Range(-halfedSpawnArea.x, halfedSpawnArea.x);
                float spawnY = Random.Range(-halfedSpawnArea.y, halfedSpawnArea.y);
                float spawnZ = Random.Range(-halfedSpawnArea.z, halfedSpawnArea.z);

                Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);
                spawnPos += rockSpawnCenter.position;
                float distanceToFloor = getDeltaToFloor(spawnPos);
                spawnPos.y -= distanceToFloor;

                AOECircle currentRock = Instantiate(fallingRockObject, spawnPos, Quaternion.identity);
                currentRock.SetStats(circleGrowthTime, circleRadius, 10, spawnY + rockSpawnCenter.position.y);
                currentRock.BeginAttack();

                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(secsBetweenSpawnPhases);
        } 
    }

    private float getDeltaToFloor(Vector3 pos)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(pos, Vector3.down, out hitInfo, 100f, groundLayer))
        {
            return hitInfo.distance;
        }

        print("No ground hit");
        return 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rockSpawnCenter.position, rockSpawningArea);
    }
}
