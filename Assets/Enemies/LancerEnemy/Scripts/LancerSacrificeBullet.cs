using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerSacrificeBullet : BulletBase
{
    [Header("Stats")]
    [SerializeField] private float bulletSpawnHeight;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private int numSpearsPerEnemy = 3;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private int maxTargets = 10;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [Header("Timing")]
    [SerializeField] private float timeBetweenAttacks = .5f;
    [SerializeField] private float timeBetweenEnemies = .1f;
    [SerializeField] private float initialShotDuration = 1f;
    [SerializeField] private float attackChargeTime;
    [Header("References")]
    [SerializeField] private BulletBase initialProjectile;
    [SerializeField] private AOECircle aoeCircleObject;
    [SerializeField] private GameObject attackRingObject;

    private Transform player;

    // keep track of targets we have finshed attacking
    private int numTargets;
    private int _numTargetsDone = 0;
    private int numTargetsDone
    {
        get
        {
            return _numTargetsDone;
        }
        set
        {
            _numTargetsDone = value;
            if (_numTargetsDone == numTargets)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        attackRingObject.transform.up = Vector3.up;
        attackRingObject.transform.localScale = Vector3.one * (detectionRadius * 2);

        // find all enemy targets within detection sphere 
        Collider[] targets = Physics.OverlapSphere(player.position, detectionRadius, enemyLayer);
        if (targets.Length > 0)
        {
            numTargets = (targets.Length > maxTargets) ? maxTargets : targets.Length;
            print("lancer sacrifice found " + numTargets + " enemies");
            StartCoroutine(shootSpearsUpward(targets));
        }
        else
        {
            print("lancer sacrifice found no enemies");
            Destroy(this.gameObject);
        }
    }

    // Shoot spears upward to telegraph attack
    private IEnumerator shootSpearsUpward(Collider[] targets)
    {
        float timeBetweenEnemies = initialShotDuration / numTargets;
        float timeBetweenShots = timeBetweenEnemies / numSpearsPerEnemy;

        // shoot a projectile up for each enemy
        for (int i = 0; i < numTargets; i++)
        {
            for (int x = 0; x < numSpearsPerEnemy; x++)
            {
                Instantiate(initialProjectile, player.position + Vector3.up * bulletSpawnHeight, Quaternion.LookRotation(Vector3.up));

                yield return new WaitForSeconds(timeBetweenShots);
            }
        }

        StartCoroutine(startAttack(targets));
    }

    private IEnumerator startAttack(Collider[] targets)
    {
        for (int i = 0; i < numTargets; i++)
        {
            StartCoroutine(individualAttack(targets[i]));

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

    private IEnumerator individualAttack(Collider target)
    {
        // start spawning attacks
        for (int i = 0; i < numSpearsPerEnemy; i++)
        {
            Vector3 spawnLocation = target.gameObject.transform.position;
            spawnLocation.y -= getDeltaToFloor(spawnLocation);

            AOECircle currentAttack = Instantiate(aoeCircleObject, spawnLocation, Quaternion.identity);
            currentAttack.SetStats(attackChargeTime, attackRadius);
            currentAttack.BeginAttack();

            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        numTargetsDone++;
    }

    /// <summary>
    /// Get the height of the given point off of the floor.
    /// </summary>
    /// <param name="pos">The point whose height we are interested in</param>
    /// <returns>Get the height of the given point off of the floor.</returns>
    private float getDeltaToFloor(Vector3 pos)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(pos, Vector3.down, out hitInfo, 100f, groundLayer))
            return hitInfo.distance;

        print("No ground hit");
        return 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (player)
            Gizmos.DrawWireSphere(player.position, detectionRadius);
        else
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
