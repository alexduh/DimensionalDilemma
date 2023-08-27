using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    private float lifetime = 1;
    private GameObject destroyer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetDestructor(GameObject go)
    {
        destroyer = go;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == destroyer)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += 3 * transform.up;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
    }
}
