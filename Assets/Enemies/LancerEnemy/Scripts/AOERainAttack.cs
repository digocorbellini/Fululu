using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOERainAttack : MonoBehaviour
{
    [Header("Attack stats")]
    [SerializeField] private int numberOfAttacks;
    [SerializeField] private float attackRadius;
    [SerializeField] private bool shouldBeLeading;
    [SerializeField] private LayerMask groundLayer;
    [Header("Timing")]
    [Tooltip("The time the initial vertical shot should take")]
    [SerializeField] private float initialShotDuration = 1f;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float attackChargeTime;
    [Header("References")]
    [SerializeField] private AOECircle aoeCircleObject;
    [SerializeField] private BulletBase initialShotObject; // should have a trail and particles and rigidbody
    [SerializeField] private Transform rainLanceSpawn;

    private GameObject player;
    private PlayerController playerController;

    private bool isAttacking = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            playerController = player.GetComponent<PlayerController>();
    }

    /// <summary>
    /// Get the total time it takes to complete an attack
    /// </summary>
    /// <returns>The total time it takes to complete an attackS</returns>
    public float getTotalAttackTime()
    {
        return initialShotDuration + (numberOfAttacks * timeBetweenAttacks);
    }

    /// <summary>
    ///  Call this function in order to perform the rain attack. Can only call if
    ///  currently not attacking.
    /// </summary>
    public void attack()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        StartCoroutine(shootSpearsUpward());
    }

    // Shoot spears upward to telegraph attack
    private IEnumerator shootSpearsUpward()
    {
        float timeBetweenShots = initialShotDuration / numberOfAttacks;

        for (int i = 0; i < numberOfAttacks; i ++)
        {
            Instantiate(initialShotObject, rainLanceSpawn.position, Quaternion.LookRotation(Vector3.up));

            yield return new WaitForSeconds(timeBetweenShots);
        }

        StartCoroutine(startAttack());
    }

    private IEnumerator startAttack()
    {
        // TODO: shoot initial shot into the air

        yield return new WaitForSeconds(initialShotDuration);

        // start spawning attacks
        for (int i = 0; i < numberOfAttacks; i++)
        {
            Vector3 spawnLocation = getAttackSpawnLocation();

            AOECircle currentAttack = Instantiate(aoeCircleObject, spawnLocation, Quaternion.identity);
            currentAttack.SetStats(attackChargeTime, attackRadius);
            currentAttack.BeginAttack();

            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        isAttacking = false;
    }

    /// <summary>
    /// Gets the position of in which to spawn the attack. Will take into account whether
    /// the attack is leading or not and where the player is.
    /// </summary>
    /// <returns>The position in which to spawn the next attack</returns>
    private Vector3 getAttackSpawnLocation()
    {
        // if not leading, just get player's current position
        if (!shouldBeLeading)
        {
            Vector3 playerPos = player.transform.position;
            playerPos.y -= getDeltaToFloor(playerPos);

            return playerPos;
        }

        print("calculating leading shot!");

        Vector3 playerHorizVelocity = playerController.GetPlayerMoveDirection();
        playerHorizVelocity.y = 0;

        print("Player horiz velocity: " + playerHorizVelocity.magnitude);

        Vector3 playerDirection = playerHorizVelocity;
        playerDirection.Normalize();

        print("pos delta: " + (playerHorizVelocity * attackChargeTime));

        Vector3 leadingPos = player.transform.position + (playerDirection * playerController.GetMoveSpeed() * attackChargeTime) + (playerDirection * attackRadius * 2);
        leadingPos.y -= getDeltaToFloor(leadingPos);

        return leadingPos;
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
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
