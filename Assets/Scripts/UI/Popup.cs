using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Popup : MonoBehaviour
{
    private PlayerInput input;
    private Animator animator;
    private float timer = 0;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Debug.LogWarning("Enabled");
        GameManager.instance.ToggleInput(false);
        animator.Play("Appear");
        input.enabled = true;
        timer = .66f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public void OnNext()
    {
        if(timer < 0)
        {
            GameManager.instance.ToggleInput(true);
            animator.Play("Disappear");
            input.enabled = false;
        }
    }


}
