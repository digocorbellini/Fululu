using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CaptureEffect : MonoBehaviour
{
    [SerializeField] public float launchForce = 500;
    [SerializeField] public float travelTime = 5;
    [SerializeField] public ParticleSystem effects;

    private Rigidbody rb;
    private Transform player;
    private bool launchComplete = false;
    private float timeElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        effects = GetComponentInChildren<ParticleSystem>();

        StartCoroutine(LaunchSpirit());
    }

    private IEnumerator LaunchSpirit()
    {
        rb.AddForce(transform.up * launchForce);
        yield return new WaitForSeconds(.5f);

        launchComplete = true;
    }

    private void Update()
    {
        if (!launchComplete)
            return;

        transform.position = Vector3.Lerp(transform.position, player.position, timeElapsed / travelTime);

        timeElapsed += Time.deltaTime;

        if (timeElapsed > travelTime)
        {
            DetachParticles();
            Destroy(gameObject);
        }  
    }

    private void OnTriggerStay(Collider other)
    {
        if(launchComplete && other.gameObject.tag == "Player")
        {
            DetachParticles();
            Destroy(gameObject);
        }
    }

    private void DetachParticles()
    {
        if (effects)
        {
            // Detach particles so they can play out
            effects.transform.parent = null;

            // Disable emission. Should self destruct when all particles dead
            effects.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
