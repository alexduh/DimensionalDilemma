using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateTrigger : Trigger
{
    float startY;
    Rigidbody rb;
    Material mat;
    AudioSource triggerSound;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;
        triggerSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < startY)
            rb.AddForce(Vector3.up * rb.mass/2);
        else
        {
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }

        if (transform.position.y <= startY - .03f * transform.localScale.x)
        {
            if (triggered)
                return;

            mat.color = Color.green;
            triggered = true;
            triggerSound.Play();
            TriggerObjects();
        }
        else
        {
            if (!triggered)
                return;

            mat.color = Color.red;
            triggered = false;
            TriggerObjects();
        }
    }
}
