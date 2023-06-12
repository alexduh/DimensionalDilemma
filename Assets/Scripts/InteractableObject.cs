using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private float scale;
    public GameObject originalParent;

    void Shrink()
    {
        if (this.transform.localScale.x >= .5f)
        {
            this.transform.localScale /= 2;
        }
    }

    void Grow()
    {
        if (this.transform.localScale.x <= 8f)
        {
            this.transform.localScale *= 2;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scale = this.transform.localScale.x;
        originalParent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //if ()
    }
}
