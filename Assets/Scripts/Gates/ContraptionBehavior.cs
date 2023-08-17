using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContraptionBehavior : MonoBehaviour
{
    Animator anim;
    AudioSource sound;
    private bool open;
    private TriggerableObject[] triggerObjs;

    void Open()
    {
        open = true;
        anim.Play("GateOpen");
        if (sound != null)
            sound.Play();
    }

    void Close()
    {
        open = false;
        anim.Play("GateClose");
        if (sound != null)
            sound.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        open = false;
        triggerObjs = transform.GetComponentsInChildren<TriggerableObject>();
        sound = GetComponent<AudioSource>();
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
