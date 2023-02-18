using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Bullet" for explosions whose hitbox lasts a short
// time but has FX that last a lot longer
public class ExplosionBullet : BulletBase
{
    [Space(10)]
    public float explosionLifetime = .2f;

    // Start is called before the first frame update
    void Start()
    {
        hasDefaultBehaviour = false;
    }

    private void Awake()
    {
        GameManager.instance.OnReset += OnReset;
        StartCoroutine(runLifetime());
        StartCoroutine(DisableHitbox());
        hitbox = GetComponent<AttackHitbox>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // pass
    }

    private IEnumerator DisableHitbox()
    {
        yield return new WaitForSecondsRealtime(explosionLifetime);
        GetComponentInChildren<Collider>().enabled = false;
        yield return null;
    }
}
