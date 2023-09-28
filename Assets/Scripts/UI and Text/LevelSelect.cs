using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private PersistentData persistentData;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private PauseMenu pauseMenu;
    public bool visible = false;

    [SerializeField] private Gun gun1;
    [SerializeField] private Gun gun2;

    public void Enable()
    {
        visible = true;
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }

    public void Disable()
    {
        visible = false;
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void OnClick(GameObject clicked)
    {
        int gunsRequired = 0;
        if (clicked.transform.parent.gameObject.name == "1GunLevels")
            gunsRequired = 1;
        if (clicked.transform.parent.gameObject.name == "2GunLevels")
            gunsRequired = 2;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Default")
                SceneManager.UnloadSceneAsync(scene);
        }

        StartCoroutine(mainMenu.WrapperCoroutine(clicked.name, gunsRequired));
        pauseMenu.PauseGame(false);
        Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
