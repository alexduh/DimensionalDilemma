using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    public bool triggered;
    public TriggerableObject[] objs;

    public void TriggerObjects()
    {
        foreach (var obj in objs)
        {
            obj.SetStatus(triggered);
        }
    }

}
