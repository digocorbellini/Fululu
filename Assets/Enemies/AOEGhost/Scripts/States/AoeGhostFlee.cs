using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeGhostFlee : AoeGhostState
{
    public float speed = 5;
    public float rotationSpeed = 15;
    public float obstacleDetectionRange = 1;
    public float heightOffset = 1;
    public LayerMask layersToIgnore;
    public float maxFleeTime = 10;
    private bool shouldCapFleeTime = true;

    private Transform player;
    private Vector3 movementDirection;
    private LayerMask raycastLayers;
    private float timer;

    public override string getStateName()
    {
        return "AOEFlee";
    }

    public override void enter()
    {
        timer = 0;

        controller.StopAttacking();
        controller.rb.velocity = Vector3.zero;

        player = controller.player.transform;

        movementDirection = player.position - controller.transform.position;
        movementDirection.y = 0;
        movementDirection.Normalize();
        movementDirection *= -1;

        raycastLayers = ~layersToIgnore.value;
    }

    public override void run()
    {
        // keep running away from player until they are out of player attack range
        if (Vector3.Distance(controller.transform.position, player.position) < controller.detectionRadius)
        {
            // reflect off of obstacles
            RaycastHit hit; 
            if (Physics.Raycast(controller.transform.position + new Vector3(0, heightOffset, 0), 
                controller.transform.forward, out hit, obstacleDetectionRange, raycastLayers))
            {
                movementDirection = Vector3.Reflect(movementDirection, hit.normal);
                movementDirection.y = 0;
                movementDirection.Normalize();
            }

            controller.rb.velocity = movementDirection * speed;

            // face movement direction
            controller.transform.forward = Vector3.RotateTowards(controller.transform.forward, movementDirection, rotationSpeed, 0.0f);
        }
        else
        {
            controller.switchState("AOEIdle");
        }

        if (shouldCapFleeTime)
        {
            if (timer > maxFleeTime)
            {
                controller.switchState("AOEIdle");
            }

            timer += Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + new Vector3(0, heightOffset, 0), transform.forward * obstacleDetectionRange);
    }
}
