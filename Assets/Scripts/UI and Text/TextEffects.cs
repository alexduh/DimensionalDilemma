using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class TextEffects : object
{
    private static bool flashingText;

    public static IEnumerator FlashText(TMP_Text text)
    {
        if (flashingText)
            yield break;

        flashingText = true;
        for (int textToggleCount = 0; textToggleCount < 5; textToggleCount++)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(.25f);
        }
        
        text.enabled = false;
        flashingText = false;
    }
}
