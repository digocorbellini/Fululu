using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Particle = UnityEngine.ParticleSystem.Particle;

public class AfterImageSpawner : MonoBehaviour
{
    [SerializeField] private ParticleLifetimeEvents _lifetimeEvents;
    [SerializeField] private AfterImage afterImagePrefab;
    [SerializeField] private SkinnedMeshRenderer meshToCopy;

    // Start is called before the first frame update
    void Start()
    {
        _lifetimeEvents.ParticleWasBorn += SpawnAfterImage;
    }

    void SpawnAfterImage(Particle particle)
    {
        AfterImage afterImage = Instantiate(afterImagePrefab);
        afterImage.particle = particle;
        afterImage.meshToCopy = meshToCopy;
    }

    private void Reset()
    {
        _lifetimeEvents = GetComponent<ParticleLifetimeEvents>();
    }
}
