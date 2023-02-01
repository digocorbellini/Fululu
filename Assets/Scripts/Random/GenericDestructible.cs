using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDestructible : MonoBehaviour
{
    private void Start()
    {
        GetComponent<EntityHitbox>().OnDeath += Selfdestruct;
    }

    private void Selfdestruct()
    {
        Destroy(gameObject);
    }
}
