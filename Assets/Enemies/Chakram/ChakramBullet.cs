using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramBullet : BulletBase
{
    public int bounces = 3;
    public Collider collisionBox;
    

    private void OnTriggerEnter(Collider other)
    {

    }

    private void Update()
    {

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bounces--;

        if(bounces <= 0)
        {
            Destroy(gameObject);
            print("No more bounces");
        }

        // Destroy on contact with player
        if (collision.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector3 dir)
    {
        transform.SetParent(null, true);
        rb.AddForce(dir, ForceMode.Impulse);
        collisionBox.gameObject.SetActive(true);
    }
}
