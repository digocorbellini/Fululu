using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotLeader : MonoBehaviour
{
    public Rigidbody target;
    public Transform shotOrigin;
    public float shotSpeed;

    public bool visualize = false;
    private Vector3 targetPrediction;

    public Vector3 CalcLead()
    {
        Vector3 shootVector;
        shootVector = CalcLeadStatic(target, shotOrigin, shotSpeed, out targetPrediction);
        return shootVector;
    }

    public static Vector3 CalcLeadStatic(Rigidbody target, Transform origin, float speed, out Vector3 prediction)
    {
        Transform targetPos = target.transform;

        // Make the assumption that distance will not change dramatically
        float timeToTarget = Vector3.Distance(origin.position, target.position) / speed;

        // Calculate where target will be
        Vector3 targetVel = target.velocity;
        targetVel.y = 0;
        Vector3 targetNextPos = targetPos.transform.position + (targetVel * timeToTarget);

        // Return direction to predicted position
        prediction = targetNextPos;
        return (targetNextPos - origin.position).normalized;
    }

    private void OnDrawGizmos()
    {
        if (visualize)
        {
            Vector3 shoot = CalcLeadStatic(target, shotOrigin, shotSpeed, out targetPrediction);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(target.position, targetPrediction);
            Gizmos.DrawSphere(targetPrediction, .3f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(shotOrigin.position, shotOrigin.position + shoot);
        }
    }
}
