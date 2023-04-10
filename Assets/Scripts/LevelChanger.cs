using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public LoadingScreen loadingScreenPrefab;
    public static LevelChanger instance;
    public Camera loadingScreenCamera;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
            loadingScreenCamera.gameObject.SetActive(false);
        } else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator ChangeSceneRoutine(string sceneName)
    {
        LoadingScreen loadingScreen = Instantiate(loadingScreenPrefab);
        loadingScreenCamera.gameObject.SetActive(true);
        if (loadingScreen)
        {
            yield return StartCoroutine(loadingScreen.Appear());
        }

        var levelChange = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => levelChange.isDone);
        
        if (loadingScreen)
        {
            yield return StartCoroutine(loadingScreen.Disappear());
        }

        loadingScreenCamera.gameObject.SetActive(false);

        Destroy(loadingScreen.gameObject);
    }

    public void ChangeSceneAsync(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

}
