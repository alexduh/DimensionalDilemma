using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPersistence : MonoBehaviour
{
    private PersistentData persistentData;

    // Start is called before the first frame update
    void Start()
    {
        persistentData = GameObject.Find("SceneLoader").GetComponent<PersistentData>();
        if (gameObject.scene.name == "GunRoom1")
            gameObject.SetActive(persistentData.numberOfGuns < 1);
        if (gameObject.scene.name == "GunRoom2")
            gameObject.SetActive(persistentData.numberOfGuns < 2);

    }
}
