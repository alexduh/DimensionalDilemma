using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class TriggerableLantern : TriggerableObject
{
    private ParticleSystem _particleSystem;
    private AudioSource[] audioSources;

    public override void SetStatus(bool status)
    {
        isTriggered = status;
        if (!_particleSystem)
            return;

        if (status)
        {
            _particleSystem.Play(true);
            audioSources[1].Play();
        }
        else
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            audioSources[0].Play();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
            _particleSystem = child.gameObject.GetComponent<ParticleSystem>();

        audioSources = GetComponents<AudioSource>();
    }

}
