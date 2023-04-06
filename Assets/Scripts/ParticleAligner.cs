using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    ParticleSystem ps;
    public bool StartRotation3D = false;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startRotation3D = StartRotation3D;
    }

    // Update is called once per frame
    void Update()
    {
        var main = ps.main;
        
        if (!StartRotation3D)
        {
            main.startRotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        } else
        {
            main.startRotationX = transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
            main.startRotationY = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            main.startRotationZ = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        }
    }
}
