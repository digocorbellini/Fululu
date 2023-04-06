using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleOnEntry : MonoBehaviour
{
    public GameObject[] enableOnEntry;
    public GameObject[] disableOnEntry;

    private bool alreadyTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !alreadyTriggered)
        {
            enableOnEntry.ToList().ForEach(obj => obj.SetActive(true));
            disableOnEntry.ToList().ForEach(obj => obj.SetActive(false));

            alreadyTriggered = true;
        }
    }
}
