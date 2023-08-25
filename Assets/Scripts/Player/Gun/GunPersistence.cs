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
        if (persistentData.hasGun)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
