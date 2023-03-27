using System.Collections;
using UnityEngine;

public class ObjectToTerrainBend : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private float blendTarget = 0.5f;
    [SerializeField] private float blendUpdateTimer = 0.5f;
    [SerializeField] private bool runtimeNormalUpdate = true;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float strength = 0.5f;
    [SerializeField] private float textureBlend = 0.5f;
    [SerializeField] private float normalBlend = 0.5f;

    private float timer = 0f;
    private Vector3 terrainPos;
    private Vector3 terrainSize;
    private TerrainData terrainData;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private Vector3[] normals;

    private void Start()
    {
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;
        terrainSize = terrainData.size;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        displacedVertices = mesh.vertices;

        if (runtimeNormalUpdate)
        {
            normals = mesh.normals;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= blendUpdateTimer)
        {
            timer = 0f;

            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 vertex = originalVertices[i];
                Vector3 worldVertex = transform.TransformPoint(vertex);

                // Calculate the corresponding UV coordinate on the terrain
                Vector3 normalizedPos = (worldVertex - terrainPos) / terrainSize;
                int x = Mathf.RoundToInt(normalizedPos.x * (terrainData.heightmapResolution - 1));
                int y = Mathf.RoundToInt(normalizedPos.z * (terrainData.heightmapResolution - 1));

                // Get the height at the UV coordinate
                float height = terrainData.GetHeight(x, y) / terrainSize.y;

                // Calculate the blend value based on the height difference between the vertex and the terrain
                float blendValue = Mathf.InverseLerp(blendTarget - radius, blendTarget + radius, height);

                // Apply the blend value to the vertex
                Vector3 displacedVertex = Vector3.Lerp(vertex, worldVertex - Vector3.up * height * terrainSize.y, blendValue * strength);
                displacedVertices[i] = transform.InverseTransformPoint(displacedVertex);

                // Update the normals in real-time
                if (runtimeNormalUpdate)
                {
                    Vector3 normal = normals[i];
                    Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.z);
                    normal = Vector3.Slerp(normal, terrainNormal, normalBlend);
                    normals[i] = normal;
                }
            }

            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.vertices = displacedVertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (textureBlend > 0f)
            {
                GetComponent<Renderer>().material.SetFloat("_TextureBlend", textureBlend);
            }
        }
    }
}
