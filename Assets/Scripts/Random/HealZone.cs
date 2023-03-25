using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    public int healAmount;
    public int healthThreshold;

    private bool alreadyTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !alreadyTriggered)
        {
            EntityHitbox player = other.gameObject.GetComponentInChildren<EntityHitbox>();
            if(player && player.health <= healthThreshold)
            {
                player.DealDamageDirect(healAmount * -1);
                alreadyTriggered = true;
            }
        }
    }
}
