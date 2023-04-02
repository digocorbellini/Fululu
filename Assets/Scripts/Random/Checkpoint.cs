using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public GameObject playerSpawn;
    public Transform respawnPoint;
    public bool singleUse = false;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(alreadyTriggered && singleUse)
        {
            return;
        }

        if (other.gameObject.transform.root.CompareTag("Player"))
        {
            playerSpawn.transform.position = respawnPoint.position;
            playerSpawn.transform.forward = respawnPoint.forward;
            alreadyTriggered = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(respawnPoint.position, respawnPoint.forward * 3f);
    }
}
