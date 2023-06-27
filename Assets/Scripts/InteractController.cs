using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] Transform holdArea;
    public static GameObject heldObject;
    private Rigidbody heldObjectRB;
    private Vector3 pickupPosition;

    private RaycastHit hit;
    private RaycastHit hitUnder;

    [SerializeField] Camera mainCamera;
    [SerializeField] private float pickupRange = 1.5f;
    [SerializeField] private float pickupForce = 150.0f;
    [SerializeField] private TMP_Text interactText;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    private bool CanPickUp()
    {
        if (heldObject)
            return false;

        if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, pickupRange) || !hit.rigidbody || (hit.rigidbody.mass > 10.0f))
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
                return;
            }
            else if (heldObject)
            {
                DropObject(heldObject);
                return;
            }
        }
        if (heldObject)
            MoveObject();

        if (heldObject && (Vector3.Distance(pickupPosition, heldObject.transform.localPosition) > 1f))
            DropObject(heldObject);

        if (CanPickUp())
            interactText.enabled = true;
        else
            interactText.enabled = false;

    }

    void MoveObject()
    {
        if (Vector3.Distance(heldObject.transform.position, holdArea.position) > 1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObject.transform.position).normalized;
            heldObjectRB.AddForce(moveDirection * pickupForce);
        }
    }

    void PickupObject(GameObject obj)
    {
        heldObjectRB.useGravity = false;
        heldObjectRB.drag = 10;
        heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;

        heldObject = obj;
        heldObjectRB.transform.parent = holdArea;
        pickupPosition = heldObject.transform.localPosition;
        Debug.Log($"pickupPosition: {pickupPosition}");
    }

    void DropObject(GameObject obj)
    {
        heldObjectRB.useGravity = true;
        heldObjectRB.drag = 1;
        heldObjectRB.constraints = RigidbodyConstraints.None;

        heldObjectRB.transform.parent = null;
        heldObject = null;
        pickupPosition = Vector3.zero;
    }
}
