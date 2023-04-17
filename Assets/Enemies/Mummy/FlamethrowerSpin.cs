using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlamethrowerSpin : MonoBehaviour
{
    
    public float angularVelocity = 40f;

    private bool isSpinning;
    private Collider[] colliders;
    private DecalProjector[] decals;
    private ParticleSystem[] ps;

    private bool isDestroyed = false;
    public void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        decals = GetComponentsInChildren<DecalProjector>();
        ps = GetComponentsInChildren<ParticleSystem>();

        GameManager.instance.OnReset += OnReset;
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
        decals.ToList().ForEach(d => d.enabled = false);
        isSpinning = false;
        GameManager.instance.OnReset -= OnReset;
        isDestroyed = true;
        Destroy(gameObject, 5f);
    }

    public void StartSpinning()
    {
        colliders.ToList().ForEach(collider => collider.enabled = true);
        decals.ToList().ForEach(d => d.enabled = true);
        isSpinning = true;
    }

    public void OnReset()
    {
        if (!isDestroyed)
        {
            Destroy(gameObject);
        }
    }
}
