using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingPuddle : MonoBehaviour
{
    private PlayerController player = null;
    public float speedModifier = .5f;
    public bool blockDash = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.CompareTag("Player"))
        {
            TryGetPlayer().ModifySpeed(speedModifier, blockDash);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.root.CompareTag("Player"))
        {
            TryGetPlayer().ModifySpeed(1.0f, false);
        }
    }

    private PlayerController TryGetPlayer()
    {
        if(player == null)
        {
            player = GameManager.instance.player;
        }

        return player;
    }
}
