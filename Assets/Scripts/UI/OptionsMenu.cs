using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public CanvasGroup optionsGroup;
    public Selectable initialSelection;

    public void Show()
    {
        gameObject.SetActive(true);
        optionsGroup.interactable = true;
        initialSelection.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        optionsGroup.interactable = false;
    }
}
