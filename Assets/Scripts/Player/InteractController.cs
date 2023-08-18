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
    public GameObject underObject;
    private Rigidbody heldObjectRB;
    private Collider[] heldColliders;
    private Quaternion pickupRotation;
    private float pickupCameraY;
    private float inObjectTime = 0;
    private float deadTimer = 0;
    private bool dead = false;

    private PersistentData _persistentData;
    [SerializeField] private SceneLoader sceneloader;
    [SerializeField] private PauseMenu pauseMenu;

    private RaycastHit hit;

    [SerializeField] Gun gun;
    [SerializeField] Camera mainCamera;
    private float pickupHorizontalRange = 2f;
    private float pickupVerticalRange = 2f;
    [SerializeField] private float pickupForce = 200.0f;
    [SerializeField] private TMP_Text interactText;

    public bool inBarrier = false;
    public bool inMagneticBarrier = false;

    [SerializeField] private AudioSource crushSound;

    // Start is called before the first frame update
    void Start()
    {
        _persistentData = sceneloader.GetComponent<PersistentData>();
        _input = GetComponent<StarterAssetsInputs>();
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
        gun.GunConnected(false);

        dead = true;
        deadTimer = 5;
    }

    private void Respawn()
    {
        GetComponent<CapsuleCollider>().radius = .45f;
        GetComponent<FirstPersonController>().enabled = true;
        GetComponent<PlayerInput>().enabled = true;

        gun.GunConnected(true);

        dead = false;
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

        if (_input.action)
        {
            _input.action = false;
            if (CanPickUp())
            {
                heldObjectRB = hit.transform.gameObject.GetComponent<Rigidbody>();
                PickupObject(hit.transform.gameObject);
                if (hit.transform.gameObject.GetComponent<PickUpTrigger>())
                {
                    hit.transform.gameObject.GetComponent<PickUpTrigger>().triggered = true;
                }

                return;
            }
            else if (heldObject)
            {
                DropObject(heldObject);
                return;
            }
        }

        if (_input.shrink)
        {
            _input.shrink = false;
            if (!gun.growCharged && !heldObject && !inBarrier && !inMagneticBarrier)
                gun.ShrinkBeam();

        }
        if (_input.grow)
        {
            _input.grow = false;
            if (gun.growCharged && !heldObject && !inBarrier && !inMagneticBarrier)
                gun.GrowBeam();

        }

        if (heldObject)
        {
            if (Vector3.Distance(heldObject.transform.position, holdArea.position) > pickupHorizontalRange + pickupVerticalRange)
                DropObject(heldObject);
            else
                MoveObject();
        }

        if (CanPickUp())
            interactText.enabled = true;
        else
            interactText.enabled = false;

    }

    void MoveObject()
    {
        float heldY;

        heldY = mainCamera.transform.position.y + mainCamera.transform.forward.y;

        Vector3 heldVertical = new Vector3(0, heldY - heldObject.transform.position.y, 0);
        Vector3 cameraHorizontal = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        Vector3 heldHorizontal = new Vector3(heldObject.transform.position.x, 0, heldObject.transform.position.z);

        if (heldHorizontal != cameraHorizontal + mainCamera.transform.forward)
        {
            Vector3 forwardHorizontal = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized * pickupHorizontalRange;
            Vector3 moveDirection = heldVertical + cameraHorizontal + forwardHorizontal - heldHorizontal;
            heldObjectRB.AddForce(moveDirection * pickupForce * heldObjectRB.mass);
        }

        float cameraYdiff = mainCamera.transform.eulerAngles.y - pickupCameraY;

        heldObject.transform.rotation = Quaternion.Euler(pickupRotation.eulerAngles.x, pickupRotation.eulerAngles.y + cameraYdiff, pickupRotation.eulerAngles.z);
    }

    void PickupObject(GameObject obj)
    {
        PickUpTrigger pickUpTrigger = obj.GetComponent<PickUpTrigger>();
        if (pickUpTrigger)
        {
            pickUpTrigger.triggered = true;
            pickUpTrigger.TriggerObjects();
        }

        if (obj.tag == "Powerup")
        {
            gun.gameObject.SetActive(true);
            gun.GunOut();
            obj.SetActive(false);
            gun.firstTimeUse = true;

            _persistentData.hasGun = true;

            _persistentData.OpenGate(pickUpTrigger.objs[0].transform.parent.parent.GetComponent<UniqueId>().uniqueId);

            //SaveData.SaveGame();
            return;
        }

        if (gun.gameObject.activeSelf)
        {
            gun.GunIn();
        }
        
        heldObjectRB.useGravity = false;
        heldObjectRB.drag = 10;
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
    }

    void DropObject(GameObject obj)
    {
        if (gun.gameObject.activeSelf)
        {
            gun.GunOut();
        }

        heldObjectRB.useGravity = true;
        heldObjectRB.drag = .25f;
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

        MetallicObject metal = obj.GetComponent<MetallicObject>();
        if (metal)
            metal.transform.localScale = metal.targetScale;
    }
}
