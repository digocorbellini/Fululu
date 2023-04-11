using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle = UnityEngine.ParticleSystem.Particle;

public class AfterImage : MonoBehaviour
{
    [SerializeField] private Material afterImageMaterial;
    [SerializeField] public Transform modelToCopy;
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private AnimationCurve zVelocityOverLifetime;
    [SerializeField] private Gradient colorOverLifetime;
    private Transform modelTrans;
    private SkinnedMeshRenderer m_Renderer;
    private float currentLifetime = 0.0f;

    private void Start()
    {
        if (!modelToCopy)
        {
            Destroy(gameObject);
            return;
        }

        modelTrans = Instantiate(modelToCopy, transform, true);

        m_Renderer = modelTrans.GetComponentInChildren<SkinnedMeshRenderer>();

        AfterImageSpawner spawner = modelTrans.GetComponentInChildren<AfterImageSpawner>();
        if (spawner)
        {
            Debug.Log("Recursive spawner disabled");
            spawner.enabled = false;
            spawner.gameObject.SetActive(false);
        }

        Animator anim = modelTrans.GetComponent<Animator>();
        if (anim)
        {
            anim.enabled = false;
        }

        AudioSource[] sources = modelTrans.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in sources)
        {
            source.enabled = false;
        }

        m_Renderer.material = new Material(afterImageMaterial);
    }

    /*private void CopyPoseGeneral()
    {
        foreach (Transform boneTran in _copyRootBoneTran)
        {
            CopyTransform("", boneTran);
        }
    }

    private void CopyTransform(string parentPath, Transform curTran)
    {
        var targetTran = rootBone.Find($"{parentPath}{curTran.name}");
        if (targetTran != null)
        {
            targetTran.localRotation = curTran.localRotation;
            targetTran.localPosition = curTran.localPosition;
        }

        foreach (Transform childBoneTran in curTran)
        {
            CopyTransform($"{parentPath}{curTran.name}/", childBoneTran);
        }
    }*/

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime <= lifetime)
        {
            if (Time.timeScale > 0)
            {
                float time = currentLifetime / lifetime;
                float zChange = zVelocityOverLifetime.Evaluate(time) * Time.deltaTime;
                Vector3 position = modelTrans.localPosition;
                position.z += zChange;
                modelTrans.localPosition = position;

                m_Renderer.material.color = colorOverLifetime.Evaluate(time);
            }
        } else
        {
            Destroy(gameObject);
        }
        
    }

    private void Reset()
    {
        m_Renderer = GetComponent<SkinnedMeshRenderer>();
    }
}
