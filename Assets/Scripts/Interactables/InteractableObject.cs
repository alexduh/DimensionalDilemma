using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected GameObject cameraRoot;

    // Start is called before the first frame update
    public abstract void Interact();

    public virtual bool StopInteract()
    {
        foreach (Transform child in cameraRoot.transform)
        {
            Gun gun = child.GetChild(0).gameObject.GetComponent<Gun>();
            if (gun.isActiveAndEnabled)
                gun.GunOut();
        }

        return true;
    }
    void Awake()
    {
        cameraRoot = GameObject.FindWithTag("CinemachineTarget");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
