using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hitbox for entities (not attacks). Most likely just enemies
public class EntityHitbox : MonoBehaviour
{
    public float maxHealth = 3.0f;

    [Tooltip("While something has armor, it is immune to normal damage. Each explosive damage removes 1 armor point")]
    public int armorPoints = 0;

    [Tooltip("Is this the player or not?")]
    public bool isPlayer = false;

    [Tooltip("How much charge to give on hit per damage")]
    public float chargeOnHit = 1;

    public float iFrameTime = 0f;

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;
    public void CallOnDeath() => OnDeath?.Invoke();

    public delegate void HurtEvent(float damage, bool isExplosive, Collider other);
    public event HurtEvent OnHurt;
    public void CallOnHurt(float damage, bool isExplosive, Collider other) => OnHurt?.Invoke(damage, isExplosive, other);

    public delegate void DestroyEvent();
    public event DestroyEvent OnDestroyed;
    public void CallOnDestroyed() => OnDestroyed?.Invoke();
    public bool wasReset = false;


    public delegate void ArmorBreakEvent();
    public event ArmorBreakEvent OnArmorBreak;
    public void CallOnArmorBreak() => OnArmorBreak?.Invoke();

    public delegate void StunEvent();
    public event ArmorBreakEvent OnStun;
    public void CallOnStun() => OnStun?.Invoke();

    public delegate void ShieldBreakEvent(bool timedOut);
    public event ShieldBreakEvent OnShieldBreak;
    public void CallOnShieldBreak(bool timedOut) => OnShieldBreak?.Invoke(timedOut);
    private Coroutine shieldBuff;
    private float shieldAmount;

    private int targetLayer;
    public float health;
    private bool isIframe = false;
    [HideInInspector] public bool alreadyDead = false;

    private Coroutine iFrameCoroutine;

    private void Awake()
    {
        health = maxHealth;
        if (isPlayer)
        {
            targetLayer = 9;
        }
        else
        {
            targetLayer = 8;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //print(other.gameObject + " on " + other.gameObject.layer);
        if (!isIframe && other.gameObject.layer == targetLayer)
        {
            if (other.gameObject.TryGetComponent(out AttackHitbox attack))
            {

                //print(gameObject + " hit by " + attack.gameObject);

                if (!attack.canHit(this))
                {
                    print("Can't hit it");
                    return;
                }

                attack.addToHit(this);

                if(armorPoints > 0)
                {
                    if (!attack.isExplosive)
                    {
                        // Attack negated by armor
                        TryDestroyAttack(attack);
                        return;
                    }
                    else
                    {
                        armorPoints -= (int) attack.damage;
                        if(armorPoints <= 0)
                        {
                            CallOnArmorBreak();
                        }
                    }
                }

                if (armorPoints <= 0 && attack.isStun)
                {
                    CallOnStun();
                    return;
                }

                float actualDamage = attack.damage;
                if(shieldAmount > 0)
                {
                  
                    if (!attack.isShieldPiercing)
                    {
                        // Non piercing attacks affected by shield
                        actualDamage -= shieldAmount;
                    }

                    shieldAmount -= attack.damage;

                    if(shieldAmount <= 0)
                    {
                        // Was shield broken?
                        CallOnShieldBreak(false);
                    }
                    if (actualDamage <= 0)
                    {
                        // Was attack negated by shield?
                        TryDestroyAttack(attack);
                        return;
                    }

                    Debug.LogWarning("Shield left: " + shieldAmount);
                }

                actualDamage = Mathf.Min(health, attack.damage);

                health -= actualDamage;
                Debug.Log("Hit! new health: " + health + " damage: " + attack.damage);

                if (!alreadyDead)
                {
                    if (health <= 0)
                    {
                        alreadyDead = true;
                        CallOnDeath();
                    }
                    else
                    {
                        CallOnHurt(actualDamage, attack.isExplosive, other);
                    }

                    GameManager.instance.OnHitGrazeCharge(chargeOnHit * actualDamage);
                }

                if(attack.damage > 0)
                {
                    isIframe = true;
                    StartCoroutine(DisableIFrame(iFrameTime));
                }
                TryDestroyAttack(attack);
                
            }
        }
    }

    // Pass in a negative value to heal instead
    public void DealDamageDirect(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        if(health <= 0)
        {
            alreadyDead = true;
            CallOnDeath();
        }
        else
        {
            CallOnHurt(damage, false, gameObject.transform.root.GetComponentInChildren<Collider>());
        }
    }

    private void TryDestroyAttack(AttackHitbox attack)
    {
        if (attack.destroyOnHit)
        {
            if (attack.gameObject.TryGetComponent<BulletBase>(out BulletBase bb))
            {
                bb.DetatchTrails();
            }

            Destroy(attack.gameObject);
        }
    }

    public void GrantShieldBuff(float amount, float time)
    {
        Debug.LogWarning("Giving " + amount + " shield for " + time + " seconds");
        if(shieldBuff != null)
        {
            StopCoroutine(shieldBuff);
        }

        shieldAmount = amount;
        shieldBuff = StartCoroutine(ShieldLifetime(time));
    }

    IEnumerator ShieldLifetime(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        if(shieldAmount > 0)
        {
            CallOnShieldBreak(true);
        }

        shieldAmount = 0;
    }
    IEnumerator DisableIFrame(float time)
    {
        if(iFrameTime != 0)
            yield return new WaitForSecondsRealtime(time);

        isIframe = false;

        yield return null;
    }

    public void GiveIFrames(float time)
    {
        ForceDisableIFrame();
        isIframe = true;
        iFrameCoroutine = StartCoroutine(DisableIFrame(time));
    }

    public void ForceDisableIFrame()
    {
        //StopAllCoroutines();
        if (iFrameCoroutine != null)
        {
            StopCoroutine(iFrameCoroutine);
        }

        isIframe = false;
    }

    public float HealthPercent()
    {
        return health / maxHealth;
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded || wasReset)
        {
            // Do nothing if being destroyed on scene closing clean up
            return;
        }

        CallOnDestroyed();
    }

}
