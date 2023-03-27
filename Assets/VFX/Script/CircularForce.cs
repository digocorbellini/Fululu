using UnityEngine;

public class CircularForce : MonoBehaviour
{
    public float forceStrength = 1f;

    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }

    void LateUpdate()
    {
        int numParticles = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            Vector3 particlePosition = particles[i].position;
            Vector3 forceDirection = new Vector3(-particlePosition.y, particlePosition.x, 0f);
            forceDirection.Normalize();
            particles[i].velocity += forceDirection * forceStrength * Time.deltaTime;
        }

        particleSystem.SetParticles(particles, numParticles);
    }
}

