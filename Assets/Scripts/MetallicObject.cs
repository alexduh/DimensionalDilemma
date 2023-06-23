using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetallicObject : MonoBehaviour
{
    private Vector3 scale;
    [SerializeField] private Vector3 targetScale;
    private float update;
    public GameObject originalParent;
    private Rigidbody rb;

    private void ChangeSize(Vector3 startScale, Vector3 endScale, float time)
    {
        transform.localScale = Vector3.Lerp(startScale, endScale, time);
    }

    public bool Shrink()
    {
        if (update > .5f && transform.localScale.x >= .5f)
        {
            update = 0;
            scale = transform.localScale;
            targetScale = transform.localScale / 2;
            rb.mass /= 8;
            //TODO: play Shrink sound effect!
            return true;
        }

        return false;
    }

    public bool Grow()
    {
        if (update > .5f && transform.localScale.x <= 4f)
        {
            update = 0;
            scale = transform.localScale;
            targetScale = transform.localScale * 2;
            rb.mass *= 8;
            //TODO: play Grow sound effect!
            return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        update = 1;
        scale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        //originalParent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (update <= .5f)
        {
            update += Time.deltaTime;
            ChangeSize(scale, targetScale, update * 2);
        }
        else
            transform.localScale = targetScale;
    }
}
