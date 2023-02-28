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
    public float chargeRingDeadZone = .1f;

    public AudioSource audioSource;

    [HideInInspector] public float chargeRate = 1.0f;

    [Space(10)]

    [Header("Gourd Capture Attack Settings")]

    public LayerMask captureMask;
    public BoxCollider captureBounds;
    public GameObject captureZoneVisualization;
    public LayerMask raycastIgnore;
    public AudioClip captureSFX;
    public ParticleSystem captureAttackParticles;
    public ParticleSystem holdTornado;
    public ParticleSystem captureTornado;
    [Range(0f, 1f)]
    public float captureAttemptCost = 0f;
    public GameObject CaptureEffects;

    [Header("Debug/Cheats")]
    public Weapon cheatWeapon;
    public bool clickToSetWeapon = false;

    private float maxRingSize;
    private float currRingSize;
    private float timeCharging;
    private float lastUnchargedFireTime; // the time when uncharged was last fired
    private float lastChargedFireTime;
    public bool isCharging;
    private Vector3? lookAtPos;

    // Start is called before the first frame update
    void Start()
    {
        isCharging = false;
        weapon = defaultWeapon;
        fullChargeTime = weapon.chargeTime;
        lastUnchargedFireTime = Time.time - weapon.rapidFireCooldown;
        lastChargedFireTime = Time.time - weapon.chargeFireCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            timeCharging += Time.deltaTime * chargeRate;
        }

        UpdateReticle();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin += ray.direction * .1f;
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f, ~raycastIgnore))
        {
            lookAtPos = hit.point;
            Debug.DrawLine(ray.origin, hit.point, Color.magenta);
        }
        else
        {
            lookAtPos = null;
        }

        if (clickToSetWeapon)
        {
            if(cheatWeapon != null)
            {
                SwitchWeapon(cheatWeapon);
            }
            clickToSetWeapon = false;
        }

    }

    private void UpdateReticle()
    {
        if (recticleRing)
        {
            recticleRing.enabled = isCharging;
            currRingSize = Mathf.Lerp(0.0f, 1.0f, (timeCharging - chargeRingDeadZone) / (fullChargeTime - chargeRingDeadZone));
            recticleRing.fillAmount = currRingSize;
        }
    }

    public void StartCharging()
    {
        if (CanShootCharged())
            isCharging = true;
        timeCharging = 0.0f;
        // TODO: will want some sort of feedback for the cooldown. Maybe an error sound effect and anim?
    }

    // Returns true if the cooldown time has elapsed since the last uncharged fire
    private bool CanShootUncharged()
    {
        return Time.time > (lastUnchargedFireTime + weapon.rapidFireCooldown);
    }

    // Returns true if the cooldown time has elapsed since the last charged fire
    private bool CanShootCharged()
    {
        return Time.time > (lastChargedFireTime + weapon.chargeFireCooldown);
    }

    public bool StopCharging()
    {
        isCharging = false;
        bool didFire = false;

        //shoot the projectile
        if (timeCharging >= fullChargeTime && CanShootCharged())
        {
            // Fire charged attack from weapon
            didFire = weapon.ChargedFire(shootPoint, lookAtPos);
            lastChargedFireTime = Time.time;
        }
        else if (CanShootUncharged())
        {
            if (weapon.chargedBullet != null)
            {
                didFire = weapon.Fire(shootPoint, lookAtPos);
            }
            else
            {
                didFire = defaultWeapon.Fire(shootPoint, lookAtPos);
            }

            lastUnchargedFireTime = Time.time;
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

    public float CaptureAttack(float chargePercent)
    {
        if(chargePercent < captureAttemptCost)
        {
            return 0;
        }
        //Debug.Log("Performing capture!");
        //Debug.Log("Bounds: " + (captureBounds.center + shootPoint.position) + " Size: " + captureBounds.size / 2.0f);

        Collider[] colliders = Physics.OverlapBox(captureBounds.center + shootPoint.position, captureBounds.size / 2.0f, shootPoint.rotation, captureMask);
        //captureZoneVisualization.SetActive(false);
        holdTornado.Stop();
        foreach (Collider collider in colliders)
        {
            ControllerBase controller = collider.GetComponentInParent<ControllerBase>();
            if (controller && controller.isCapturable)
            {
                // Found enemy in range
                if(controller.captureCost <= chargePercent)
                {
                    // Player has enhough charge to capture

                    holdTornado.gameObject.SetActive(false);

                    audioSource.PlayOneShot(captureSFX);
                    captureAttackParticles.Play();
                    SwitchWeapon(controller.captureWeapon);

                    // Spawn spirit capture particle
                    Instantiate(CaptureEffects, controller.transform.position, Quaternion.identity);
                    //int numMetersConsumed = Mathf.FloorToInt(controller.captureCost / .33f);
                    captureTornado.Play();

                    Destroy(controller.gameObject);

                    holdTornado.gameObject.SetActive(true);

                    return controller.captureCost;
                }
            }
        }
        // Failed to capture
        return captureAttemptCost;
    }

    public void ShowCaptureZone(bool show)
    {
        //captureZoneVisualization.SetActive(show);
        if (show)
            holdTornado.Play();
        else
            holdTornado.Stop();
    }

    public void SwitchWeapon(Weapon wep)
    {
        if (wep)
        {
            timeCharging = 0.0f;
            weapon = wep;
            fullChargeTime = wep.chargeTime;
            GameManager.instance.UIManager.gourdUI.SetCaptureImage(wep.captureImage);
        }
    }
}
