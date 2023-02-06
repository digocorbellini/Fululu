using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeZone : MonoBehaviour
{
    public PlayerController player;
    public ParticleSystem grazeSparks;
    private ParticleSystem.EmissionModule emission;

    private bool alreadyChecked;
    private bool isGrazing;

    // Start is called before the first frame update
    void Start()
    {
        emission = grazeSparks.emission;
        emission.rateOverTimeMultiplier = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Physics.OverlapSphere will not work as attacks are triggers
    // and OverlapSphere only checks for colliders
    // OnTriggerEnter / Exit or unreliable as projectiles may be destroyed
    // while inside the graze zone and not exit
    private void OnTriggerStay(Collider other)
    {
        // Only check once a physics frame
        if (!alreadyChecked)
        {
            player.chargeGraze(Time.fixedDeltaTime);
            alreadyChecked = true;
            emission.rateOverTimeMultiplier = 15;

            grazeSparks.transform.position = other.ClosestPoint(transform.position);
        }
    }

    private void FixedUpdate()
    {
        alreadyChecked = false;
        emission.rateOverTimeMultiplier = 0;
    }
}
