using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInteractable : InteractableObject
{
    private GameObject player;
    private GameObject cameraRoot;
    private float yVelocity = 0;
    private bool triggered;
    [SerializeField] private GameObject spawnLocation;
    [SerializeField] private GameObject torches;
    private AudioSource bgMusic;
    private AudioSource rocketSound;


    public override void Interact()
    {
        foreach (Transform child in cameraRoot.transform)
        {
            Gun gun = child.GetChild(0).gameObject.GetComponent<Gun>();
            if (gun.isActiveAndEnabled)
                gun.GunIn();
        }

        cameraRoot.transform.parent = null;
        cameraRoot.transform.position = spawnLocation.transform.position;
        cameraRoot.transform.rotation = spawnLocation.transform.rotation;
        //player.transform.position = transform.position;
        torches.SetActive(true);
        triggered = true;
        rocketSound.Play();
        // TODO: change camera clamp angle, show player inside ship, screen fade to black, roll credits
    }

    public override void StopInteract()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraRoot = GameObject.FindWithTag("CinemachineTarget");
        rocketSound = GetComponent<AudioSource>();
        bgMusic = GameObject.FindWithTag("SceneLoader").GetComponent<AudioSource>();
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
            transform.parent.position = transform.position + new Vector3(0, yVelocity*yVelocity, 0);
        }
    }
}
