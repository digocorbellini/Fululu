using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class AOECircle : MonoBehaviour
{
    [SerializeField] private GameObject redCircleObject;
    [SerializeField] private GameObject redRingObject;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private BulletBase projectile;
    [SerializeField] private Transform bulletSpawn;

    [Tooltip("Small difference between ground circle and actual collider size")]
    public float colliderRadiusDiff = 0.05f;
    
    private float attackRadius;
    private float radiusGrowthSpeed;
    private float chargeTime;

    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        GameManager.instance.OnReset += OnReset;
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;
    }

    private void OnReset()
    {
        GameManager.instance.OnReset -= OnReset;
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.OnReset -= OnReset;
    }

    /// <summary>
    /// Set the stats for this attack
    /// </summary>
    /// <param name="chargeTime">the time this attack should wait before
    /// raining on the player</param>
    public void SetStats(float chargeTime, float attackRadius, int orderInLayer = 10, float bulletSpawnHeight = -1f)
    {
        this.attackRadius = attackRadius;
        this.chargeTime = chargeTime;

        // determine the growth speed of the red damage circle
        radiusGrowthSpeed = attackRadius / chargeTime;

        // set the radius of the collider to the attack radius
        capsuleCollider.radius = attackRadius - colliderRadiusDiff;

        // set the size of the red ring on the floor to match the hitbox radius
        redRingObject.transform.localScale = Vector3.one * (attackRadius * 2);

        // set ring rendering layer order
        redCircleObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderInLayer;
        redRingObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderInLayer;

        if (bulletSpawnHeight > 0)
        {
            Vector3 bulletPos = bulletSpawn.position;
            bulletPos.y = bulletSpawnHeight;
            bulletSpawn.position = bulletPos;
        }
    }

    /// <summary>
    /// Should be called after stats are set. Will perform the attack/
    /// </summary>
    public void BeginAttack()
    {
        StartCoroutine(performAttack());
    }

    private IEnumerator performAttack()
    {
        redRingObject.SetActive(true);
        redCircleObject.SetActive(true);
        redCircleObject.transform.localScale = Vector3.zero;

        // enable capsule collider for grazing
        capsuleCollider.enabled = true;

        // grow red circle indicator if charge time is not insignificant
        if (chargeTime > 0.1)
        {
            while (redCircleObject.transform.localScale.x < (attackRadius * 2))
            {
                Vector3 newScaleVal = Vector3.one * (radiusGrowthSpeed * Time.deltaTime);
                redCircleObject.transform.localScale += newScaleVal;

                yield return null;
            }
        }
        redCircleObject.transform.localScale = Vector3.one * (attackRadius * 2);

        // start performing attack
        BulletBase spawnedProjectile = Instantiate(projectile, bulletSpawn.position, Quaternion.LookRotation(Vector3.down));
        smokeParticles.Play();

        // dissale capsule collider for grazing
        capsuleCollider.enabled = false;

        // yield return new WaitForSeconds(bulletSpawn.position.y / spawnedProjectile.Speed);

        // wait for projectile to get destoryed
        while (spawnedProjectile != null && !isDestroyed(spawnedProjectile.gameObject))
        {
            yield return null;
        }

        selfDestruct();
    }

    /// <summary>
    /// Checks if a GameObject has been destroyed.
    /// </summary>
    /// <param name="gameObject">GameObject reference to check for destructedness</param>
    /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
    private bool isDestroyed(GameObject gameObject)
    {
        // UnityEngine overloads the == opeator for the GameObject type
        // and returns null when the object has been destroyed, but 
        // actually the object is still there but has not been cleaned up yet
        // if we test both we can determine if the object has been destroyed.
        return gameObject == null && !ReferenceEquals(gameObject, null);
    }

    /// <summary>
    /// Destroy this object
    /// </summary>
    private void selfDestruct()
    {
        Destroy(this.gameObject);
    }
}
