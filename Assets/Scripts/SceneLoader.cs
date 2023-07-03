using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    private StarterAssetsInputs _input;
    private GameObject player;
    private float sceneRenderDist = 75f;
    // Start is called before the first frame update

    public void SetScene(string scene)
    {
        Scene active = SceneManager.GetSceneByName(scene);
        SceneManager.SetActiveScene(active);
        AddScenes();
    }

    private void AddScenes()
    {
        foreach (Transform child in transform)
        {
            if (!SceneManager.GetSceneByName(child.name).IsValid())
            {
                if (Vector3.Distance(child.position, player.transform.position) < sceneRenderDist)
                    SceneManager.LoadScene(child.name, LoadSceneMode.Additive);
            }
            else if (Vector3.Distance(child.position, player.transform.position) >= sceneRenderDist)
            {
                SceneManager.UnloadSceneAsync(child.name);
            }
        }
    }

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (!Debug.isDebugBuild)
            SceneManager.LoadScene("Intro", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("r"))
            pauseMenu.RestartLevel();
    }
}
