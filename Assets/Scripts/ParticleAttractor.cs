using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class ParticleAttractor : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[1000];

    public Vector3 attractorPosition;
    float systemLifetime = 1;
    float timer;

    public void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        timer = systemLifetime;
    }

    public void LateUpdate()
    {
        // TODO: make particles curve towards target point!
        int particleCount = _particleSystem.GetParticles(_particles);
        if (timer < .9)
        {
            for (int i = 0; i < particleCount; i++)
            {
                Vector3 diff = (_particleSystem.transform.InverseTransformPoint(attractorPosition) - _particles[i].position) / _particles[i].remainingLifetime * 10 * Time.deltaTime;
                _particles[i].position += diff;
            }
            _particleSystem.SetParticles(_particles, particleCount);
        }
        if (timer < 0)
            Destroy(gameObject);

        timer -= Time.deltaTime;
    }
}