using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticFire : MonoBehaviour
{
    public Transform target;
    public Transform shootPos;
    public GameObject projectile;
    public bool autoFire = false;
    public float autoFireInterval = 5;

    [Space(10)]
    public ParticleSystem telegraphParticles;
    public float telegraphTime = .5f;



    private ControllerBase controller;
    private bool hasTelegraphed;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        timer = autoFireInterval;
        hasTelegraphed = false;
        controller = GetComponent<ControllerBase>();
    }

    private void Update()
    {
        if (autoFire)
        {
            timer -= Time.deltaTime;

            if(!hasTelegraphed && timer <= telegraphTime)
            {
                hasTelegraphed = true;
                if (telegraphParticles)
                {
                    telegraphParticles.Play();
                }
            }

            if(timer <= 0)
            {
                Fire(target.position);
                timer = autoFireInterval;
                hasTelegraphed = false;
            }
        }
    }

    public bool Fire(Vector3 targetPos)
    {
        if (controller != null && controller.isStunned)
        {
            return false;
        }

        Vector3 trajectory = BallisticTrajectory(transform.position, targetPos);
        GameObject obj = Instantiate(projectile, shootPos.position, Quaternion.identity);
        obj.GetComponent<Rigidbody>().AddForce(trajectory, ForceMode.Impulse);

        return true;
    }

    public static Vector3 BallisticTrajectory(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        float height = dir.y;
        dir.y = 0;

        float distance = dir.magnitude;
        float a = 45 * Mathf.Deg2Rad;
        dir.y = distance * Mathf.Tan(a);
        distance += height / Mathf.Tan(a);

        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * dir.normalized;
    }
}
