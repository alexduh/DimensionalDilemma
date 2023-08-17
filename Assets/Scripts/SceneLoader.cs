using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text puzzleName;
    private GameObject player;
    private float sceneRenderDist = 75f;
    private float textFade = 1f;
    private float fadeDelay = 10f;
    private float update;
    // Start is called before the first frame update

    public void SetScene(string sceneName)
    {
        UpdateLoadedScenes();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        puzzleName.text = sceneName;
        ShowSceneName();
    }

    public void ShowSceneName()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTextToFullAlpha(textFade, puzzleName));
        update = fadeDelay;
    }

    private void UpdateLoadedScenes()
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
        player = GameObject.FindGameObjectWithTag("Player");
        update = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (update > 0)
            update -= Time.deltaTime;
        else if (puzzleName.color.a >= 1)
            StartCoroutine(FadeTextToZeroAlpha(textFade, puzzleName));
    }

    public IEnumerator FadeTextToFullAlpha(float t, TMP_Text name)
    {
        name.color = new Color(name.color.r, name.color.g, name.color.b, 0);
        while (name.color.a < 1.0f)
        {
            name.color = new Color(name.color.r, name.color.g, name.color.b, name.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TMP_Text name)
    {
        name.color = new Color(name.color.r, name.color.g, name.color.b, 1);
        while (name.color.a > 0.0f)
        {
            name.color = new Color(name.color.r, name.color.g, name.color.b, name.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
