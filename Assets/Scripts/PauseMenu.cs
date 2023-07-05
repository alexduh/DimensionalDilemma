using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] private GameObject player;
    [SerializeField] private SceneLoader loader;
    [SerializeField] private GameObject mainCamera;

    public void RestartLevel()
    {
        //SceneManager.sceneLoaded += OnRestart;
        string current = SceneManager.GetActiveScene().name;
        StartCoroutine(reloadScene(current));
    }

    void OnRestart(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnRestart;
        GameObject[] objs = scene.GetRootGameObjects();
        loader.SetScene(scene.name);
        foreach (GameObject go in objs)
        {
            if (go.tag == "Respawn")
            {
                player.transform.position = go.transform.position;
                player.transform.rotation = go.transform.rotation;
                SceneManager.MoveGameObjectToScene(mainCamera, scene);
                return;
            }
        }
        
    }

    IEnumerator reloadScene(string scene)
    {
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

        Debug.Log("asyncLoad completed!");
        UnityEngine.SceneManagement.Scene newScene = SceneManager.GetSceneByName(scene);
        GameObject[] objs = newScene.GetRootGameObjects();
        loader.SetScene(scene);
        foreach (GameObject go in objs)
        {
            if (go.tag == "Respawn")
            {
                player.transform.position = go.transform.position;
                player.transform.rotation = go.transform.rotation;
                SceneManager.MoveGameObjectToScene(mainCamera, newScene);
                yield break;
            }
        }
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
