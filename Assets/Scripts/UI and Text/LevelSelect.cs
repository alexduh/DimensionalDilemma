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

    public void Enable()
    {
        visible = true;

        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        foreach (Transform child in transform)
            foreach (Transform child2 in child)
                child2.GetComponent<Button>().interactable = true;

        if (persistentData.numberOfGuns < 1)
            foreach (Transform child in transform.Find("1GunLevels"))
                child.GetComponent<Button>().interactable = false;
        if (persistentData.numberOfGuns < 2)
            foreach (Transform child in transform.Find("2GunLevels"))
                child.GetComponent<Button>().interactable = false;
    }

    public void Disable()
    {
        visible = false;
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void OnClick(GameObject clicked)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Default")
                SceneManager.UnloadSceneAsync(scene);
        }

        StartCoroutine(mainMenu.WrapperCoroutine(clicked.name));
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
