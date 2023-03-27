using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanZone : MonoBehaviour
{
    public GameObject han;
    public ParticleSystem particles;
    public float minAppearanceTime = 3.0f;

    private float timer;
    bool active;

    private void Start()
    {
        active = false;
        timer = 0;
        han.SetActive(false);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            timer = minAppearanceTime;
            han.SetActive(true);
            particles.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(timer <= 0)
            {
                Disappear();
            }
            else
            {
                StartCoroutine(DelayedDisappear(timer));
            }
        }
    }

    private void Disappear()
    {
        particles.Play();
        han.SetActive(false);
        active = false;
    }

    private IEnumerator DelayedDisappear(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Disappear();
    }
}
