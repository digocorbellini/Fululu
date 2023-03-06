using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject OptionsPanel;
    public Selectable initialSelection;
    public CanvasGroup pauseGroup;
    public CanvasGroup optionsGroup;

    // Start is called before the first frame update
    public void Show()
    {
        gameObject.SetActive(true);

        // ensure only pause panel is active
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);
        pauseGroup.interactable = true;

        initialSelection.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        pauseGroup.interactable = true;
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
        optionsGroup.interactable = true;
        pauseGroup.interactable = false;
    }

    public void BackToPause()
    {
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);
        optionsGroup.interactable = false;
        pauseGroup.interactable = true;
    }
}
