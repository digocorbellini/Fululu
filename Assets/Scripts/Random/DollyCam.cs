using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCam : MonoBehaviour
{
    public Transform cam;
    public Transform[] targets;
    public float moveSpeed = 10f;
    public float acceleration = 10f;
    public bool accelerates = false;
    public int gizmoSteps = 100;

    private int currentIndex = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentIndex = 0;
            StopAllCoroutines();
            StartCoroutine(startMoving());
        }
    }

    IEnumerator startMoving()
    {
        while (currentIndex < targets.Length)
        {
            float distToNextTarget = Vector3.Distance(targets[currentIndex].position, cam.position);
            if (distToNextTarget < .01f)
            {
                currentIndex++;
            }

            // TODO: handle acceleration

            cam.position = Vector3.Slerp(cam.position, targets[currentIndex].position, Time.deltaTime * moveSpeed);
            cam.rotation = Quaternion.Slerp(cam.rotation, targets[currentIndex].rotation, Time.deltaTime * moveSpeed);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        
        for (int i = 0; i < targets.Length; i++)
        {
            Transform currTarget = targets[i];

            // draw forward direction of target locations
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(currTarget.position, currTarget.forward * 2f);

            if (i >= targets.Length - 1)
                continue;
            Transform nextTarget = targets[i + 1];
            // connect points
            Gizmos.color = Color.red;
            for (int t = 0; t < gizmoSteps - 1; t++)
            {
                Vector3 startPos = Vector3.Slerp(currTarget.position, nextTarget.position, (float)t / gizmoSteps);
                Vector3 endPos = Vector3.Slerp(currTarget.position, nextTarget.position, (float)(t + 1) / gizmoSteps);
                Vector3 dir = endPos - startPos;
                Gizmos.DrawRay(startPos, dir);
            }
        }



    }
}
