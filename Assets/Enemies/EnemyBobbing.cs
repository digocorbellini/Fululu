using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes the mesh of the ghost bob in the air 
public class EnemyBobbing : MonoBehaviour
{
    [SerializeField] private AnimationCurve bobbingCurve;
    [SerializeField] private AnimationCurve straffingCurve;
    [SerializeField] private float movementFactor = 1;

    private float time;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        float xDelta = Mathf.Clamp(straffingCurve.Evaluate(time), -1, 1) * movementFactor;
        float yDelta = Mathf.Clamp(bobbingCurve.Evaluate(time), -1, 1) * movementFactor;
        float zDelta = Mathf.Clamp(straffingCurve.Evaluate(time), -1, 1) * movementFactor;

        Vector3 positionDelta = new Vector3(xDelta, yDelta, zDelta);

        transform.localPosition = originalPos + positionDelta;
    }

}
