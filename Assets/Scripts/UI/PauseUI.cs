using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject PausePanel;
    public Selectable initialSelection;
    public CanvasGroup pauseGroup;
    public OptionsMenu optionsMenu;
    public Selectable OptionsButton;

    // Start is called before the first frame update
    public void Show()
    {
        gameObject.SetActive(true);

        // ensure only pause panel is active
        PausePanel.SetActive(true);
        optionsMenu.Hide();
        pauseGroup.interactable = true;

        initialSelection.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        pauseGroup.interactable = false;
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
        optionsMenu.Show();
        pauseGroup.interactable = false;
    }

    public void BackToPause()
    {
        PausePanel.SetActive(true);
        optionsMenu.Hide();
        pauseGroup.interactable = true;
        OptionsButton.Select();
    }
}
