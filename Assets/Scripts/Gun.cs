using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private RaycastHit hit;
    private float shotRange = 100.0f;
    private bool growCharged;

    [SerializeField] Camera _camera;
    private InteractController interactController;
    private MetallicObject lastHovered;
    public LayerMask inanimateLayers;

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        growCharged = false;
        interactController = GameObject.Find("Player").GetComponent<InteractController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_input.shrink)
        {
            _input.shrink = false;
            if (!growCharged && !interactController.heldObject)
                ShrinkBeam();

        }
        if (_input.grow)
        {
            _input.grow = false;
            if (growCharged && !interactController.heldObject)
                GrowBeam();
            
        }

        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            MetallicObject hoverObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (hoverObject && !hoverObject.resizing)
            {
                if (hoverObject.transform.localScale.x >= .5f && !growCharged)
                    hoverObject.FlashColor(Color.yellow);
                else if (hoverObject.transform.localScale.x <= 4f && growCharged)
                    hoverObject.FlashColor(Color.cyan);

                lastHovered = hoverObject;
            }
            else if (lastHovered)
            {
                lastHovered.ResetColor();
                lastHovered = null;
            }
        }
    }

    void ShrinkBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange))
        {
            MetallicObject shotObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shotObject)
            {
                growCharged = shotObject.Shrink();
            }
        }
    }

    void GrowBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange))
        {
            MetallicObject shotObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shotObject)
            {
                growCharged = !shotObject.Grow();
            }
        }
    }
}
