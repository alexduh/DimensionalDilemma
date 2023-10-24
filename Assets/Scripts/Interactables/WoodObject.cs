using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodObject : MonoBehaviour
{
    private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        float volume = collision.relativeVelocity.magnitude / 20;
        audioSource.volume = volume;
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
