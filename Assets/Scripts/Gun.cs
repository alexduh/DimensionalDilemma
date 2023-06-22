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

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        growCharged = false;
        interactController = GameObject.Find("Player").GetComponent<InteractController>();
    }

    // Update is called once per frame
    void Update()
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
    }

    void ShrinkBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange))
        {
            MetallicObject shot = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shot)
            {
                growCharged = shot.Shrink();
            }
        }
    }

    void GrowBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange))
        {
            MetallicObject shot = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shot)
            {
                growCharged = !shot.Grow();
            }
        }
    }
}
