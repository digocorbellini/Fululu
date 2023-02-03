using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeZone : MonoBehaviour
{
    public PlayerController player;
    private bool alreadyChecked;

    // Start is called before the first frame update
    void Start()
    {
        
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
        }
    }

    private void FixedUpdate()
    {
        alreadyChecked = false;
    }
}
