using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData : MonoBehaviour
{
    static PersistentData persistentData = GameObject.Find("SceneLoader").GetComponent<PersistentData>();
    public string playerLocation;
    public bool hasGun;
    public List<string> openGates;

    // Gates opened: {openGates.Count}

    public static void SaveGame()
    {
        if (persistentData == null)
            return;

        string fileLocation = Path.Combine(Application.persistentDataPath, "SaveData.dat");
        FileStream file = File.Create(fileLocation);
        file.Close();
        File.WriteAllText(fileLocation, JsonUtility.ToJson(persistentData));
    }

    public static void LoadGame()
    {
        string fileLocation = Path.Combine(Application.persistentDataPath, "SaveData.dat");
        if (File.Exists(fileLocation))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(fileLocation), persistentData);
            /*persistentData.playerLocation = data.playerLocation;
            persistentData.openGates = data.openGates;
            persistentData.hasGun = data.hasGun;
            */
        }
        else
            Debug.LogError("There is no save data!");
    }

    // Start is called before the first frame update
    void Start()
    {
        //persistentData = GameObject.Find("SceneLoader").GetComponent<PersistentData>();
    }
}
