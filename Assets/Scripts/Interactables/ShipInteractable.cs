using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInteractable : InteractableObject
{
    private GameObject player;
    private float yVelocity = 0;
    private bool triggered;
    [SerializeField] private GameObject spawnLocation;
    [SerializeField] private GameObject cockpitLocation;
    [SerializeField] private GameObject torches;
    [SerializeField] private GameObject hatch;
    private AudioSource bgMusic;
    private AudioSource rocketSound;

    private MainMenu mainMenu;

    public override void Interact()
    {
        foreach (Transform child in cameraRoot.transform)
        {
            Gun gun = child.GetChild(0).gameObject.GetComponent<Gun>();
            if (gun.isActiveAndEnabled)
                gun.GunIn();
        }

        cameraRoot.transform.parent = null;

        FirstPersonController.LeftClamp = 0;
        FirstPersonController.RightClamp = 180;
        cameraRoot.transform.position = spawnLocation.transform.position;
        cameraRoot.transform.rotation = spawnLocation.transform.rotation;
        player.GetComponent<Rigidbody>().isKinematic = true;
        torches.SetActive(true);
        triggered = true;
        rocketSound.Play();

        player.transform.rotation = cockpitLocation.transform.rotation;
        hatch.GetComponent<Animator>().Play("HatchClose");
        // TODO: screen fade to black

        mainMenu.RollCredits();
    }

    new public void StopInteract()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraRoot = GameObject.FindWithTag("CinemachineTarget");
        rocketSound = GetComponent<AudioSource>();
        bgMusic = GameObject.FindWithTag("SceneLoader").GetComponent<AudioSource>();

        mainMenu = GameObject.FindWithTag("Canvas").transform.Find("MainMenu").GetComponent<MainMenu>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (triggered)
        {
            if (bgMusic.volume > 0)
            {
                bgMusic.volume -= .001f;
                return;
            }
            
            yVelocity += Time.deltaTime/10;

            player.transform.position = cockpitLocation.transform.position;
            transform.parent.position = transform.position + new Vector3(0, yVelocity*yVelocity, 0);
        }
    }
}
