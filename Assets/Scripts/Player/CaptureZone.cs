using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureZone : MonoBehaviour
{
    public PlayerController playerController;
    private float chargePercent;
    private bool alreadyCaptured;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        chargePercent = playerController.GetChargePercent();
    }

    private void OnTriggerStay(Collider other)
    {
        if (alreadyCaptured)
        {
            return;
        }

        ControllerBase entity = other.gameObject.GetComponentInParent<ControllerBase>();
        if (entity)
        {
            if (entity.isCapturable && entity.captureCost <= chargePercent)
            {
                alreadyCaptured = playerController.DoCapture(entity);
            }
        }
    }

    public void SetAlreadyCaptured(bool active)
    {
        alreadyCaptured = active;
    }


}
