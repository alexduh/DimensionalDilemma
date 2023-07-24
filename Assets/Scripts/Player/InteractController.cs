using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

public class InteractController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] Transform holdArea;
    [SerializeField] GameObject crosshair;
    public static GameObject heldObject;
    private Rigidbody heldObjectRB;
    private Quaternion pickupRotation;

    private RaycastHit hit;

    [SerializeField] Gun gun;
    [SerializeField] Camera mainCamera;
    private float pickupHorizontalRange = 1.5f;
    private float pickupVerticalRange = 2f;
    [SerializeField] private float pickupForce = 100.0f;
    [SerializeField] private TMP_Text interactText;

    private bool inBarrier = false;
    private bool inMagneticBarrier = false;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            inBarrier = true;
            if (heldObject)
            {
                DropObject(heldObject);
            }

            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("MagneticBarrier"))
        {
            inMagneticBarrier = true;
            // TODO: lose gun charges and refund to sources!

            // TODO: drop heldObject depending on barrier type

            if (heldObject && heldObject.GetComponent<MetallicObject>())
            {
                DropObject(heldObject);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("MagneticBarrier") || other.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            inMagneticBarrier = false;
            inBarrier = false;
        }
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
        Collider[] underObjects = Physics.OverlapBox(playerPosition, new Vector3(.2f, .25f, .1f), Quaternion.identity, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        
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

        heldObject.transform.rotation = Quaternion.Euler(pickupRotation.eulerAngles.x, pickupRotation.eulerAngles.z, -pickupRotation.eulerAngles.y);
    }

    void PickupObject(GameObject obj)
    {
        if (obj.tag == "Powerup")
        {
            gun.gameObject.SetActive(true);
            gun.GunOut();
            obj.SetActive(false);
            gun.firstTimeUse = true;
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
        heldObjectRB.transform.parent = holdArea;
        pickupRotation = heldObject.transform.rotation;
        crosshair.SetActive(false);
    }

    void DropObject(GameObject obj)
    {
        if (gun.gameObject.activeSelf)
        {
            gun.GunOut();
        }

        heldObjectRB.useGravity = true;
        heldObjectRB.drag = 1;
        heldObjectRB.constraints = RigidbodyConstraints.None;

        heldObjectRB.transform.parent = null;
        heldObject = null;
        crosshair.SetActive(true);

        MetallicObject metal = obj.GetComponent<MetallicObject>();
        if (metal)
            metal.transform.localScale = metal.targetScale;
    }
}
