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

    public float iFrameTime = .5f;

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;
    public void CallOnDeath() => OnDeath?.Invoke();

    public delegate void HurtEvent(float damage, bool isExplosive = false);
    public event HurtEvent OnHurt;
    public void CallOnHurt(float damage, bool isExplosive) => OnHurt?.Invoke(damage, isExplosive);

    public delegate void ArmorBreakEvent();
    public event ArmorBreakEvent OnArmorBreak;
    public void CallOnArmorBreak() => OnArmorBreak?.Invoke();

    public delegate void StunEvent();
    public event ArmorBreakEvent OnStun;
    public void CallOnStun() => OnStun?.Invoke();

    private int targetLayer;
    public float health;
    private bool isIframe = false;
    private bool alreadyDead = false;

    private void Start()
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
        print(other.gameObject + " on " + other.gameObject.layer);
        if (!isIframe && other.gameObject.layer == targetLayer)
        {
            if (other.gameObject.TryGetComponent(out AttackHitbox attack))
            {

                print(gameObject + " hit by " + attack.gameObject);

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
                        print("Attack Negated!");
                        return;
                    }
                    else
                    {
                        armorPoints -= (int) attack.damage;
                        if(armorPoints <= 0)
                        {
                            CallOnArmorBreak();
                            print("Armor broken!");
                        }
                    }
                }

                if (armorPoints <= 0 && attack.isStun)
                {
                    CallOnStun();
                    return;
                }

                health -= attack.damage;

                if (!alreadyDead)
                {
                    if (health <= 0)
                    {
                        CallOnDeath();
                        alreadyDead = true;
                    }
                    else
                    {
                        CallOnHurt(attack.damage, attack.isExplosive);
                    }
                }

                isIframe = true;
                StartCoroutine(DisableIFrame());

                if (attack.destroyOnHit)
                {
                    Destroy(attack.gameObject);
                }
            }
        }
    }

    IEnumerator DisableIFrame()
    {
        yield return new WaitForSecondsRealtime(iFrameTime);

        isIframe = false;

        yield return null;
    }

    public void ForceDisableIFrame()
    {
        StopAllCoroutines();
        isIframe = false;
    }


}
