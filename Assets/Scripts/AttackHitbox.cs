using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hitbox for attacks (projectiles and explosions)
public class AttackHitbox : MonoBehaviour
{
    public float damage = 1;
    public bool isExplosive = false;
    public bool isStun = false;
    public bool destroyOnHit = true;

    private List<EntityHitbox> alreadyHit;

    private void Start()
    {
        alreadyHit = new List<EntityHitbox>();
    }
    public void addToHit(EntityHitbox c)
    {
        alreadyHit.Add(c);
    }

    public bool canHit(EntityHitbox c)
    {
        return !alreadyHit.Contains(c);
    }

    public void ResetAlreadyHit()
    {
        alreadyHit.Clear();
    }
}