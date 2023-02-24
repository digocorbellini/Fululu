using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject PausePanel;

    // Start is called before the first frame update
    public void Show()
    {
        gameObject.SetActive(true);

        // ensure pause panel is the only active panel
        PausePanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        // ensure pause panel is the only active panel
        PausePanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        GameManager.instance.TogglePause();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    public void OpenSettings()
    {
        PausePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void OpenPause()
    {
        PausePanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }
}
