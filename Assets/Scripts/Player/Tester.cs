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
        {
            Debug.Log("T pressed");
            if (!controller.SetCurrentDialogue("ExampleDialogue"))
                Debug.LogError("AAAAH WRONG DIALOGUE NAME");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            controller.PlayNextDialogue();
        }
            
    }
}
