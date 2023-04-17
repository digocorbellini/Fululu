using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathFollower : MonoBehaviour
{
    public float speed = 5f;
    public BezierPath path;

    private float distanceTraveled;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            distanceTraveled = 0f;
            StopAllCoroutines();
            StartCoroutine(startMoving());
        }
    }

    IEnumerator startMoving()
    {
        while (true)
        {
            distanceTraveled += speed * Time.deltaTime;
            transform.position = path.GetPosAtDistance(distanceTraveled);
            //transform.up = path.GetDirAtDistance(distanceTraveled);

            yield return null;
        }
    }
}
