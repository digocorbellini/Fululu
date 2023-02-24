using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : MonoBehaviour
{
    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider ySlider;

    private PlayerCameraController playerCamController;

    // Start is called before the first frame update
    void Start()
    {
        playerCamController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCameraController>();

        ySlider.onValueChanged.AddListener(UpdateYSensitivity);
        xSlider.onValueChanged.AddListener(UpdateXSensitivity);

        UpdateYSensitivity(ySlider.value);
        UpdateXSensitivity(xSlider.value);
    }

    private void UpdateYSensitivity(float value)
    {
        if(playerCamController)
            playerCamController.yLookSpeed = value;

        print("Y sens value: " + value);
    }

    private void UpdateXSensitivity(float value)
    {
        if(playerCamController)
            playerCamController.xLookSpeed = value;

        print("X sens value: " + value);
    }
}
