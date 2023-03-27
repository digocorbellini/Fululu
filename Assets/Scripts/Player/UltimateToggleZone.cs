using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UltimateToggleZone : MonoBehaviour
{
    [Tooltip("Whether the player should be able to use their ultimate or not upon entering this trigger")]
    public bool enableOnEnter = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().canUseSacrifice = enableOnEnter;
        }
    }
}
