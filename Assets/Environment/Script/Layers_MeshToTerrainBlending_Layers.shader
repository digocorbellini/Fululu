Shader "Custom/MeshToTerrainBlendingWithLayers" {
    Properties {
        // Main texture for mesh-to-terrain blending
        _MainTex ("Base Texture", 2D) = "white" {}
        
        // Grass layer
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _GrassColor ("Grass Color", Color) = (1, 1, 1, 1)
        _GrassTint ("Grass Tint", Color) = (1, 1, 1, 1)
        _GrassNormalMap ("Grass Normal Map", 2D) = "bump" {}
        _GrassHeightMap ("Grass Height Map", 2D) = "white" {}
        _GrassRoughness ("Grass Roughness", 2D) = "white" {}
        _GrassAO ("Grass Ambient Occlusion", 2D) = "white" {}
        
        // Rock layer
        _RockTex ("Rock Texture", 2D) = "white" {}
        _RockColor ("Rock Color", Color) = (1, 1, 1, 1)
        _RockTint ("Rock Tint", Color) = (1, 1, 1, 1)
        _RockNormalMap ("Rock Normal Map", 2D) = "bump" {}
        _RockHeightMap ("Rock Height Map", 2D) = "white" {}
        _RockRoughness ("Rock Roughness", 2D) = "white" {}
        _RockAO ("Rock Ambient Occlusion", 2D) = "white" {}
        
        // Cliff layer
        _CliffTex ("Cliff Texture", 2D) = "white" {}
        _CliffColor ("Cliff Color", Color) = (1, 1, 1, 1)
        _CliffTint ("Cliff Tint", Color) = (1, 1, 1, 1)
        _CliffNormalMap ("Cliff Normal Map", 2D) = "bump" {}
        _CliffHeightMap ("Cliff Height Map", 2D) = "white" {}
        _CliffRoughness ("Cliff Roughness", 2D) = "white" {}
        _CliffAO ("Cliff Ambient Occlusion", 2D) = "white" {}
        
        // Layer blend values
        _GrassLayer ("Grass Layer", Range(0,1)) = 0.2
        _RockLayer ("Rock Layer", Range(0,1)) = 0.5
        _CliffLayer ("Cliff Layer", Range(0,1)) = 0.8
        
        // Blend range
        _BlendRange ("Blend Range", Range(0,1)) = 0.1
        
        // Heightmap values
        _UseHeightMap ("Use Height Map", Range(0,1)) = 0
        _MinHeight ("Minimum Height", Range(0,1)) = 0
        _MaxHeight ("Maximum Height", Range(0,1)) = 1
        
        // Roughness values
        _UseRoughnessMap ("Use Roughness Map", Range(0,1)) = 0
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100
        
        CGPROGRAM
        #pragma surface surf Standard

        struct Input {
            float2 uv_MainTex;
            float2 uv_GrassTex;
            float2 uv_RockTex;
            float3 worldPos;
            float3 worldNormal;
        };
        
        sampler2D _MainTex;
        sampler2D _GrassTex;
        sampler2D _RockTex;
        sampler2D _NormalMap;
        float _Blend;
        float _GrassLayer;
        float _RockLayer;
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 worldPos = IN.worldPos;
            worldPos.y = 0; // Flatten mesh to terrain
            
            float4 blend = tex2D(_MainTex, IN.uv_MainTex);
            float4 grass = tex2D(_GrassTex, IN.uv_GrassTex);
            float4 rock = tex2D(_RockTex, IN.uv_RockTex);
            
            // Calculate weights based on blend value
            float grassWeight = saturate((blend.r - _GrassLayer) / _Blend);
            float rockWeight = saturate((_RockLayer - blend.r) / _Blend);
            float3 weights = float3(grassWeight, rockWeight, 1 - (grassWeight + rockWeight));
            
            // Mix textures based on weights
            float4 baseColor = lerp(lerp(grass, rock, weights.y), blend, weights.z);
            o.Albedo = baseColor.rgb;
            o.Metallic = 0;
            o.Smoothness = baseColor.a;

            // Apply normal map
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
