using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    public Rigidbody rb;
    public Collider c;
    public List<GameObject> invisibleWalls = new List<GameObject>();
    public GameObject particles;
    public float gravity = 20f;

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

            Instantiate(particles, transform.position, Quaternion.identity);

            // enable invisible walls
            foreach(GameObject x in invisibleWalls)
            {
                x.SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!done || rb == null)
            return;

        rb.AddForce(Vector3.down * gravity * rb.mass);
    }
}
