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
    [Tooltip("Particles that spawn if anything is hit. Overriden by enemy hit particles if hit enemy.")]
    public GameObject hitParticles;
    [Tooltip("OPTIONAL. Particles that spawn on collision with enemy")]
    public GameObject enemyHitParticles;
    public Transform hitParticlesSpawn;

    [Tooltip("If true, bullet will die after given lifetime and will move forward forever.")]
    public bool hasDefaultBehaviour = true;
    private bool didTimeOut = false;
    private bool hitTerrain = false;

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
            hitTerrain = true;
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
        GameObject particlesToSpawn;

        if (hitTerrain)
        {
            particlesToSpawn = hitParticles;
        }
        else
        {
            particlesToSpawn = (enemyHitParticles != null) ? enemyHitParticles : hitParticles;
        }

        if (particlesToSpawn != null)
        {
            if (hitParticlesSpawn != null)
            {
                Instantiate(particlesToSpawn, hitParticlesSpawn.position, Quaternion.LookRotation(hitParticlesSpawn.forward));
            }
            else
            {
                Instantiate(particlesToSpawn, transform.position, Quaternion.identity);
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
