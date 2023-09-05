using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.Rendering.DebugUI.Table;

public class BenchInteractable : InteractableObject
{
    private GameObject player;
    private GameObject cameraRoot;

    public override void Interact()
    {
        foreach (Transform child in cameraRoot.transform)
        {
            Gun gun = child.GetChild(0).gameObject.GetComponent<Gun>();
            if (gun.isActiveAndEnabled)
                gun.GunIn();
        }

        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
        // TODO: change limbs position
    }

    public override void StopInteract()
    {
        foreach (Transform child in cameraRoot.transform)
        {
            Gun gun = child.GetChild(0).gameObject.GetComponent<Gun>();
            if (gun.isActiveAndEnabled)
                gun.GunOut();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraRoot = GameObject.FindWithTag("CinemachineTarget");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
