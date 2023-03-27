using System.Collections;
using UnityEngine;

public class TreeColorChange : MonoBehaviour
{
    public Texture baseColorTexture;
    public Texture normalTexture;
    public Color[] leafColors = new Color[4];

    private Renderer rend;
    private Vector3 treePosition;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        treePosition = transform.position;
    }

    private void Update()
    {
        Vector3 relativePosition = transform.position - treePosition;
        Color newColor = Color.white;

        // Check if tree is on the left or right of the world
        if (relativePosition.x < 0)
        {
            newColor = leafColors[0];
        }
        else if (relativePosition.x > 0)
        {
            newColor = leafColors[1];
        }

        // Check if tree is in front or back of the world
        if (relativePosition.z < 0)
        {
            newColor = leafColors[2];
        }
        else if (relativePosition.z > 0)
        {
            newColor = leafColors[3];
        }

        // Apply color and textures to tree leaves
        rend.material.SetColor("_Color", newColor);
        rend.material.SetTexture("_MainTex", baseColorTexture);
        rend.material.SetTexture("_BumpMap", normalTexture);
    }
}