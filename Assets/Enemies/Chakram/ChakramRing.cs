using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramRing : MonoBehaviour
{
    public float rotationSpeed;
    public ChakramBullet[] chakrams;
    public float launchForce;
    public bool manuralLaunch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime, 0);

        if (manuralLaunch)
        {
            Launch();
            manuralLaunch = false;
        }
    }

    public void Launch()
    {
        foreach (ChakramBullet chakram in chakrams)
        {
            if(chakram == null)
            {
                // chakrams may of been destroyed already
                continue;
            }

            // Find vector pointing out from center of ring
            Vector3 outDir = chakram.transform.position - transform.position;
            chakram.Launch(outDir * launchForce);
        }

        manuralLaunch = false;
    }
}
