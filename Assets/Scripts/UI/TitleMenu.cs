using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    public Selectable initialSelection;
    public Selectable optionsButton;
    public CanvasGroup menuButtonGroup;
    public OptionsMenu optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        initialSelection.Select();
        menuButtonGroup.interactable = true;
    }

    public void ShowOptions()
    {
        menuButtonGroup.interactable = false;
        optionsMenu.Show();
    }

    public void BackToMenu()
    {
        optionsMenu.Hide();
        menuButtonGroup.interactable = true;
        optionsButton.Select();
    }
}
