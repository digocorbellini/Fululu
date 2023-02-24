using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject OptionsPanel;

    // Start is called before the first frame update
    public void Show()
    {
        gameObject.SetActive(true);

        // ensure only pause panel is active
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        GameManager.instance.TogglePause();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    public void OpenOptions()
    {
        PausePanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }
}
