using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InteractController _interact;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private LevelSelect levelList;
    [SerializeField] private SceneLoader loader;
    [SerializeField] private Gun[] guns;
    [SerializeField] private Transform buttons;

    [SerializeField] private PersistentData persistentData;
    [SerializeField] private TMP_Text gateCount;
    [SerializeField] private TMP_Text hintBox;
    [SerializeField] private TMP_Text puzzleName;
    [SerializeField] private FadeText tutorialText;

    private Dictionary<string, string> hintDict;

    private string currentSceneName;
    public bool paused = false;
    public static bool demoEnabled = false;

    private void ShowPauseMenu(bool visible)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(visible);

        foreach (Transform child in buttons)
            if (child.gameObject.name == "LevelSelect" || child.gameObject.name == "Hint")
                child.gameObject.SetActive(visible && demoEnabled);

        if (!visible)
            hintBox.gameObject.SetActive(false);
    }

    public void PauseOrBack()
    {
        if (!paused)
            PauseGame(true);
        else if (levelList.visible)
        {
            levelList.Disable();
            ShowPauseMenu(true);
        }
        else if (optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            ShowPauseMenu(true);
        }
        else
            PauseGame(false);
    }

    public void PauseGame(bool toPause)
    {
        if (toPause)
        {
            Time.timeScale = 0;
            puzzleName.color = new Color(puzzleName.color.r, puzzleName.color.b, puzzleName.color.g, 255);
            Cursor.visible = true;
            gateCount.text = persistentData.openGates.Count.ToString() + "/28";
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
        }

        ShowPauseMenu(toPause);
        paused = toPause;
    }

    void ResetPlayerState()
    {
        _interact.inMagneticBarrier = false;
        if (InteractController.interactable)
        {
            InteractController.interactable.StopInteract();
            InteractController.interactable = null;
        }
        if (InteractController.heldObject)
            _interact.DropObject(InteractController.heldObject);
    }

    public void RestartLevel()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Prerequisite")
            StartCoroutine(reloadScene("Postrequisite"));
        if (currentSceneName == "Postrequisite")
            StartCoroutine(reloadScene("Prerequisite"));

        ResetPlayerState();

        StartCoroutine(reloadScene(currentSceneName));
        loader.ShowSceneName();
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        ShowPauseMenu(false);
    }

    public void LevelSelect()
    {
        levelList.Enable();
        ShowPauseMenu(false);
    }

    public void Hint()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        if (hintDict.ContainsKey(currentSceneName))
        {
            hintBox.text = hintDict[currentSceneName];
            hintBox.gameObject.SetActive(true);
        }
    }

    public void QuitToMainMenu()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Default")
                SceneManager.UnloadSceneAsync(scene);
        }

        ResetPlayerState();

        guns[0].gameObject.SetActive(false);
        guns[1].gameObject.SetActive(false);
        puzzleName.color = new Color(puzzleName.color.r, puzzleName.color.g, puzzleName.color.b, 0);

        tutorialText.showingText = false;
        GameObject.FindWithTag("Canvas").transform.Find("MainMenu").GetComponent<MainMenu>().EnableChildren();
        PauseGame(false);
        Time.timeScale = 0;
        Cursor.visible = true;
        MainMenu.loadingGame = false;
    }

    public void GetLocation(string scene)
    {
        GameObject[] objs = SceneManager.GetSceneByName(scene).GetRootGameObjects();
        foreach (GameObject go in objs)
        {
            if (go.tag == "Respawn")
            {
                player.transform.position = go.transform.position;
                player.transform.rotation = go.transform.rotation;

                foreach (Gun gun in guns)
                    gun.growCharged = false;
            }
        }
    }

    IEnumerator reloadScene(string scene)
    {
        GetLocation(scene);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        PauseGame(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        hintDict = new Dictionary<string, string>
        {
            { "Intro", "Press 'E' while facing an object to pick it up, then press 'E' again to drop it" },
            { "Beginnings", "Buttons must be held down simultaneously to progress. Try activating the upper one first" },
            { "Incline", "Gates remain open while their buttons are held down" },
            { "Metal", "Two objects are required to reach the lower button, but metal objects are blocked by the force field" },
            { "Mass", "Pay attention to the dimensional dilator's glow! It will glow yellow when charged" },
            { "Divider", "Force fields prevent transferring of dilator charges" },
            { "Momentum", "Momentum is the product of the mass of a particle and its velocity" },
            { "Narrow", "Balls can be moved while standing above" },
            { "Reach", "Each size increase/decrease has a doubling effect on dimensions" },
            { "Trenches", "Metal objects can be useful even when out of reach" },
            { "Perspective", "Try rotating objects for a different perspective" },
            { "Elevate", "Large objects cannot be easily climbed onto" },
            { "Navigate", "Crossing through force fields will remove any charge from the dimensional dilator" },
            { "Allocate", "Force fields act as solid structures against metal objects" },
            { "Prerequisite", "Rotating an object is easier done using other objects" },
            { "Postrequisite", "Think outside the box!" },
            { "Opening", "Metal objects can be interacted with as long as there is a line of sight" },
            { "Playground", "An object in motion will remain in motion until acted upon by an unbalanced force" },
            { "Jammed", "Try approaching from above" },
            { "Push", "Heavy objects cannot be moved directly; other objects may be able to affect them" },
            { "Containment", "When passing through a force field, each dilator will attempt to restore its charge to the original object" },
        };
    }

}
