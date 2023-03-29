using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialZone : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera cam;
    public Tutorial tut;
    public bool enable;
    public bool disable;
    public UIPinger.PingLocation pingLocation;
    public GameObject[] activateDuring;
    public GameObject[] activateAfter;

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
            timer = 0;
        }
    }

    private void NextTutorial()
    {
        index++;
        if(index >= tut.dialogue.Length)
        {
            // Tutorial completed

            cam.enabled = false;
            if (input)
            {
                input.enabled = false;
            }
            GameManager.instance.ToggleInput(true);
            UIManager.instance.ToggleTutorial(false);

            if (pingLocation != UIPinger.PingLocation.None)
            {
                GameManager.instance.StopPingUI(pingLocation);
            }

            activateDuring.ToList().ForEach(obj => obj.SetActive(false));
            activateAfter.ToList().ForEach(obj => obj.SetActive(true));
        }
        else
        {
            SetTutorial(index);
            timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;
                index = 0;
                cam.enabled = true;
                if (input)
                {
                    input.enabled = true;
                }
                
                GameManager.instance.ToggleInput(false);
                UIManager.instance.ToggleTutorial(true);
                SetTutorial(index);
                enable = false;
                timer = -1;

                activateDuring.ToList().ForEach(obj => obj.SetActive(true));

                if (pingLocation != UIPinger.PingLocation.None)
                {
                    GameManager.instance.PingUI(pingLocation);
                }
            }
        }
    }
}
