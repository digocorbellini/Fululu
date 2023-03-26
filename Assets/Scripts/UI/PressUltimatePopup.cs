using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressUltimatePopup : MonoBehaviour
{
    private PlayerInput input;

    private void Awake()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // listen for ultimate press
        input.actions["Ultimate"].started += Deactivate;

        // deactive all inputs except ultimate
        input.actions["Jump"].Disable();
        input.actions["Dash"].Disable();
        input.actions["Attack"].Disable();
        input.actions["AltFire"].Disable();
        input.actions["Move"].Disable();
    }


    private void Deactivate(InputAction.CallbackContext context)
    {
        // reactivate all inputs
        input.actions["Jump"].Enable();
        input.actions["Dash"].Enable();
        input.actions["Attack"].Enable();
        input.actions["AltFire"].Enable();
        input.actions["Move"].Enable();

        // stop listening for ulitimate press
        input.actions["Ultimate"].started -= Deactivate;

        // deactivate this gameobject
        this.gameObject.SetActive(false);
    }
}
