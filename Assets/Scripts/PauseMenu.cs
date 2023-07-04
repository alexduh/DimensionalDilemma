using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] private GameObject player;
    [SerializeField] private SceneLoader loader;

    public void RestartLevel()
    {
        SceneManager.sceneLoaded += OnRestart;
        string current = SceneManager.GetActiveScene().name;
        SceneManager.UnloadSceneAsync(current);
        SceneManager.LoadScene(current, LoadSceneMode.Additive);
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
                return;
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
