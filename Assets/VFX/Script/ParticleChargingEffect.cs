using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChargingEffect : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float forceMagnitude = 10.0f;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }

    void LateUpdate()
    {
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 forceDirection = (transform.position - particles[i].position).normalized;
            Vector3 force = forceDirection * forceMagnitude;
            particles[i].velocity += force * Time.deltaTime;
        }

        particleSystem.SetParticles(particles, numParticlesAlive);
    }
}

