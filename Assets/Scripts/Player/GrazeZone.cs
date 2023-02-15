using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeZone : MonoBehaviour
{
    public PlayerController player;
    public ParticleSystem grazeSparks;

    public AudioSource audioSource;
    public AudioClip charge33;
    public AudioClip charge66;
    public AudioClip charge100;

    private ParticleSystem.EmissionModule emission;
    private bool alreadyChecked;
    private int chargeState;
    private float chargeAmount;
    

    // Start is called before the first frame update
    void Start()
    {
        emission = grazeSparks.emission;
        emission.rateOverTimeMultiplier = 0;

        chargeState = 0;
        chargeAmount = 0.0f;
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
            chargeAmount = player.ChargeGraze(Time.fixedDeltaTime);

            // Figure out what sound clip to play if any

            if(chargeState == 0 && chargeAmount >= .33)
            {
                audioSource.PlayOneShot(charge33);
                chargeState = 1;
            }
            else if(chargeState == 1 && chargeAmount >= .66)
            {
                audioSource.PlayOneShot(charge66);
                chargeState = 2;
            }
            else if(chargeState == 2 && chargeAmount >= 1.0)
            {
                audioSource.PlayOneShot(charge100);
                chargeState = 3;
            }
            


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

    public void Reset()
    {
        chargeAmount = 0.0f;
        chargeState = 0;
    }
}
