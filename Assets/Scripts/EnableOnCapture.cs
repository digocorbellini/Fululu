using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnableOnCapture : MonoBehaviour
{

    public GameObject[] enableAfter;
    private void OnDestroy()
    {

        if (!gameObject.scene.isLoaded)
        {
            // Do nothing if being destroyed on scene closing clean up
            return;
        }

        enableAfter.ToList().ForEach(obj => obj.SetActive(true));
    }
}
