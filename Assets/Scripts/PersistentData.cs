using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersistentData : MonoBehaviour
{
    public string playerLocation;
    public bool hasGun = false;
    public List<string> openGates;

    public void OpenGate(string gateID)
    {
        if (!openGates.Contains(gateID))
        {
            openGates.Add(gateID);
            SaveData.SaveGame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerLocation == null)
            playerLocation = string.Empty;
        if (openGates == null)
            openGates = new List<string>();
    }
}
