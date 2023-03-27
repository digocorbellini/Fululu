Shader "Custom/MeshToTerrainWithLayers" {
    Properties {
        _MainTex ("Base Texture", 2D) = "white" {}
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _RockTex ("Rock Texture", 2D) = "white" {}
        _CliffTex ("Cliff Texture", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 0.5
        _GrassLayer ("Grass Layer", Range(0,1)) = 0.2
        _RockLayer ("Rock Layer", Range(0,1)) = 0.5
        _CliffLayer ("Cliff Layer", Range(0,1)) = 0.8
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _RoughnessTex ("Roughness Texture", 2D) = "white" {}
        _Roughness ("Roughness", Range(0,1)) = 0.5
        _AOTex ("Ambient Occlusion Texture", 2D) = "white" {}
        _AO ("Ambient Occlusion", Range(0,1)) = 1
        _HeightTex ("Height Texture", 2D) = "white" {}
        _Height ("Height", Range(0,1)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input {
            float2 uv_MainTex;
            float2 uv_GrassTex;
            float2 uv_RockTex;
            float2 uv_CliffTex;
            float3 worldPos;
            float3 worldNormal;
        };
        
        sampler2D _MainTex;
        sampler2D _GrassTex;
        sampler2D _RockTex;
        sampler2D _CliffTex;
        sampler2D _NormalMap;
        sampler2D _RoughnessTex;
        sampler2D _AOTex;
        sampler2D _HeightTex;
        float _Blend;
        float _GrassLayer;
        float _RockLayer;
        float _CliffLayer;
        float _Roughness;
        float _AO;
        float _Height;
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 worldPos = IN.worldPos;
            worldPos.y = 0; // Flatten mesh to terrain
            
            float4 blend = tex2D(_MainTex, IN.uv_MainTex);
            float4 grass = tex2D(_GrassTex, IN.uv_GrassTex);
            float4 rock = tex2D(_RockTex, IN.uv_RockTex);
            float4 cliff = tex2D(_CliffTex, IN.uv_CliffTex);
            
            // Calculate weights based on blend value
            float grassWeight = saturate((blend.r - _GrassLayer) / _Blend);
            float rockWeight = saturate((_RockLayer - blend.r) / _Blend);
            float cliffWeight = saturate((blend.r - _CliffLayer) / _Blend);
            float3 weights = float3(grassWeight, rockWeight, cliffWeight);
            weights /= dot(weights, float3(1,1,1));
  
            // Mix textures based on weights
            float4 baseColor = lerp(lerp(grass, rock, weights.y), lerp(cliff, blend, weights.z), weights.x);
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
