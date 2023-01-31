using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public EntityHitbox[] barrierGenerators;
    private int generatorLefts;
    private void Start()
    {
        generatorLefts = barrierGenerators.Length;

        foreach(EntityHitbox e in barrierGenerators)
        {
            e.OnDeath += DestroyGenerator;
        }
    }

    private void DestroyGenerator()
    {
        generatorLefts--;

        if(generatorLefts <= 0)
        {
            Destroy(gameObject);
        }
    }
}
