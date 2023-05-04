using System;
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
    public GameObject[] activateBefore;
    public GameObject[] activateDuring;
    public GameObject[] activateAfter;
    public Cinemachine.CinemachineVirtualCamera[] camSwaps;
    public bool respawnPlayer = false;

    private Cinemachine.CinemachineVirtualCamera currCam;

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
        if (i < tut.dialogue.Length)
        {
            UIManager.instance.SetTutorialText(tut.dialogue[i]);
        }
        
        if (i < tut.names.Length) {
            UIManager.instance.SetTutorialNametag(tut.names[i]);
        }
        
        if (i < tut.potraits.Length)
        {
            UIManager.instance.SetPotraitImage(tut.potraits[i]);
        }

        if(tut.voiceLines[i])
        {
            GameManager.instance.PlayVoiceLine(tut.voiceLines[i]);
        }

        if (camSwaps[i])
        {
            if(currCam != null)
            {
                currCam.enabled = false;
            }

            currCam = camSwaps[i];
            currCam.enabled = true;
        }
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

            currCam.enabled = false;
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
            if (respawnPlayer)
            {
                GameManager.instance.SpawnPlayer();
                GameManager.instance.UIManager.Reset();
            }
            activateDuring.ToList().ForEach(obj => obj.SetActive(false));
            activateAfter.ToList().ForEach(obj => obj.SetActive(true));

            if (tut.afterVoiceLine)
            {
                GameManager.instance.PlayVoiceLine(tut.afterVoiceLine, 1.0f, 1.0f);
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
        if (!alreadyTriggered)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;

                if (!input)
                {
                    GetComponent<PlayerInput>();
                }

                int l = tut.dialogue.Length;

                if (camSwaps.Length < l)
                {
                    Cinemachine.CinemachineVirtualCamera[] temp = new Cinemachine.CinemachineVirtualCamera[l];
                    Array.Copy(camSwaps, temp, camSwaps.Length);
                    camSwaps = temp;
                }

                index = 0;
                camSwaps[0] = cam;

                input.enabled = true;
              
                GameManager.instance.ToggleInput(false);
                UIManager.instance.ToggleTutorial(true);
                SetTutorial(index);
                enable = false;
                timer = -1;

                activateBefore.ToList().ForEach(obj => obj.SetActive(true));
                activateDuring.ToList().ForEach(obj => obj.SetActive(true));

                if (pingLocation != UIPinger.PingLocation.None)
                {
                    GameManager.instance.PingUI(pingLocation);
                }
            }
        }
    }
}
