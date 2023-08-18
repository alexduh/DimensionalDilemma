using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject sceneLoader;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private Button continueButton;
    
    private PersistentData persistentData;
    private bool loadingGame;

    IEnumerator loadGame(string startScene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(startScene));
        pauseMenu.GetLocation(startScene);
        sceneLoader.GetComponent<SceneLoader>().SetScene(startScene);
        if (persistentData && persistentData.hasGun)
            gun.SetActive(true);

        crosshair.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Continue()
    {
        if (loadingGame)
            return;

        loadingGame = true;
        SaveData.LoadGame();
        persistentData = sceneLoader.GetComponent<PersistentData>();
        StartCoroutine(loadGame(persistentData.playerLocation));

        Time.timeScale = 1;
        Cursor.visible = false;
    }

    public void NewGame()
    {
        if (loadingGame)
            return;

        loadingGame = true;
        StartCoroutine(loadGame("Intro"));

        Time.timeScale = 1;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "SaveData.dat")))
            continueButton.interactable = false; // no save file, can't continue

        loadingGame = false;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        Cursor.visible = true;
    }

}
