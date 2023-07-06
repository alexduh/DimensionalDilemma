using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] private GameObject player;
    [SerializeField] private SceneLoader loader;
    [SerializeField] private GameObject mainCamera;
    private Scene Default;

    public void RestartLevel()
    {
        //SceneManager.sceneLoaded += OnRestart;
        string current = SceneManager.GetActiveScene().name;
        //SceneManager.UnloadSceneAsync(current);
        StartCoroutine(reloadScene(current));
    }

    IEnumerator reloadScene(string scene)
    {
        //UnityEngine.SceneManagement.Scene newScene = SceneManager.GetSceneByName(scene);
        GameObject[] objs = SceneManager.GetSceneByName(scene).GetRootGameObjects();
        //loader.SetScene(scene);
        foreach (GameObject go in objs)
        {
            if (go.tag == "Respawn")
            {
                player.transform.position = go.transform.position;
                player.transform.rotation = go.transform.rotation;
                SceneManager.MoveGameObjectToScene(mainCamera, Default);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
