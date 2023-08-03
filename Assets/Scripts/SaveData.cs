using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData : MonoBehaviour
{
    static PersistentData persistentData;
    public string playerLocation;
    public List<string> openGates;

    // Gates opened: {openGates.Count}

    public static void SaveGame()
    {
        if (persistentData == null)
            return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        
        bf.Serialize(file, persistentData);
        file.Close();
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            persistentData.playerLocation = data.playerLocation;
            persistentData.openGates = data.openGates;
        }
        else
            Debug.LogError("There is no save data!");
    }

    // Start is called before the first frame update
    void Start()
    {
        persistentData = GameObject.Find("SceneLoader").GetComponent<PersistentData>();
    }
}
