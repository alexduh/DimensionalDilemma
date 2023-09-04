using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    // Start is called before the first frame update
    public abstract void Interact();
    public abstract void StopInteract();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
