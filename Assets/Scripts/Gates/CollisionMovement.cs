using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CollisionMovement : MonoBehaviour
{
    GameObject movingObject;
    GameObject startObj;
    GameObject endObj;
    Vector3 startPos;
    Vector3 endPos;
    Quaternion startRot;
    Quaternion endRot;

    private bool open;
    private DoorTrigger[] triggers;
    private float update;

    void Open()
    {
        open = true;
        startPos = startObj.transform.position;
        endPos = endObj.transform.position;
        startRot = startObj.transform.rotation;
        endRot = endObj.transform.rotation;
        update = 0;
    }

    void Close()
    {
        open = false;
        startPos = endObj.transform.position;
        endPos = startObj.transform.position;
        startRot = endObj.transform.rotation;
        endRot = startObj.transform.rotation;
        update = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        open = false;
        update = 1;
        triggers = transform.GetComponentsInChildren<DoorTrigger>();
        movingObject = GameObject.FindGameObjectWithTag("Triggerable");
        startObj = GameObject.FindGameObjectWithTag("StartPos");
        endObj = GameObject.FindGameObjectWithTag("EndPos");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (update < 1)
        {
            update += Time.deltaTime;
            movingObject.transform.position = Vector3.Lerp(startPos, endPos, update);
            movingObject.transform.rotation = Quaternion.Lerp(startRot, endRot, update);
            return;
        }
        else if (endPos != Vector3.zero)
        {
            movingObject.transform.position = endPos;
            movingObject.transform.rotation = endRot;
        }
            
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
