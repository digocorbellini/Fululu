using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    public TMPro.TextMeshProUGUI popupText;
    public Animator popupPanelAnim;

    [@TextAreaAttribute(2,5)]
    public string tutorialText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            popupText.text = tutorialText;
            popupPanelAnim.Play("Open");
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            popupPanelAnim.Play("Close");
        }
    }
}
