using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialZone : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera cam;
    public Tutorial tut;
    public bool enable;
    public bool disable;
    public UIPinger.PingLocation pingLocation;

    private int index;
    private float timer;
    private PlayerInput input;
    private bool alreadyTriggered = false;
  
    public void Start()
    {
        input = GetComponent<PlayerInput>();
        index = 0;
        timer = 0;
    }

    public void Update()
    {
        if (disable)
        {
            cam.enabled = false;
            input.enabled = false;
            GameManager.instance.ToggleInput(true);
            UIManager.instance.ToggleTutorial(false);
            disable = false;
        }

        timer += Time.deltaTime;
    }

    private void SetTutorial(int i)
    {
        UIManager.instance.SetTutorialText(tut.dialogue[i]);
        UIManager.instance.SetTutorialNametag(tut.names[i]);
        UIManager.instance.SetPotraitImage(tut.potraits[i]);
    }

    private void OnDrawGizmosSelected()
    {

    }

    public void OnNext()
    {
        if(timer > .66)
        {
            NextTutorial();
        }
    }

    private void NextTutorial()
    {
        index++;
        if(index >= tut.dialogue.Length)
        {
            cam.enabled = false;
            input.enabled = false;
            GameManager.instance.ToggleInput(true);
            UIManager.instance.ToggleTutorial(false);

            if (pingLocation != UIPinger.PingLocation.None)
            {
                GameManager.instance.StopPingUI(pingLocation);
            }
        }
        else
        {
            SetTutorial(index);
            timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hello!!! TriggerEnter");
        if (!alreadyTriggered)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;

                cam.enabled = true;
                input.enabled = true;
                GameManager.instance.ToggleInput(false);
                UIManager.instance.ToggleTutorial(true);
                SetTutorial(index);
                enable = false;
                index = 0;
                timer = -1;

                if(pingLocation != UIPinger.PingLocation.None)
                {
                    GameManager.instance.PingUI(pingLocation);
                }
            }
        }
    }
}
