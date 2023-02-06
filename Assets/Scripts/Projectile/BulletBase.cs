using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Base for a projectiles
// Extend this class if your project needs custom behavior (i.e. seeking)
// Or use the GenericBullet if it doesn't
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AttackHitbox))]
public abstract class BulletBase: MonoBehaviour
{
    public float Speed;
    protected AttackHitbox hitbox;
    protected Rigidbody rb;
    public float lifetime;

    public GameObject[] trails;
    public GameObject hitParticles;

    [Tooltip("If true, bullet will die after given lifetime and will move forward forever and destroy on trigger enter.")]
    public bool hasDefaultBehaviour = true;

    public void SetDamage(float dmg)
    {
        if(hitbox == null)
        {
            hitbox = GetComponent<AttackHitbox>();
        }

        hitbox.damage = dmg;
    }

    private void Awake()
    {
        StartCoroutine(runLifetime());
    }

    private void Update()
    {
        transform.position = transform.position + transform.forward * Speed * Time.deltaTime;
    }

    private IEnumerator runLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        print("lifetime ran out!");
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitParticles != null)
        {
            var pos = other.ClosestPoint(transform.position);
            Instantiate(hitParticles, pos, Quaternion.identity);
        }

        bool shouldDestroy = false;

        if (hitbox != null && hitbox.destroyOnHit)
        {
            // Attack is single hit
            print("attack is single hit!");
            shouldDestroy = true;
        }
            
        if(other.gameObject.layer != 7 && other.gameObject.layer != 6 && other.gameObject.layer != 11)
        {
            // Piercing bullet hit terrain. Destroy it regardless;
            print("hit terrain!");
            shouldDestroy = true;
        }

        if (shouldDestroy)
        {
            // Make sure trails get to play to end before destroying
            foreach (GameObject obj in trails)
            {
                obj.transform.SetParent(null, true);
                Destroy(obj, 1f);
            }
            print("Should destory is true!");
            Destroy(this.gameObject);
        }
            
    }
}
