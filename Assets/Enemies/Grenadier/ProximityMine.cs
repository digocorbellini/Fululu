using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximityMine : MonoBehaviour
{
    public float maxLifetime = 20.0f;
    public GameObject explosion;

    // Update is called once per frame
    void Update()
    {
        maxLifetime -= Time.deltaTime;
        if(maxLifetime < 0)
        {
            Detonate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Detonate();         
        }
    }

    private void Detonate()
    {
        Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
