using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDestructible : MonoBehaviour
{
    public GameObject destroyParticles;

    private void Start()
    {
        GetComponent<EntityHitbox>().OnDeath += Selfdestruct;
    }

    private void Selfdestruct()
    {
        if (destroyParticles)
        {
            Instantiate(destroyParticles, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }


}
