using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public GameObject boss;

    public GameObject SpawnTheBoss()
    {
        return Instantiate(boss, transform.position, Quaternion.identity);
    }
}
