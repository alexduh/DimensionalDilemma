using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetallicObject : MonoBehaviour
{
    private Vector3 startScale;
    public Vector3 targetScale;
    private float update;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public bool handlePhasing = false;
    public bool resizing;
    public GameObject originalParent;
    private Rigidbody rb;

    private AudioSource[] audioSources;
    Renderer ren;
    public Color startColor;

    private void ChangeSize(Vector3 startScale, Vector3 endScale, float time)
    {
        transform.localScale = Vector3.Lerp(startScale, endScale, time);
    }

    public void ResetColor()
    {
        ren.material.color = startColor;
    }

    public void FlashColor(Color endColor)
    {
        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time, 1));
    }

    public void Shrink()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        update = 0;
        startScale = transform.localScale;
        targetScale /= 2;
        rb.mass /= 8;
            
        resizing = true;
        return;
    }

    public void Grow()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        update = 0;
        startScale = transform.localScale;
        targetScale *= 2;
        rb.mass *= 8;
            
        resizing = true;
        return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float volume = collision.relativeVelocity.magnitude / 20;
        if (transform.localScale.x >= 1f)
        {
            audioSources[0].volume = volume;
            audioSources[0].Play();
        }
        else
        {
            audioSources[1].volume = volume;
            audioSources[1].Play();
        }
            
    }

    private void OnCollisionExit(Collision collision)
    {
        if (handlePhasing)
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
            handlePhasing = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        update = 0;
        targetScale = startScale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        resizing = false;
        audioSources = GetComponents<AudioSource>();
        ren = GetComponent<Renderer>();
        startColor = ren.material.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (resizing && update <= .5f)
        {
            update += Time.deltaTime;
            ChangeSize(startScale, targetScale, update * 2);
        }
        else if (resizing)
        {
            transform.localScale = new Vector3((float)Mathf.Round(targetScale.x * 100f) / 100f, (float)Mathf.Round(targetScale.y * 100f) / 100f, (float)Mathf.Round(targetScale.z * 100f) / 100f);
            resizing = false;
            //handlePhasing = true;
        }
            
    }
}
