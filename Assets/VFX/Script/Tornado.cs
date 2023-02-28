using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : MonoBehaviour {
    public float rotationSpeed = 10.0f;

    void Update() {
        float rotationAmount = Input.GetAxis("Horizontal") * rotationSpeed;
        transform.Rotate(Vector3.up, rotationAmount);
    }
}

