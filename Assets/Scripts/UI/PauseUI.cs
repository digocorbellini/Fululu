using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void Show()
    {
        gameObject.SetActive(true);
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
}
