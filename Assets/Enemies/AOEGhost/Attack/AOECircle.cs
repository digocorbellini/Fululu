using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AttackHitbox))]
[RequireComponent(typeof(CapsuleCollider))]
public class AOECircle : MonoBehaviour
{
    [SerializeField] private GameObject redCircleObject;
    [SerializeField] private GameObject redRingObject;
    
    private float rainDuration;
    private float attackRadius;
    private float radiusGrowthSpeed;

    private ParticleSystem rainParticles;
    private CapsuleCollider capsuleCollider;
    private AttackHitbox hitbox;

    private void Awake()
    {
        rainParticles = GetComponent<ParticleSystem>();
        rainParticles.Stop();

        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;

        hitbox = GetComponent<AttackHitbox>();
        hitbox.destroyOnHit = false;
    }

    /// <summary>
    /// Set the stats for this attack
    /// </summary>
    /// <param name="chargeTime">the time this attack should wait before
    /// raining on the player</param>
    /// <param name="damage">the damage that the rain should do</param>
    /// <param name="rainDuration">the duration of the rain</param>
    public void SetStats(float chargeTime, float damage, float rainDuration, float attackRadius)
    {
        hitbox.damage = damage;
        this.rainDuration = rainDuration;
        this.attackRadius = attackRadius;

        // determine the growth speed of the red damage circle
        radiusGrowthSpeed = attackRadius / chargeTime;

        // set the radius of the particle rain to the damage circle
        var shape = rainParticles.shape;
        shape.radius = attackRadius;

        // set the radius of the collider to the attack radius
        capsuleCollider.radius = attackRadius;

        // set the size of the red ring on the floor to match the hitbox radius
        redRingObject.transform.localScale = Vector3.one * (attackRadius * 2);
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
        
        // grow red circle indicator
        while (redCircleObject.transform.localScale.x < (attackRadius * 2))
        {
            Vector3 newScaleVal = Vector3.one * (radiusGrowthSpeed * Time.deltaTime);
            redCircleObject.transform.localScale += newScaleVal;

            yield return null;
        }

        // start performing attack
        rainParticles.Play();
        capsuleCollider.enabled = true;

        yield return new WaitForSeconds(rainDuration);

        // stop attack
        rainParticles.Stop();

        // wait for particles to fall
        yield return new WaitForSeconds(1);
        capsuleCollider.enabled = false;

        // TODO: maybe fade out red circles

        selfDestruct();
    }

    /// <summary>
    /// Destroy this object
    /// </summary>
    private void selfDestruct()
    {
        Destroy(this.gameObject);
    }
}
