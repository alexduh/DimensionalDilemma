using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    Animator anim;
    AudioSource[] sounds;
    private PersistentData persistentData;
    private bool open;
    private TriggerableObject[] triggerObjs;
    UniqueId uniqueID;

    void Open()
    {
        open = true;
        anim.Play("GateOpen");
        if (sounds[0] != null)
            sounds[0].Play();
    }

    void Close()
    {
        open = false;
        anim.Play("GateClose");
        if (sounds[1] != null)
            sounds[1].Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        open = false;
        triggerObjs = transform.GetComponentsInChildren<TriggerableObject>();
        sounds = GetComponents<AudioSource>();
        persistentData = GameObject.Find("SceneLoader").GetComponent<PersistentData>();
        uniqueID = GetComponent<UniqueId>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (uniqueID && persistentData.openGates.Contains(uniqueID.uniqueId))
        {
            if (!open)
                Open();

            return;
        }

        foreach (TriggerableObject obj in triggerObjs)
        {
            if (!obj.GetStatus())
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
