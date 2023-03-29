using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    public Rigidbody rb;
    public Collider c;

    private bool done;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !done)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            c.enabled = false;
            Destroy(rb.gameObject, 30);
            done = true;
        }
    }
}
