using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : DoorTrigger
{
    float startY;
    Rigidbody rb;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < startY)
            rb.AddForce(Vector3.up * rb.mass/2);
        else
            rb.velocity = Vector3.zero;

        if (transform.position.y <= startY - .15f)
        {
            mat.color = Color.green;
            triggered = true;
        }
        else
        {
            mat.color = Color.red;
            triggered = false;
        }
    }
}
