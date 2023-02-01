using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFireControl : MonoBehaviour
{
    public Weapon defaultWeapon;
    public Weapon weapon;

    public Transform shootPoint;

    public Image recticleRing;

    public float fullChargeTime;

    private float maxRingSize;
    private float currRingSize;
    private float timeCharging;
    private bool isCharging;

    // Start is called before the first frame update
    void Start()
    {
        maxRingSize = recticleRing.transform.localScale.x;
        recticleRing.enabled = false;
        isCharging = false;
        weapon = defaultWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            timeCharging += Time.deltaTime;
            currRingSize = Mathf.Lerp(0.0f, 1.0f, (timeCharging - .1f) / fullChargeTime);
            //recticleRing.transform.localScale = new Vector3(currRingSize, currRingSize);
            recticleRing.fillAmount = currRingSize;
        }
    }

    public void StartCharging()
    {
        isCharging = true;
        recticleRing.enabled = true;
        timeCharging = 0.0f;
        recticleRing.fillAmount = 0;
        //currRingSize = maxRingSize;
    }

    public bool StopCharging()
    {
        isCharging = false;
        recticleRing.enabled = false;
        bool didFire = false;

        //shoot the projectile
        if (timeCharging >= fullChargeTime)
        {
            print("Firing charged attack");
            didFire = weapon.ChargedFire(shootPoint);
        }
        else
        {
            didFire = weapon.Fire(shootPoint);
        }

        timeCharging = 0.0f;

        // returns if the player shot or not;
        return didFire;
    }

    public void SacrificeWeapon()
    {
        weapon.Sacrifice(shootPoint);
        SwitchWeapon(defaultWeapon);
    }

    public void SwitchWeapon(Weapon wep)
    {
        timeCharging = 0.0f;
        weapon = wep;
        fullChargeTime = wep.chargeTime;
    }
}
