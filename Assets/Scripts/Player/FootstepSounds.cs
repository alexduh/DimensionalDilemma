using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource soundPlayer;

    // Start is called before the first frame update
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        soundPlayer.clip = sounds[Random.Range(0, sounds.Length)];
        soundPlayer.Play();
    }
}
