using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPinger : MonoBehaviour
{
    /*
     * Instructions
     * 1. Copy the "ping" prefab (there is one under canvas > gourd
     * 2. Place the copied prefab in the right location/parent
     * 3. Add location to PingLocation enum and GetTarget function
     * 4. Add new variable to store new prefab's animator below, assign in editor inspector
     * 5. Call **GameManager**.PingUI() to ping
     * 6. Call **GameManager**.StopPingUI() or specify time in StartPing() to stop
     */

    public Animator gourdPing;
    public Animator heartThree;

    public enum PingLocation
    {
        None,
        Gourd,
        HeartThree
    }
    
    public void PlayPing(PingLocation location, float time = 0)
    {
        if(location == PingLocation.None)
        {
            return;
        }

        Animator target = GetTarget(location);
        target.SetTrigger("Play");

        if(time > 0)
        {
            StartCoroutine(StopPing(target, time));
        }
    }

    public void StopPing(PingLocation location)
    {
        if (location == PingLocation.None)
        {
            return;
        }

        Animator target = GetTarget(location);
        target.SetTrigger("Stop");
    }

    private Animator GetTarget(PingLocation location)
    {
        switch (location)
        {
            case PingLocation.Gourd:
                return gourdPing;
            case PingLocation.HeartThree:
                return heartThree;
            default:
                Debug.LogError("Invalid ping location: " + location);
                return null;
        }
    }
    
    public IEnumerator StopPing(Animator anim, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        anim.SetTrigger("Stop");
    }
}
