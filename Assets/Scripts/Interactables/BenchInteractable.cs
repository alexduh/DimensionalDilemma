using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.Rendering.DebugUI.Table;

public class BenchInteractable : InteractableObject
{
    private GameObject player;

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
        FirstPersonController.LeftClamp = -90;
        FirstPersonController.RightClamp = 90;
        // TODO: change limbs position
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
