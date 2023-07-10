using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool firstTimeUse = false;
    private Animator anim;
    private AudioSource[] sounds;
    private StarterAssetsInputs _input;
    private RaycastHit hit;
    private float shotRange = 100.0f;
    private bool growCharged;

    [SerializeField] GunText gunText;
    [SerializeField] Camera _camera;
    private MetallicObject lastHovered;
    public LayerMask inanimateLayers;

    Renderer ren;
    Material mat;

    float emission;
    Color baseColor = new Color(.8658f, 1, 0);
    Color finalColor;

    public void GunIn()
    {
        anim.Play("GunIn");
    }

    public void GunOut()
    {
        anim.Play("GunOut");
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        growCharged = false;
        ren = GetComponent<Renderer>();
        mat = ren.materials[3];
        emission = 0;
        sounds = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_input.shrink)
        {
            _input.shrink = false;
            if (!growCharged && !InteractController.heldObject)
                ShrinkBeam();

        }
        if (_input.grow)
        {
            _input.grow = false;
            if (growCharged && !InteractController.heldObject)
                GrowBeam();
            
        }

        if (growCharged)
            emission = Mathf.PingPong(Time.time, 1.0f);
        else if (emission > 0)
            emission -= .01f;
        else
            emission = 0;

        finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor);

        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            MetallicObject hoverObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (lastHovered && lastHovered != hoverObject)
            {
                lastHovered.ResetColor();
                lastHovered = null;
            }
            if (!InteractController.heldObject && hoverObject && !hoverObject.resizing)
            {
                if (hoverObject.transform.localScale.x >= .5f && !growCharged)
                {
                    hoverObject.FlashColor(Color.yellow);
                    if (firstTimeUse)
                    {
                        gunText.ShowText("<sprite=0> to shrink metal object");
                    }
                }
                else if (hoverObject.transform.localScale.x <= 2f && growCharged)
                {
                    hoverObject.FlashColor(Color.cyan);
                }

                lastHovered = hoverObject;
            }
        }
    }

    void ShrinkBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            MetallicObject shotObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shotObject)
            {
                if (!shotObject.resizing && shotObject.transform.localScale.x >= .5f)
                {
                    sounds[0].Play();
                    growCharged = true;
                    shotObject.Shrink();
                    if (firstTimeUse)
                    {
                        gunText.ShowText("<sprite=1> to enlarge metal object");
                    }
                }
            }
        }
    }

    void GrowBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            MetallicObject shotObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shotObject)
            {
                if (!shotObject.resizing && shotObject.transform.localScale.x <= 2.0f)
                {
                    sounds[1].Play();
                    growCharged = false;
                    shotObject.Grow();
                    if (firstTimeUse)
                    {
                        gunText.showingText = false;
                        firstTimeUse = false;
                    }
                }
            }
        }
    }
}
