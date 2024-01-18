using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class TextEffects : object
{
    private static bool flashingText;

    public static IEnumerator FlashText(GameObject textObject)
    {
        if (flashingText)
            yield break;

        flashingText = true;
        for (int textToggleCount = 0; textToggleCount < 5; textToggleCount++)
        {
            textObject.SetActive(!textObject.activeSelf);
            yield return new WaitForSeconds(.25f);
        }
        
        textObject.SetActive(false);
        flashingText = false;
    }
}
