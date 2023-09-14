using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBehavior : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void OnEnable()
    {
        _particleSystem.Play();
    }

    private void OnDisable()
    {
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
}
