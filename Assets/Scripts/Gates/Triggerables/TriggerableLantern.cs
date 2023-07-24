using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerableLantern : TriggerableObject
{

    public override void SetStatus(bool status)
    {
        isTriggered = status;

        // TODO: light lantern, create fire effect! (enable particle system?)
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
