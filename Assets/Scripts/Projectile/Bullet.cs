using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AttackHitbox))]
public abstract class Bullet : MonoBehaviour
{
    public float Speed;
    protected AttackHitbox hitbox;
    protected Rigidbody rb;
    public float lifetime;

    public GameObject[] trails;
    public GameObject hitParticles;

    private void Awake()
    {
        StartCoroutine(runLifetime());
    }

    private IEnumerator runLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit1");
        if (hitParticles != null)
        {
            var pos = other.ClosestPoint(transform.position);
            Instantiate(hitParticles, pos, Quaternion.identity);
        }

        bool shouldDestroy = false;

        if (hitbox != null && hitbox.destroyOnHit)
        {
            // Non-piercing bullet hit something. Destroy it
            shouldDestroy = true;
        }
            
        if(other.gameObject.layer != 7 || other.gameObject.layer != 6)
        {
            // Piercing bullet hit terrain. Destroy it regardless;
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
            Destroy(this.gameObject);
        }
            
    }
}
