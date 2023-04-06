using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlamethrowerSpin : MonoBehaviour
{
    
    public float angularVelocity = 40f;

    private bool isSpinning;
    private Collider[] colliders;
    private ParticleSystem[] ps;

    public void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        ps = GetComponentsInChildren<ParticleSystem>();

        Debug.Log("C: " + colliders);
        Debug.Log("PS: " + ps);
    }
    private void Update()
    {
        if (isSpinning)
        {
            transform.eulerAngles += new Vector3(0, angularVelocity * Time.deltaTime, 0);
        }
    }

    public void StopSpinning()
    {
        ps.ToList().ForEach(p => p.Stop());
        colliders.ToList().ForEach(c => c.enabled = false);
        isSpinning = false;
        Destroy(gameObject, 5f);
    }

    public void StartSpinning()
    {
        colliders.ToList().ForEach(collider => collider.enabled = true);
        isSpinning = true;
    }
}
