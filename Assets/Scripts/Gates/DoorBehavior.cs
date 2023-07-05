using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    Animator anim;
    private bool open;
    private DoorTrigger[] triggers;

    void Open()
    {
        open = true;
        anim.Play("GateOpen");
    }

    void Close()
    {
        open = false;
        anim.Play("GateClose");
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        open = false;
        triggers = transform.GetComponentsInChildren<DoorTrigger>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (DoorTrigger trigger in triggers)
        {
            if (!trigger.triggered)
            {
                if (open)
                {
                    Close();
                }
                return;
            }
        }

        if (!open)
            Open();
    }

}
