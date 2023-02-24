using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float xLookSpeed;
    [SerializeField] private float xDeadZoneEpsilon = 0.01f;
    public float yLookSpeed;
    [SerializeField] private float yDeadZoneEpsilon = 0.01f;
    [SerializeField] private bool invertY = true;

    private CinemachineFreeLook cam;
    private PlayerInput input;
    private InputAction lookAction;

    private const float Y_MAX = 1.0f;
    private const float Y_MIN = 0.0f;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        cam = GetComponentInChildren<CinemachineFreeLook>();

        lookAction = input.actions["Look"];
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        if (Mathf.Abs(lookInput.x) > xDeadZoneEpsilon)
        {
            cam.m_XAxis.Value += lookInput.x * xLookSpeed * Time.deltaTime;
        }
        
        if (Mathf.Abs(lookInput.y) > yDeadZoneEpsilon)
        {
            float newYVal = cam.m_YAxis.Value;
            int sign = (invertY) ? -1 : 1;
            newYVal = Mathf.Clamp(newYVal + (sign * lookInput.y * yLookSpeed * Time.deltaTime), Y_MIN, Y_MAX);
            cam.m_YAxis.Value = newYVal;
        }
    }
}
