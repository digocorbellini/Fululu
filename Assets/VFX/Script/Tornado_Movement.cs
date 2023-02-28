using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Movement : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    //public float fixedAngle = 45.0f;

    private Vector3 rotationAxis;

    private void Start()
    {
        rotationAxis = transform.forward;
    }

    void Update() {
        float rotationAmount = Input.GetAxis("Horizontal") * rotationSpeed;
        //transform.rotation = Quaternion.Euler(0, fixedAngle + rotationAmount, 0f);
        transform.RotateAround(transform.position, rotationAxis, rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 5);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right * 5);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up * 5);
    }
}
