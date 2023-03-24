using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGizmo : MonoBehaviour
{
    [Header("Forward Indicator")]
    public float forwardGizmoLength = 1f;
    public Color forwardGizmoColor = Color.white;

    [Header("Player Box")]
    public Vector3 playerSize = new Vector3(1f, 2f, 1f);
    public Color playerBoxGizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = forwardGizmoColor;
        Gizmos.DrawRay(transform.position, transform.forward * forwardGizmoLength);

        Gizmos.color = playerBoxGizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, playerSize);
    }
}
