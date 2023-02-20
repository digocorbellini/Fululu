using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GourdUI : MonoBehaviour
{
    public Image captureImage;
    public Animator anim;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }
    }

    public void SetCaptureImage(Sprite image)
    {
        if (image)
        {
            captureImage.sprite = image;
            captureImage.gameObject.SetActive(true);
        } else
        {
            captureImage.gameObject.SetActive(false);
        }
    }

    public void SetCharging(bool charging)
    {
        if (anim)
        {
            if (charging)
            {
                anim.Play("Charging");
            } else
            {
                anim.Play("Idle");
            }
            
        }
    }
}
