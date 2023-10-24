using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : InteractableObject
{
    private Animator animator;
    private bool closed;
    [SerializeField] private AudioSource doorSound;

    public override void Interact()
    {
        if (closed)
            animator.Play("Opening 1");
        else
            animator.Play("Closing 1");

        closed = !closed;
        doorSound.Play();
        StopInteract();
        InteractController.interactable = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        closed = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
