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
    [SerializeField] private GameObject gun1;
    [SerializeField] private GameObject gun2;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject sceneLoader;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private Button continueButton;

    [SerializeField] private FadeText tutorialText;

    private PersistentData persistentData;
    public static bool loadingGame;

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
        if (persistentData)
        {
            gun1.SetActive(false); 
            gun2.SetActive(false);

            if (persistentData.numberOfGuns >= 1)
                gun1.SetActive(true);
            if (persistentData.numberOfGuns == 2)
                gun2.SetActive(true);
        }
        else
        {
            gun1.SetActive(false);
            gun2.SetActive(false);
        }
            
        crosshair.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Continue()
    {
        if (loadingGame)
            return;

        loadingGame = true;
        SaveData.LoadGame();
        StartCoroutine(loadGame(persistentData.playerLocation));

        Time.timeScale = 1;
        Cursor.visible = false;
    }

    public void NewGame()
    {
        if (loadingGame)
            return;

        loadingGame = true;
        persistentData.playerLocation = string.Empty;
        persistentData.openGates = new List<string>();
        persistentData.numberOfGuns = 0;
        StartCoroutine(loadGame("Intro"));

        tutorialText.ShowText("WASD to move, spacebar to jump");
        Time.timeScale = 1;
        Cursor.visible = false;
    }

    public void RollCredits()
    {
        // TODO: hide main menu, reveal credits text

        // TODO: hide credits text, show main menu
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

        persistentData = sceneLoader.GetComponent<PersistentData>();
        loadingGame = false;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        Cursor.visible = true;
    }

}
