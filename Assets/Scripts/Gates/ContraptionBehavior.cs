using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContraptionBehavior : MonoBehaviour
{
    Animator anim;
    AudioSource[] sounds;
    private bool open;
    private TriggerableObject[] triggerObjs;

    void Open()
    {
        open = true;
        anim.Play("GateOpen");
        if (sounds != null)
            sounds[0].Play();
    }

    void Close()
    {
        open = false;
        anim.Play("GateClose");
        if (sounds != null)
            sounds[1].Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        open = false;
        triggerObjs = transform.GetComponentsInChildren<TriggerableObject>();
        sounds = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (TriggerableObject obj in triggerObjs)
        {
            if (obj.GetStatus())
            {
                if (!open)
                {
                    Open();
                }
                return;
            }
        }

        if (open)
            Close();
    }

}
