using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Movement : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public float fixedAngle = 45.0f;

    void Update() {
        float rotationAmount = Input.GetAxis("Horizontal") * rotationSpeed;
        transform.rotation = Quaternion.Euler(0, fixedAngle + rotationAmount);
    }
}
