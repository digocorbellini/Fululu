using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramRing : MonoBehaviour
{
    public float rotationSpeed;
    public ChakramBullet[] chakrams;
    public float launchForce;
    public bool manuralLaunch;

    private bool isPreLaunch;
    private float preLaunchTimer;
    private float scale;

    private float PRELAUNCHTIME = .8f;
    private float PRELAUNCHSCALE = .7f;
    private Vector3 CHAKRAMSCALE;

    // Start is called before the first frame update
    void Start()
    {
        isPreLaunch = false;
        preLaunchTimer = PRELAUNCHTIME;
        scale = 1.0f;
        CHAKRAMSCALE = chakrams[0].transform.localScale;
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

        if (isPreLaunch)
        {
            preLaunchTimer -= Time.deltaTime;
            scale = Mathf.Lerp(PRELAUNCHSCALE, 1f, preLaunchTimer / PRELAUNCHTIME);
            transform.localScale = new Vector3(scale, scale, scale);

            foreach (ChakramBullet chakram in chakrams)
            {
                // Resize chakrams so they stay same size relatively
                if (chakram == null)
                {
                    continue;
                }

                chakram.gameObject.transform.localScale = CHAKRAMSCALE / scale;
            }

            if (preLaunchTimer <= 0)
            {
                isPreLaunch = false;
                Launch();
            }
        }
    }

    public void PreLaunch()
    {
        isPreLaunch = true;
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
        Destroy(gameObject);
    }
}
