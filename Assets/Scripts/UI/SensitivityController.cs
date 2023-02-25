using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityController : MonoBehaviour
{
    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider ySlider;

    private string mouseXPrefName = "MouseXSensitivity";
    private string mouseYPrefName = "MouseYSensitivity";

    private PlayerCameraController playerCamController;

    // Start is called before the first frame update
    void Start()
    {
        playerCamController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCameraController>();

        ySlider.onValueChanged.AddListener(UpdateYSensitivity);
        xSlider.onValueChanged.AddListener(UpdateXSensitivity);

        if (PlayerPrefs.HasKey(mouseXPrefName))
        {
            xSlider.value = PlayerPrefs.GetFloat(mouseXPrefName);
        }

        if (PlayerPrefs.HasKey(mouseYPrefName))
        {
            ySlider.value = PlayerPrefs.GetFloat(mouseYPrefName);
        }

        UpdateYSensitivity(ySlider.value);
        UpdateXSensitivity(xSlider.value);
    }

    private void UpdateYSensitivity(float value)
    {
        if(playerCamController)
            playerCamController.yLookSpeed = value;

        PlayerPrefs.SetFloat(mouseYPrefName, value);

        print("Y sens value: " + value);
    }

    private void UpdateXSensitivity(float value)
    {
        if(playerCamController)
            playerCamController.xLookSpeed = value;

        PlayerPrefs.SetFloat(mouseXPrefName, value);

        print("X sens value: " + value);
    }
}
