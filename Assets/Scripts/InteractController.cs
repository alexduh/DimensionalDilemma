using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] Transform holdArea;
    private GameObject heldObject;
    private Rigidbody heldObjectRB;

    private RaycastHit hit;

    [SerializeField] Camera mainCamera;
    [SerializeField] private float pickupRange = 1f;
    [SerializeField] private float pickupForce = 150.0f;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.action)
        {
            if (!heldObject)
            {
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, pickupRange))
                {
                    PickupObject(hit.transform.gameObject);
                }

            }
            else
            {
                DropObject(heldObject);
            }

            if (heldObject)
                MoveObject();

            _input.action = false;
        }

        if (heldObject && Vector3.Distance(transform.position, heldObject.transform.position) > pickupRange)
        {
            DropObject(heldObject);
        }
    }

    void MoveObject()
    {
        if (Vector3.Distance(heldObject.transform.position, holdArea.position) > .1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObject.transform.position).normalized;
            heldObjectRB.AddForce(moveDirection * pickupForce);
        }
    }

    void PickupObject(GameObject obj)
    {
        heldObjectRB = obj.GetComponent<Rigidbody>();
        if (heldObjectRB)
        {
            heldObjectRB.useGravity = false;
            heldObjectRB.drag = 10;
            heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjectRB.transform.parent = holdArea;
            heldObject = obj;
        }
        
    }

    void DropObject(GameObject obj)
    {
        heldObjectRB.useGravity = true;
        heldObjectRB.drag = 1;
        heldObjectRB.constraints = RigidbodyConstraints.None;

        heldObjectRB.transform.parent = null;
        heldObject = null;
    }
}
