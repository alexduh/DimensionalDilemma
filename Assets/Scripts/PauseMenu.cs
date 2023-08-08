using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SceneLoader loader;
    private Scene Default;
    private string currentSceneName;
    public bool paused = false;

    public void PauseGame(bool toPause)
    {
        if (toPause)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            gameObject.SetActive(false);
        }

        paused = toPause;
    }

    public void RestartLevel()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(reloadScene(currentSceneName));
    }

    IEnumerator reloadScene(string scene)
    {
        GameObject[] objs = SceneManager.GetSceneByName(scene).GetRootGameObjects();
        foreach (GameObject go in objs)
        {
            if (go.tag == "Respawn")
            {
                player.transform.position = go.transform.position;
                player.transform.rotation = go.transform.rotation;
                yield return null;
            }
        }
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
    }

    // Start is called before the first frame update
    void Start()
    {
        Default = SceneManager.GetSceneByName("Default");
        Cursor.lockState = CursorLockMode.None;
    }

}
