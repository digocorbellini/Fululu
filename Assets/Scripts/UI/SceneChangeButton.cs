using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    public void ChangeScene(string name)
    {
        LevelChanger.instance.ChangeSceneAsync(name);
    }
}
