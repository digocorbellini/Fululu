using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyDeathHandler : MonoBehaviour
{
    public float delay = 5.0f;
    private bool done = false;

    private void Update()
    {
        delay -= Time.deltaTime;
        

        if (delay <= 0 && !done)
        {
            delay = 100000;
            done = true;
            LevelChanger.instance.ChangeSceneAsync("WinScreen");
        }
    }
}
