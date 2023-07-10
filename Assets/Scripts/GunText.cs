using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunText : MonoBehaviour
{
    public TMP_Text textBox;
    public bool showingText = false;
    private Color targetColor;
    private Color fadeOutColor;

    public void ShowText(string newText)
    {
        StopAllCoroutines();
        textBox.text = newText;
        showingText = true;
    }

    private void Start()
    {
        targetColor = new Color(textBox.color.r, textBox.color.g, textBox.color.b, 1);
        fadeOutColor = new Color(textBox.color.r, textBox.color.g, textBox.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (showingText)
        {
            textBox.color = Color.Lerp(fadeOutColor, targetColor, Mathf.PingPong(Time.time, 1));
        }
        else if (textBox.color.a >= 0)
        {
            textBox.color = new Color(textBox.color.r, textBox.color.g, textBox.color.b, textBox.color.a - Time.deltaTime);
        }

    }

}
