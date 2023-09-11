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
    public bool resizing;
    public GameObject originalParent;
    private Rigidbody rb;

    private AudioSource[] audioSources;
    private Color origColor;
    private List<Color> toggleColors;
    Renderer ren;

    private void ChangeSize(Vector3 startScale, Vector3 endScale, float time)
    {
        transform.localScale = Vector3.Lerp(startScale, endScale, time);
    }

    public void ResetColor()
    {
        ren.material.color = origColor;
        toggleColors.Clear();
    }

    public void AddColor(Color newColor)
    {
        if (!toggleColors.Contains(newColor))
            toggleColors.Add(newColor);
        else if (!toggleColors.Contains(origColor))
            toggleColors.Add(origColor);
    }

    private void FlashColor()
    {
        if (toggleColors.Count < 2)
            return;

        ren.material.color = Color.Lerp(toggleColors[0], toggleColors[1], Mathf.PingPong(Time.time, 1));
    }

    public void Shrink()
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        update = 0;
        startScale = transform.localScale;
        targetScale /= 2;
        rb.mass /= 10;
            
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
        rb.mass *= 10;
            
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

    // Start is called before the first frame update
    void Start()
    {
        update = 0;
        targetScale = startScale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        resizing = false;
        audioSources = GetComponents<AudioSource>();
        ren = GetComponent<Renderer>();
        origColor = ren.material.color;
        toggleColors = new List<Color>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (toggleColors.Count > 0)
            FlashColor();

        float resizeTime = Mathf.Abs(startScale.x - targetScale.x);
        if (resizing && update <= resizeTime)
        {
            update += Time.deltaTime;
            ChangeSize(startScale, targetScale, update/resizeTime);
        }
        else if (resizing)
        {
            transform.localScale = new Vector3((float)Mathf.Round(targetScale.x * 100f) / 100f, (float)Mathf.Round(targetScale.y * 100f) / 100f, (float)Mathf.Round(targetScale.z * 100f) / 100f);
            resizing = false;
        }
            
    }
}
