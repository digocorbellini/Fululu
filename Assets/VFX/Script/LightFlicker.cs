using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.25f;
    public float maxIntensity = 0.5f;
    
    private ParticleSystem particles;
    private Light lightSource;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        lightSource = GetComponent<Light>();
        
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float randomIntensity = Random.Range(minIntensity, maxIntensity);
            
            lightSource.intensity = randomIntensity;
            particles.startSize = randomIntensity * 2;
            
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }
    }
}
