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
    private LineRenderer lr;
    private float shotRange = 100.0f;
    private float shotLifetime = .05f;
    private float shotTimer;

    private bool dropSoundPlayed = false;
    public bool growCharged;
    private MetallicObject lastShot;

    [SerializeField] InteractController player;
    [SerializeField] FadeText gunText;
    [SerializeField] Camera _camera;
    Vector3 startPos;
    Quaternion startRot;
    Rigidbody rb;
    private Transform gunHolder;
    private MetallicObject lastHovered;
    public LayerMask inanimateLayers;

    Renderer ren;
    Material mat;

    float emission;
    Color baseColor = new Color(.8658f, 1, 0);
    Color finalColor;

    // Called when player dies
    public void SetGunConnected(bool connected)
    {
        if (!isActiveAndEnabled)
            return;

        rb.useGravity = !connected;
        GetComponent<Collider>().enabled = !connected;
        if (connected)
        {
            dropSoundPlayed = false;
            transform.parent = gunHolder;
            transform.localPosition = startPos;
            transform.localRotation = startRot;
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }            
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            transform.parent = null;
        }
    }

    public void GunIn()
    {
        anim.Play("GunIn");
    }

    public void GunOut()
    {
        anim.Play("GunOut");
    }

    private void OnEnable()
    {
        GunOut();
    }

    private void Awake()
    {
        anim = transform.parent.GetComponent<Animator>();
        gunHolder = transform.parent;
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        growCharged = false;
        ren = GetComponent<Renderer>();
        lr = GetComponent<LineRenderer>();
        mat = ren.materials[3];
        emission = 0;
        sounds = GetComponents<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!dropSoundPlayed)
        {
            sounds[2].Play();
            dropSoundPlayed = true;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shotTimer > 0)
            shotTimer -= Time.deltaTime;
        else if (lr.enabled)
            lr.enabled = false;

        if (growCharged)
        {
            emission = Mathf.PingPong(Time.time, 1.0f);
            if (player.inBarrier || player.inMagneticBarrier)
                RefundCharge();
        }
            
        else if (emission > 0)
            emission -= .01f;
        else
            emission = 0;

        finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor);

        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            MetallicObject hoverObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (lastHovered && (player.inBarrier || player.inMagneticBarrier || lastHovered != hoverObject || hoverObject.resizing))
            {
                lastHovered.ResetColor();
                lastHovered = null;
            }
            if (!InteractController.heldObject && hoverObject && !player.inBarrier && !player.inMagneticBarrier)
            {
                if (!growCharged)
                {
                    if (hoverObject.targetScale.x >= .5f)
                    {
                        hoverObject.AddColor(Color.yellow);
                        if (firstTimeUse)
                        {
                            gunText.ShowText("<sprite=0> to shrink metal object");
                        }
                    }
                    else
                        hoverObject.AddColor(Color.red);
                }
                else if (growCharged)
                {
                    if (hoverObject.targetScale.x <= 2f)
                        hoverObject.AddColor(Color.cyan);
                    else
                        hoverObject.AddColor(Color.red);
                }

                lastHovered = hoverObject;
            }
        }
    }

    void RefundCharge()
    {
        if (lastShot.targetScale.x < 4f)
        {
            // TODO: play unique sound effect representing this effect!
            // TODO: display VFX (teal circles around gun) to indicate refund!
            sounds[1].Play();
            growCharged = false;
            lastShot.Grow();
            lastShot = null;
        }
    }

    private void SpawnLaser(Color color)
    {
        sounds[3].Play();
        lr.startColor = color;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, hit.point);
        lr.enabled = true;
        shotTimer = shotLifetime;

    }

    public void ShrinkBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            SpawnLaser(Color.yellow);
            MetallicObject shotObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shotObject)
            {
                if (shotObject.targetScale.x >= .5f)
                {
                    lastShot = shotObject;
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

    public void GrowBeam()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.TransformDirection(Vector3.forward), out hit, shotRange, inanimateLayers))
        {
            SpawnLaser(Color.cyan);
            MetallicObject shotObject = hit.transform.gameObject.GetComponent<MetallicObject>();
            if (shotObject)
            {
                if (shotObject.targetScale.x <= 2.0f)
                {
                    lastShot = null;
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
