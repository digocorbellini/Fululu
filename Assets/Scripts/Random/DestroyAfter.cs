using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{

    public float lifetime = 10.0f;

    public GameObject[] unparentBeforeDestroy;

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;

        if(lifetime <= 0)
        {
            foreach(GameObject go in unparentBeforeDestroy)
            {
                go.transform.SetParent(null, true);
            }
            Destroy(gameObject);
        }
    }
}
