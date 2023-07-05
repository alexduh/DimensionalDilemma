using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetallicObject : MonoBehaviour
{
    private Vector3 scale;
    [SerializeField] private Vector3 targetScale;
    private float update;
    public bool resizing;
    public GameObject originalParent;
    private Rigidbody rb;

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

    public bool Shrink()
    {
        if (!resizing && transform.localScale.x >= .5f)
        {
            update = 0;
            scale = transform.localScale;
            targetScale = transform.localScale / 2;
            rb.mass /= 8;
            //TODO: play Shrink sound effect!
            
            resizing = true;
            return true;
        }

        return false;
    }

    public bool Grow()
    {
        if (!resizing && transform.localScale.x <= 2.0f)
        {
            update = 0;
            scale = transform.localScale;
            targetScale = transform.localScale * 2;
            rb.mass *= 8;
            //TODO: play Grow sound effect!
            
            resizing = true;
            return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        update = 0;
        scale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        resizing = false;
        ren = GetComponent<Renderer>();
        startColor = ren.material.color;
        //originalParent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (resizing && update <= .5f)
        {
            update += Time.deltaTime;
            ChangeSize(scale, targetScale, update * 2);
        }
        else if (resizing)
        {
            transform.localScale = new Vector3((float)Mathf.Round(targetScale.x * 100f) / 100f, (float)Mathf.Round(targetScale.y * 100f) / 100f, (float)Mathf.Round(targetScale.z * 100f) / 100f);
            resizing = false;
        }
            
    }
}
