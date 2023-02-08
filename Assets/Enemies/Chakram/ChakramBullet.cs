using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramBullet : BulletBase
{
    public int bounces = 3;
    public Collider collisionBox;
    private AudioSource sfx;

    private void OnTriggerEnter(Collider other)
    {

    }

    private void Update()
    {

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sfx = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bounces--;
        bool shouldDestroy = false;

        if(bounces <= 0)
        {
            shouldDestroy = true;
        }

        // Destroy on contact with player
        if (collision.gameObject.layer == 6)
        {
            shouldDestroy = false;
        }

        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            sfx.Play();
        }
        
    }

    public void Launch(Vector3 dir)
    {
        transform.SetParent(null, true);
        rb.AddForce(dir, ForceMode.Impulse);
        collisionBox.gameObject.SetActive(true);
    }
}
