using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGhostOnExit : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning("Exiting");
        EntityHitbox hitbox = other.transform.parent.GetComponentInChildren<EntityHitbox>();
        if(hitbox)
        {
            if (!hitbox.isPlayer)
            {
                hitbox.DealDamageDirect(1000);
            }
        }
    }
}
