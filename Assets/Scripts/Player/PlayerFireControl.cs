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

    [Space(10)]

    [Header("Gourd Capture Attack Settings")]

    public LayerMask captureMask;
    public BoxCollider captureBounds;
    public LayerMask raycastIgnore;
    public Image captureImage;

    private float maxRingSize;
    private float currRingSize;
    private float timeCharging;
    private bool isCharging;
    private Vector3? lookAtPos;

    // Start is called before the first frame update
    void Start()
    {
        isCharging = false;
        weapon = defaultWeapon;
        fullChargeTime = weapon.chargeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            timeCharging += Time.deltaTime;
        }

        UpdateReticle();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin += ray.direction * .1f;
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, ~raycastIgnore))
        {
            lookAtPos = hit.point;
            //print("Targeting: " + hit.collider.gameObject);
            Debug.DrawLine(ray.origin, hit.point, Color.magenta);
        }
        else
        {
            lookAtPos = null;
        }

    }

    private void UpdateReticle()
    {
        if (recticleRing)
        {
            recticleRing.enabled = isCharging;
            currRingSize = Mathf.Lerp(0.0f, 1.0f, (timeCharging - .1f) / fullChargeTime);
            recticleRing.fillAmount = currRingSize;
        }
    }

    public void StartCharging()
    {
        isCharging = true;
        timeCharging = 0.0f;
    }

    public bool StopCharging()
    {
        isCharging = false;
        bool didFire = false;

        //shoot the projectile
        if (timeCharging >= fullChargeTime)
        {
            print("Firing charged attack");
            didFire = weapon.ChargedFire(shootPoint, lookAtPos);
        }
        else
        {
            didFire = weapon.Fire(shootPoint, lookAtPos);
        }

        timeCharging = 0.0f;

        // returns if the player shot or not;
        return didFire;
    }

    public void CancelCharge()
    {
        isCharging = false;
        timeCharging = 0.0f;
    }

    public void SacrificeWeapon()
    {
        weapon.Sacrifice(shootPoint);
        SwitchWeapon(defaultWeapon);
    }

    public bool CaptureAttack()
    {
        Debug.Log("Performing capture!");
        Debug.Log("Bounds: " + (captureBounds.center + shootPoint.position) + " Size: " + captureBounds.size / 2.0f);
        Collider[] colliders = Physics.OverlapBox(captureBounds.center + shootPoint.position, captureBounds.size / 2.0f, shootPoint.rotation, captureMask);
        foreach(Collider collider in colliders)
        {
            ControllerBase controller = collider.GetComponentInParent<ControllerBase>();
            if (controller)
            {
                // Found enemy in range
                Debug.Log("Found enemy");
                SwitchWeapon(controller.captureWeapon);
                Destroy(controller.gameObject);
                return true;
            }
        }

        return false;
    }

    public void SwitchWeapon(Weapon wep)
    {
        if (wep)
        {
            timeCharging = 0.0f;
            weapon = wep;
            fullChargeTime = wep.chargeTime;
            if (captureImage)
            {
                if (wep.captureImage)
                {
                    captureImage.gameObject.SetActive(true);
                    captureImage.sprite = wep.captureImage;
                } else
                {
                    captureImage.gameObject.SetActive(false);
                }
                
            }
        }
    }
}
