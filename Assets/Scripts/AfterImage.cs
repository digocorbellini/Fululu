using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Particle = UnityEngine.ParticleSystem.Particle;

public class AfterImage : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_Renderer;
    [SerializeField] public SkinnedMeshRenderer meshToCopy;
    [SerializeField] public Particle particle;
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private AnimationCurve zVelocityOverLifetime;
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private Transform modelTrans;
    private Transform rootBone;
    private Transform _copyRootBoneTran;
    private float currentLifetime = 0.0f;

    private void Start()
    {
        if (m_Renderer)
        {
            rootBone = m_Renderer.rootBone;
        }

        if(meshToCopy)
        {
            _copyRootBoneTran = meshToCopy.rootBone;
        }

        CopyPoseGeneral();

        transform.position = particle.position;

        transform.rotation = Quaternion.Euler(particle.rotation3D);

        m_Renderer.material = new Material(m_Renderer.material);
    }

    private void CopyPoseGeneral()
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
    }

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
