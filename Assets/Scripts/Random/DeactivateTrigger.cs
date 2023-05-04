using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateTrigger : MonoBehaviour
{
    public GameObject[] objectsToDeactivate;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnReset += this.Reset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (GameObject obj in objectsToDeactivate)
            {
                obj.SetActive(false);
            }
        }
    }

    private void Reset()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(true);
        }
    }
}
