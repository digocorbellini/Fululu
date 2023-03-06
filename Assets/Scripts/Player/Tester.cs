using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public DialogueController controller;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            if (!controller.SetCurrentDialogue("ExampleDialogue", true))
                Debug.LogError("AAAAH WRONG DIALOGUE NAME");
    }
}
