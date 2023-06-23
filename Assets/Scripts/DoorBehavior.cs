using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    Transform left;
    Transform right;
    Animator anim;
    bool open;
    private GameObject player;

    void Open()
    {
        anim.Play("GateOpen");
    }

    void Close()
    {
        anim.Play("GateClose");
    }

    // Start is called before the first frame update
    void Start()
    {
        left = transform.Find("Door_Left");
        right = transform.Find("Door_Right");
        anim = GetComponent<Animator>();
        open = false;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 4f)
        {
            if (!open)
            {
                open = true;
                Open();
            }
        }
        else if (open)
        { 
            open = false;
            Close();
        }
    }
}
