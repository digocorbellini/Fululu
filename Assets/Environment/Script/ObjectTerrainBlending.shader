using System.Collections;
using UnityEngine;

public class ObjectTerrainBlending : MonoBehaviour
{
    public GameObject[] objects;
    public Terrain terrain;
    public LayerMask terrainLayer;
    public float objectSpacing = 1.0f;
    public float slopeThreshold = 30.0f;
    public float heightThreshold = 1.0f;
    public float maskBlurSize = 2.0f;
    public Shader blendShader;
    public Texture2D terrainTexture;
    public float blendRange = 0.1f;
    public float blendStrength = 1.0f;
    public AnimationCurve maskValueCurve;

    private Material blendMaterial;
    private Texture2D maskTexture;

    private void Start()
    {
        blendMaterial = new Material(blendShader);
        maskTexture = new Texture2D(terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);

        foreach (GameObject obj in objects)
        {
            PlaceObjectOnTerrain(obj);
        }
    }

    private void PlaceObjectOnTerrain(GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, 100, terrainLayer))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= slopeThreshold)
            {
                float distance = Random.Range(objectSpacing * 0.8f, objectSpacing * 1.2f);
                Vector3 randomDirection = Random.insideUnitSphere * distance;
                randomDirection.y = 0;
                Vector3 position = hit.point + randomDirection;
                position.y = terrain.SampleHeight(position);

                if (Mathf.Abs(position.y - hit.point.y) <= heightThreshold)
                {
                    obj.transform.position = position;
                    obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    // Generate mask texture for object placement
                    GenerateMaskTexture(obj.transform.position);

                    // Apply custom shader to blend object texture with terrain texture
                    Renderer objRenderer = obj.GetComponent<Renderer>();
                    objRenderer.material = blendMaterial;
                    objRenderer.material.SetTexture("_MainTex", objRenderer.material.mainTexture);
                    objRenderer.material.SetTexture("_MaskTex", maskTexture);
                    objRenderer.material.SetTexture("_TerrainTex", terrainTexture);
                    objRenderer.material.SetFloat("_BlendRange", blendRange);
                    objRenderer.material.SetFloat("_BlendStrength", blendStrength);
                }
            }
        }
    }

  private void GenerateMaskTexture(Vector3 position)
{
    int maskSize = Mathf.RoundToInt(maskBlurSize * 2 * terrain.terrainData.heightmapResolution / terrain.terrainData.size.z);
    maskTexture = new Texture2D(maskSize, maskSize, TextureFormat.RGBA32, false);
    maskTexture.wrapMode = TextureWrapMode.Clamp;
    maskTexture.filterMode = FilterMode.Bilinear;

    // Generate mask texture based on object position and terrain heightmap
    Vector3 terrainPos = terrain.transform.position;
    Vector3 terrainSize = terrain.terrainData.size;
    Vector2 terrainUV = new Vector2((position.x - terrainPos.x) / terrainSize.x, (position.z - terrainPos.z) / terrainSize.z);
    int maskOffset = maskSize / 2;
    float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
    for (int x = 0; x < maskSize; x++)
    {
        for (int y = 0; y < maskSize; y++)
        {
            float distance = Vector2.Distance(new Vector2(x - maskOffset, y - maskOffset), Vector2.zero);
            float maskValue = Mathf.Clamp01(maskValueCurve.Evaluate(distance / maskBlurSize) * heightMap[Mathf.RoundToInt(terrainUV.y *
            (terrain.terrainData.heightmapResolution-1)), Mathf.RoundToInt(terrainUV.x *
            (terrain.terrainData.heightmapResolution-1))]);
            maskTexture.SetPixel(x, y, new Color(maskValue, maskValue, maskValue, maskValue));
        }
    }
    maskTexture.Apply();
}
}