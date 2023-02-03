using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOERainAttack : MonoBehaviour
{
    [Header("Attack stats")]
    [SerializeField] private int numberOfAttacks;
    [SerializeField] private float damage;
    [SerializeField] private float attackRadius;
    [SerializeField] private bool shouldBeLeading;
    [SerializeField] private LayerMask groundLayer;
    [Header("Timing")]
    [Tooltip("The time the initial vertical shot should take")]
    [SerializeField] private float initialShotDuration;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float attackChargeTime;
    [SerializeField] private float rainDuration;
    [Header("References")]
    [SerializeField] private AOECircle aoeCircleObject;
    [SerializeField] private GameObject initialShotObject; // should have a trail and particles and rigidbody

    private GameObject player;
    private PlayerController playerController;

    private bool attackTriggered = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            playerController = player.GetComponent<PlayerController>();
    }

    /// <summary>
    ///  Call this function in order to perform the rain attack. Can only call this once.
    /// </summary>
    public void attack()
    {
        if (attackTriggered)
            return;

        attackTriggered = true;
        // TODO:
        // - shoot initial shot into the air
        // - make shot dissapear
        // - start spawning rings around player location
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
            currentAttack.SetStats(attackChargeTime, damage, rainDuration, attackRadius);
            currentAttack.BeginAttack();

            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        // destroy this object
        selfDestruct();
    }

    /// <summary>
    /// Destroy this object
    /// </summary>
    private void selfDestruct()
    {
        Destroy(this.gameObject);
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

        // TODO: fix leading shot

        print("calculating leading shot!");

        Vector3 playerHorizVelocity = playerController.GetVelocity();
        playerHorizVelocity.y = 0;

        print("Player horiz velocity: " + playerHorizVelocity.magnitude);

        Vector3 playerDirection = playerHorizVelocity;
        playerDirection.Normalize();

        float playerHorizSpeed = playerHorizVelocity.magnitude;

        // get the distance the player would travel to get to the attack position in time for attack
        float leadingPosDelta = playerHorizSpeed * attackChargeTime;

        Vector3 leadingPos = player.transform.position + (playerHorizVelocity * leadingPosDelta);
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
