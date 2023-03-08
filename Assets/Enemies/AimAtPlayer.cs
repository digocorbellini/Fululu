using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtPlayer : MonoBehaviour
{
    public float heightOffset = .5f;

    private Transform player;
    public bool shouldLook = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player.position + (Vector3.up * heightOffset));
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldLook)
            transform.LookAt(player.position + (Vector3.up * heightOffset));
    }
}
