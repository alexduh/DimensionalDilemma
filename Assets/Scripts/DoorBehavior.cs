using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    Transform left;
    Transform right;
    Animator anim;
    private bool open;
    private GameObject player;

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
                Open();
            }
        }
        else if (open)
        { 
            Close();
        }
    }
}
