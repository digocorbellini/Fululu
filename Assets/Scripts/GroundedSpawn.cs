using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedSpawn : MonoBehaviour
{
    public GameObject mainObject;
    // Start is called before the first frame update
    void Start()
    {
        mainObject.transform.SetParent(null, true);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 1.5f);
    }
}
