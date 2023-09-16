using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipInteractable : InteractableObject
{
    private GameObject player;
    private float yVelocity = 0;
    private bool triggered;
    [SerializeField] private GameObject spawnLocation;
    [SerializeField] private GameObject cockpitLocation;
    [SerializeField] private GameObject torches;
    [SerializeField] private GameObject hatch;
    private Image square;
    private AudioSource bgMusic;
    private AudioSource rocketSound;

    private PauseMenu pauseMenu;
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
        FirstPersonController._cinemachineTargetYaw = 90;
        cameraRoot.transform.position = spawnLocation.transform.position;
        cameraRoot.transform.rotation = spawnLocation.transform.rotation;
        player.GetComponent<Rigidbody>().isKinematic = true;
        triggered = true;
        rocketSound.Play();

        foreach (Transform child in torches.transform)
        {
            child.GetComponent<ParticleSystem>().Play();
        }

        player.transform.position = cockpitLocation.transform.position;
        player.transform.rotation = cockpitLocation.transform.rotation;
        hatch.SetActive(true);
        hatch.GetComponent<Animator>().Play("HatchClose");
    }

    public override bool StopInteract()
    {
        cameraRoot.transform.parent = GameObject.FindWithTag("Player").transform;
        cameraRoot.transform.localPosition = new Vector3(0, 1.675f, .15f);
        cameraRoot.transform.localRotation = Quaternion.identity;

        player.GetComponent<Rigidbody>().isKinematic = false;
        triggered = false;
        square.enabled = false;
        bgMusic.volume = 1;
        bgMusic.Play();

        return false;
    }

    public IEnumerator FadeToBlack(float fadeSpeed)
    {
        square.enabled = true;
        Color objectColor = square.color;
        float fadeAmount;
        while (square.color.a < 1)
        {
            fadeAmount = objectColor.a + fadeSpeed * Time.fixedDeltaTime;

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            square.color = objectColor;
            yield return null;
        }
    }

    public IEnumerator FadeToClear(int fadeSpeed)
    {
        Color objectColor = square.color;
        float fadeAmount;
        while (square.color.a > 0)
        {
            fadeAmount = objectColor.a - fadeSpeed * Time.deltaTime;

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            square.color = objectColor;
            yield return null;
        }
        square.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cameraRoot = GameObject.FindWithTag("CinemachineTarget");
        rocketSound = GetComponent<AudioSource>();
        bgMusic = GameObject.FindWithTag("SceneLoader").GetComponent<AudioSource>();

        pauseMenu = GameObject.FindWithTag("Canvas").transform.Find("PauseMenu").GetComponent<PauseMenu>();
        mainMenu = GameObject.FindWithTag("Canvas").transform.Find("MainMenu").GetComponent<MainMenu>();
        square = GameObject.FindWithTag("Canvas").transform.Find("BlackSquare").GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (triggered)
        {
            if (bgMusic.volume > 0)
            {
                bgMusic.volume -= .02f;
                return;
            }
            else if (bgMusic.isPlaying)
            {
                bgMusic.Stop();
                StartCoroutine(FadeToBlack(.1f));
                mainMenu.RollCredits();
            }
            yVelocity += Time.deltaTime/10;

            player.transform.position = cockpitLocation.transform.position;
            transform.parent.position = transform.position + new Vector3(0, yVelocity*yVelocity, 0);
        }
    }
}
