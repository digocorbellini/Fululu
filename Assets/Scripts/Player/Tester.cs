using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public BulletSpread spread;

    // Start is called before the first frame update
    void Start()
    {
        spread.SpawnBullets();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
