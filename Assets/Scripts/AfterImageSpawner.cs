using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Particle = UnityEngine.ParticleSystem.Particle;

public class AfterImageSpawner : MonoBehaviour
{
    [SerializeField] private AfterImage afterImagePrefab;
    [SerializeField] private Transform modelToCopy;
    [SerializeField] private float emissionOverDistance = 1.0f;
    [SerializeField] private float emissionPer360Degrees = 0.0f;
    private float distanceTraveled = 0.0f;
    private float degreesRotated = 0.0f;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        distanceTraveled += Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;

        degreesRotated += Quaternion.Angle(lastRotation, transform.rotation);
        lastRotation = transform.rotation;

        Debug.Log("distance: " + distanceTraveled);

        bool spawnImage = false;

        if (distanceTraveled * emissionOverDistance >= 1.0f)
        {
            spawnImage = true;
            distanceTraveled -= 1.0f / emissionOverDistance;
        }

        if ((degreesRotated / 360.0f) * emissionPer360Degrees >= 1.0f)
        {
            spawnImage = true;
            degreesRotated -= 360.0f / emissionPer360Degrees;
        }

        if (spawnImage)
        {
            SpawnAfterImage();
        }
    }

    private void OnEnable()
    {
        distanceTraveled = 0.0f;
        degreesRotated = 0.0f;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void SpawnAfterImage()
    {
        AfterImage afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        afterImage.transform.localScale = transform.lossyScale;
        afterImage.modelToCopy = modelToCopy;
    }
}
