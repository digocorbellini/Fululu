using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMove : MonoBehaviour
{
    public Rigidbody rb;

    private void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float horz = Input.GetAxis("Horizontal");

        if(Mathf.Abs(vert) > 0)
        {
            rb.AddForce(new Vector3(0, 0, vert) * 2f);
        }

        if (Mathf.Abs(horz) > 0)
        {
            rb.AddForce(new Vector3(horz, 0, 0) * 2f);
        }
    }
}
