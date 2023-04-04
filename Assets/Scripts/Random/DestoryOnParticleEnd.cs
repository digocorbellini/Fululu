using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryOnParticleEnd : MonoBehaviour
{
    public ParticleSystem[] particles;

    // Update is called once per frame
    void Awake()
    {
        float maxLifetime = 0f;
        foreach (ParticleSystem p in particles)
        {
            float lifetime = particles[0].main.startLifetime.constantMax;
            if (lifetime > maxLifetime)
            {
                maxLifetime = lifetime;
            }
        }

        // destory this after all particle systems are done playing
        Destroy(this.gameObject, maxLifetime);
    }
}
