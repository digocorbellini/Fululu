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
    public Transform hitParticlesSpawn;

    [Tooltip("If true, bullet will die after given lifetime and will move forward forever.")]
    public bool hasDefaultBehaviour = true;
    private bool didTimeOut = false;

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
        GameManager.instance.OnReset += OnReset;
        if(hasDefaultBehaviour)
            StartCoroutine(runLifetime());
        hitbox = GetComponent<AttackHitbox>(); 
    }

    protected void OnReset()
    {
        GameManager.instance.OnReset -= OnReset;
        if (gameObject != null)
        {
            didTimeOut = true;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(hasDefaultBehaviour)
            transform.position = transform.position + transform.forward * Speed * Time.deltaTime;
    }

    protected IEnumerator runLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        didTimeOut = true;
        DetatchTrails();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool shouldDestroy = false;
            
        if(other.gameObject.layer == 0 || other.gameObject.layer == 12)
        {
            // Piercing bullet hit terrain. Destroy it regardless;
            // print("hit terrain: " + other.gameObject);
            shouldDestroy = true;
        }

        if (shouldDestroy)
        {             
            DetatchTrails();
            Destroy(this.gameObject);
        }
            
    }

    public void DetatchTrails()
    {
        foreach(GameObject obj in trails)
        {
            if(obj != null)
            {
                obj.transform.SetParent(null, true);
                Destroy(obj, 3f);
            }
        }
    }

    public void spawnHitParticle()
    {
        if (hitParticles != null)
        {
            if (hitParticlesSpawn != null)
            {
                Instantiate(hitParticles, hitParticlesSpawn.position, Quaternion.LookRotation(hitParticlesSpawn.forward));

            }
            else
            {
                Instantiate(hitParticles, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnDestroy()
    {
        
        if (!gameObject.scene.isLoaded)
        {
            // Do nothing if being destroyed on scene closing clean up
            return;
        }

        GameManager.instance.OnReset -= OnReset;

        if (!didTimeOut)
        {
            spawnHitParticle();
        }
    }
}
