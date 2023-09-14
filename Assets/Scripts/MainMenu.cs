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
    [SerializeField] private TMP_Text creditsText;
    private RectTransform creditsTranform;

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
        gameObject.SetActive(true);
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        creditsText.gameObject.SetActive(true);
        creditsTranform.localPosition = new Vector3(0, -1000, 0);

        StartCoroutine(ScrollText());
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
        creditsTranform = creditsText.GetComponent<RectTransform>();
    }

    IEnumerator ScrollText()
    {
        while (creditsTranform.localPosition.y < 1000)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
                creditsTranform.localPosition = new Vector3(0, 1000, 0);

            creditsTranform.Translate(Vector3.up / 5);
            yield return null;
        }

        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
        creditsText.gameObject.SetActive(false);
        Image square = GameObject.FindWithTag("Canvas").transform.Find("BlackSquare").GetComponent<Image>();
        square.color = new Color(square.color.r, square.color.g, square.color.b, 0);

        if (InteractController.interactable)
        {
            InteractController.interactable.StopInteract();
            InteractController.interactable = null;
        }

        pauseMenu.QuitToMainMenu();
    }

}
