using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InteractController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] Transform holdArea;
    [SerializeField] GameObject crosshair;
    public static GameObject heldObject;
    private Rigidbody heldObjectRB;
    private float origDrag;
    private Collider[] heldColliders;
    public static InteractableObject interactable;
    private bool attemptingInteract = false;

    private Quaternion pickupRotation;
    private float pickupCameraY;
    private float inObjectTime = 0;
    private float deadTimer = 0;
    private bool dead = false;

    private PersistentData _persistentData;
    [SerializeField] private SceneLoader sceneloader;
    [SerializeField] private PauseMenu pauseMenu;

    private RaycastHit hit;

    private float shotLockoutTime = .25f;
    private float shotTimer = 0;
    [SerializeField] Gun gun1;
    [SerializeField] Gun gun2;
    [SerializeField] Camera mainCamera;
    private float pickupHorizontalRange = 2f;
    private float pickupVerticalRange = 2f;
    [Range(0,1)] private float scrollAmount;
    [SerializeField] private float pickupForce = 150.0f;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private TMP_Text scrollText;
    [SerializeField] private FadeText tutorialText;

    public bool inBarrier = false;
    public bool inMagneticBarrier = false;

    [SerializeField] private AudioSource crushSound;
    [SerializeField] private AudioSource pickupSound;

    // Start is called before the first frame update
    void Start()
    {
        _persistentData = sceneloader.GetComponent<PersistentData>();
        _input = GetComponent<StarterAssetsInputs>();
        scrollAmount = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TransparentFX"))
        {
            Scene active = SceneManager.GetActiveScene();
            Scene newActive = SceneManager.GetSceneByName(other.name);

            if (active != newActive)
            {
                sceneloader.SetScene(other.name);

                _persistentData.playerLocation = other.name;
                SaveData.SaveGame();
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            inBarrier = true;
            if (heldObject)
            {
                DropObject(heldObject);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("MagneticBarrier"))
        {
            inMagneticBarrier = true;
            // TODO: add behavior to Stardust objects!

            if (heldObject && heldObject.GetComponent<MetallicObject>())
            {
                DropObject(heldObject);
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Metal") && other.transform.localScale.x >= 2f)
        {
            inObjectTime += Time.deltaTime;
            if (inObjectTime >= .1f)
                Die();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Metal"))
            inObjectTime = 0;

        if (other.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            _persistentData.OpenGate(other.gameObject.GetComponent<UniqueId>().uniqueId);
            inBarrier = false;

            if (tutorialText.showingText)
                tutorialText.showingText = false;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("MagneticBarrier"))
            inMagneticBarrier = false;

    }

    private void Die()
    {
        GetComponent<CapsuleCollider>().radius = .05f;
        GetComponent<FirstPersonController>().enabled = false;
        GetComponent<PlayerInput>().enabled = false;

        crushSound.Play();
        gun1.SetGunConnected(false);
        gun2.SetGunConnected(false);

        dead = true;
        deadTimer = 5;
    }

    private void Respawn()
    {
        GetComponent<CapsuleCollider>().radius = .45f;
        GetComponent<FirstPersonController>().enabled = true;
        GetComponent<PlayerInput>().enabled = true;

        gun1.SetGunConnected(true);
        gun2.SetGunConnected(true);

        dead = false;
    }

    private bool CanInteract()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, pickupVerticalRange, Physics.AllLayers - (1 << LayerMask.NameToLayer("TransparentFX")))) 
        {
            if (hit.rigidbody)
                return CanPickUp();
            else
                return hit.transform.gameObject.GetComponent<InteractableObject>();
        }

        return false;
    }

    private bool CanPickUp()
    {
        if (heldObject || inBarrier)
            return false;

        if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, pickupVerticalRange, Physics.AllLayers - (1<<LayerMask.NameToLayer("TransparentFX"))) 
            || !hit.rigidbody || (hit.rigidbody.mass > 10.0f))
        {
            return false;
        }

        if (hit.transform.gameObject.GetComponent<MetallicObject>() && inMagneticBarrier)
            return false;

        Vector3 playerPosition = new Vector3(transform.position.x, transform.position.y - FirstPersonController.GroundedOffset, transform.position.z);
        Collider[] underObjects = Physics.OverlapBox(playerPosition, new Vector3(.2f, .5f, .1f), Quaternion.identity, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        
        foreach (Collider c in underObjects)
        {
            if (c.gameObject == hit.transform.gameObject)
            {
                return false;
            }
        }

        return true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (deadTimer > 0)
        {
            deadTimer -= Time.deltaTime;
            return;
        }
        else if (dead)
        {
            pauseMenu.RestartLevel();
            Respawn();
        }

        if (shotTimer > 0)
            shotTimer -= Time.deltaTime;

        if (_input.action)
        {
            _input.action = false;

            if (interactable)
            {
                if (interactable.StopInteract())
                    interactable = null;

                return;
            }
            if (heldObject)
            {
                DropObject(heldObject);
                return;
            }

            if (!attemptingInteract)
                StartCoroutine(AttemptInteract());
        }

        if (_input.scrollInAmount > 0)
        {
            if (heldObject && scrollAmount > 0)
                scrollAmount -= _input.scrollInAmount/1200;
        }

        if (_input.scrollOutAmount > 0)
        {
            if (heldObject && scrollAmount < 1)
                scrollAmount += _input.scrollOutAmount/1200;
        }

        if (_input.shrink)
        {
            _input.shrink = false;
            if (interactable)
                return;

            if (gun1.isActiveAndEnabled && !gun1.growCharged && !heldObject && !inBarrier && !inMagneticBarrier && shotTimer <= 0)
            {
                gun1.ShrinkBeam();
                shotTimer = shotLockoutTime;
            }
            else if (gun2.isActiveAndEnabled && !gun2.growCharged && !heldObject && !inBarrier && !inMagneticBarrier && shotTimer <= 0)
            {
                gun2.ShrinkBeam();
                shotTimer = shotLockoutTime;
            }

        }
        if (_input.grow)
        {
            _input.grow = false;
            if (interactable)
                return;

            if (gun2.isActiveAndEnabled && gun2.growCharged && !heldObject && !inBarrier && !inMagneticBarrier && shotTimer <= 0)
            {
                gun2.GrowBeam();
                shotTimer = shotLockoutTime;
            }
            else if (gun1.isActiveAndEnabled && gun1.growCharged && !heldObject && !inBarrier && !inMagneticBarrier && shotTimer <= 0)
            {
                gun1.GrowBeam();
                shotTimer = shotLockoutTime;
            }
        }

        if (heldObject)
        {
            if (Vector3.Distance(heldObject.transform.position, holdArea.position) > pickupHorizontalRange + pickupVerticalRange)
                DropObject(heldObject);
            else
                MoveObject();
        }

        if (CanInteract())
        {
            if (hit.rigidbody)
                interactText.text = "Press 'E' to pickup";
            else
                interactText.text = "Press 'E' to use";

            interactText.enabled = true;
        }
            
        else
            interactText.enabled = false;

    }

    IEnumerator AttemptInteract()
    {
        float attemptTimer = 0;
        float attemptTotal = .25f;
        float attemptDelay = .05f;
        attemptingInteract = true;

        while (attemptTimer < attemptTotal)
        {
            attemptTimer += attemptDelay;

            if (CanInteract())
            {
                if (hit.rigidbody)
                    PickupObject(hit.transform.gameObject);
                else
                {
                    interactable = hit.transform.gameObject.GetComponent<InteractableObject>();
                    interactable.Interact();
                }
                attemptTimer = .25f;
            }
            
            yield return new WaitForSeconds(attemptDelay);
        }

        attemptingInteract = false;
    }

    void MoveObject()
    {
        float heldY;

        heldY = mainCamera.transform.position.y + 1.75f * mainCamera.transform.forward.y;

        Vector3 heldVertical = new Vector3(0, heldY - heldObject.transform.position.y, 0);
        Vector3 cameraHorizontal = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        Vector3 heldHorizontal = new Vector3(heldObject.transform.position.x, 0, heldObject.transform.position.z);

        if (heldHorizontal != cameraHorizontal + mainCamera.transform.forward)
        {
            Vector3 forwardHorizontal = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized * pickupHorizontalRange;
            Vector3 moveDirection = heldVertical + cameraHorizontal + forwardHorizontal*(scrollAmount) - heldHorizontal;
            heldObjectRB.AddForce(moveDirection * pickupForce * heldObjectRB.mass);
        }

        float cameraYdiff = mainCamera.transform.eulerAngles.y - pickupCameraY;

        heldObject.transform.rotation = Quaternion.Euler(pickupRotation.eulerAngles.x, pickupRotation.eulerAngles.y + cameraYdiff, pickupRotation.eulerAngles.z);
    }

    void PickupObject(GameObject obj)
    {
        heldObjectRB = hit.transform.gameObject.GetComponent<Rigidbody>();
        heldObjectRB.mass /= 10;
        PickUpTrigger pickUpTrigger = obj.GetComponent<PickUpTrigger>();
        if (pickUpTrigger)
        {
            pickUpTrigger.triggered = true;
            pickUpTrigger.TriggerObjects();
        }

        if (obj.tag == "Powerup")
        {
            _persistentData.numberOfGuns++;
            obj.SetActive(false);

            if (_persistentData.numberOfGuns == 1)
            {
                gun1.gameObject.SetActive(true);
                gun1.firstTimeUse = true;
                _persistentData.OpenGate(pickUpTrigger.objs[0].transform.parent.parent.GetComponent<UniqueId>().uniqueId);
            }
            else if (_persistentData.numberOfGuns == 2)
            {
                gun2.gameObject.SetActive(true);
            }

            return;
        }

        if (gun1.gameObject.activeSelf)
            gun1.GunIn();
        if (gun2.gameObject.activeSelf)
            gun2.GunIn();

        pickupSound.Play();
        heldObjectRB.useGravity = false;
        origDrag = heldObjectRB.drag;
        heldObjectRB.drag = 5;
        heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;

        heldObject = obj;
        heldColliders = heldObject.GetComponents<Collider>();
        foreach (Collider c in heldColliders)
        {
            Physics.IgnoreCollision(c, transform.GetComponent<Collider>(), true);
            c.material.dynamicFriction = 0;
            c.material.staticFriction = 0;
        }

        pickupRotation = heldObject.transform.rotation;
        pickupCameraY = mainCamera.transform.eulerAngles.y;
        crosshair.SetActive(false);
        scrollAmount = 1;
        scrollText.enabled = true;
    }

    void DropObject(GameObject obj)
    {
        if (gun1.gameObject.activeSelf)
            gun1.GunOut();
        if (gun2.gameObject.activeSelf)
            gun2.GunOut();

        heldObjectRB.mass *= 10;
        heldObjectRB.useGravity = true;
        heldObjectRB.drag = origDrag;
        heldObjectRB.constraints = RigidbodyConstraints.None;
        heldObjectRB.velocity = Vector3.zero;
        foreach (Collider c in heldColliders)
        {
            Physics.IgnoreCollision(c, transform.GetComponent<Collider>(), false);
            c.material.dynamicFriction = 0.6f;
            c.material.staticFriction = 0.6f;
        }

        heldObject = null;

        crosshair.SetActive(true);
        scrollText.enabled = false;

        MetallicObject metal = obj.GetComponent<MetallicObject>();
        if (metal)
            metal.transform.localScale = metal.targetScale;
    }
}
