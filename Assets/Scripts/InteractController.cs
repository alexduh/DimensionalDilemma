using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] Transform holdArea;
    public GameObject heldObject;
    private Rigidbody heldObjectRB;
    private Vector3 pickupPosition;

    private RaycastHit hit;

    [SerializeField] Camera mainCamera;
    [SerializeField] private float pickupRange = 1.5f;
    [SerializeField] private float pickupForce = 150.0f;
    [SerializeField] private TMP_Text interactText;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_input.action)
        {
            _input.action = false;
            if (!heldObject)
            {
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, pickupRange) && hit.rigidbody && (hit.rigidbody.mass <= 10.0f))
                {
                    heldObjectRB = hit.transform.gameObject.GetComponent<Rigidbody>();
                    if (heldObjectRB)
                    {
                        PickupObject(hit.transform.gameObject);
                        return;
                    }
                }
            }
            else
            {
                DropObject(heldObject);
                return;
            }
        }
        if (heldObject)
            MoveObject();

        if (heldObject && (Vector3.Distance(pickupPosition, heldObject.transform.localPosition) > 1f))
            DropObject(heldObject);

        if (!heldObject && Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, pickupRange) && hit.rigidbody && (hit.rigidbody.mass <= 10.0f))
            interactText.enabled = true;
        else
            interactText.enabled = false;

        //if (Physics.CheckBox(transform.position, new Vector3(.2f, .25f, .1f), Quaternion.identity, LayerMask., QueryTriggerInteraction.Ignore))


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
