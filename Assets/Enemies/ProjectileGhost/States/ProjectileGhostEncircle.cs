using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGhostEncircle : ProjectileGhostState
{
    public float maxDuration = 5;
    public float minDuration = 1;
    public float rotationSpeed = 20;
    public float moveSpeed = 4;
    public float movementThreshold = 1;

    private Transform player;
    private float originalDistance;
    [SerializeField]private float timeRemaining;

    public override string getStateName()
    {
        return "PGEncircle";
    }

    public override void enter()
    {
        player = controller.player.transform;
        controller.rb.velocity = Vector3.zero;

        originalDistance = Vector3.Distance(controller.transform.position, player.position);
        timeRemaining = Random.Range(minDuration, maxDuration);
    }

    public override void run()
    {
        if (timeRemaining < 0)
        {
            if (Vector3.Distance(controller.transform.position, controller.player.transform.position)
                > controller.MaxDistanceFromPlayer)
            {
                controller.switchState("PGChase");
            }
            else
            {
                controller.switchState("PGIdle");
            }
        }
        timeRemaining -= Time.deltaTime;

        // rotate around the player
        controller.transform.RotateAround(player.position, Vector3.up, rotationSpeed * Time.deltaTime);

        // stay the same distance away from the player for the duration of the rotation
        Vector3 directionToPlayer = player.position - controller.transform.position;
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();
        float newDistance = Vector3.Distance(controller.transform.position, player.position);
        float distanceDelta = newDistance - originalDistance;
        if (distanceDelta < 0 && Mathf.Abs(distanceDelta) > movementThreshold)
        {
            // move away from player if they are too close. The closer the player, the higher the velocity
            controller.rb.velocity = -directionToPlayer * moveSpeed * (Mathf.Abs(distanceDelta) / originalDistance);
            //print("AWAY factors: " + (Mathf.Abs(distanceDelta) / originalDistance));

        }
        else if (distanceDelta > 0 && Mathf.Abs(distanceDelta) > movementThreshold)
        {
            // move towards the player if they are too far. The closer the player, the lower the velocity

            controller.rb.velocity = directionToPlayer * moveSpeed * Mathf.Clamp((Mathf.Abs(distanceDelta) / (originalDistance / 2)), 0, 1f);
            //print("TOWARDS factor: " + Mathf.Clamp((Mathf.Abs(distanceDelta) / (originalDistance / 2)), 0, 1f));
        }
        else
        {
            controller.rb.velocity = Vector3.zero;
        }

        controller.transform.LookAt(controller.player.transform.position);
        Vector3 rot = controller.transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        controller.transform.eulerAngles = rot;
    }
}
