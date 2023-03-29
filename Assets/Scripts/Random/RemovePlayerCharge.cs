using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlayerCharge : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.player;
            bool success = player.UseCharge(1f, true);
            Destroy(this);
        }
    }
}
