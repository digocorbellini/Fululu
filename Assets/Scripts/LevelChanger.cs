using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public LoadingScreen loadingScreen;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator ChangeSceneRoutine(string sceneName)
    {
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
    }

    public void ChangeSceneAsync(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

}
