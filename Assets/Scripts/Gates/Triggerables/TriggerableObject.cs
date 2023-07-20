using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerableObject : MonoBehaviour
{
    protected bool isTriggered = false;

    public abstract void SetStatus(bool status);

    public bool GetStatus()
    {
        return isTriggered;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
