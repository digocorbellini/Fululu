using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }
        DontDestroyOnLoad(this);
    }

    public IEnumerator Appear()
    {
        gameObject.SetActive(true);
        anim.Play("Appear");
        yield return new WaitUntil(() => IsAnimationDone("Appear"));
    }

    public bool IsAnimationDone(string animationName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public IEnumerator Disappear()
    {
        anim.CrossFade("Disappear", 0.1f);
        yield return new WaitUntil(() => IsAnimationDone("Disappear"));
        gameObject.SetActive(false);
    }
}
