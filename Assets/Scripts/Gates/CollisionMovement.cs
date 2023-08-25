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
    private TriggerableObject[] triggerObjs;
    private float update;

    void Open()
    {
        open = true;
        startPos = movingObject.transform.position;
        endPos = endObj.transform.position;
        startRot = movingObject.transform.rotation;
        endRot = endObj.transform.rotation;
        update = 0;
    }

    void Close()
    {
        open = false;
        startPos = movingObject.transform.position;
        endPos = startObj.transform.position;
        startRot = movingObject.transform.rotation;
        endRot = startObj.transform.rotation;
        update = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        open = false;
        update = 1;
        triggerObjs = transform.GetComponentsInChildren<TriggerableObject>();

        foreach (Transform child in transform)
        {
            if (child.tag == "Triggerable")
                movingObject = child.gameObject;
            if (child.tag == "StartPos")
                startObj = child.gameObject;
            if (child.tag == "EndPos")
                endObj = child.gameObject;
        }
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
