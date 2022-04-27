using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralFX : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles = new List<ParticleSystem>();

    float timeAlive = 0.0f;
    bool activated = false;

    private void Start()
    {
        foreach (ParticleSystem particle in particles)
        {
            var main = particle.main;
            main.startLifetime = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        
        if (timeAlive > 0.1f && !activated)
        {

            foreach (ParticleSystem particle in particles)
            {
                var main = particle.main;
                main.startLifetime = 10.0f;
            }
            activated = true;
        }


    }
}
