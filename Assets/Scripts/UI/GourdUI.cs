using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GourdUI : MonoBehaviour
{
    public Sprite ErrorSprite;
    public Image gourdUI;

    public CaptureAbilities nothing;

    // Start is called before the first frame update
    void Start()
    {
       // GameManager.OnCapture += UpdateUI;
    }

    private void UpdateUI(CaptureAbilities cap)
    {
        if(cap == null)
        {
            cap = nothing;
        }

        Sprite img = cap.enemySprite;
        if(img != null)
        {
            gourdUI.sprite = img;
        }
        else
        {
            gourdUI.sprite = ErrorSprite;
        }
    }
}
