using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletSpawnLocation : MonoBehaviour
{
    public Transform mesh;
    public Transform lookAt;
    public float distanceFromPlayer = 0.5f;

    private Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraForward = mainCam.forward;
        cameraForward.y = 0;
        transform.position = mesh.transform.position + (cameraForward * distanceFromPlayer);
        transform.forward = (lookAt.position - transform.position).normalized;


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 10);
    }
}
