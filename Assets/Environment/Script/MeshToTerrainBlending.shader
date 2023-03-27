// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MeshToTerrainBlending" {
    Properties {
        _MainTex ("Base Texture", 2D) = "white" {}
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _RockTex ("Rock Texture", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 0.5
        _GrassLayer ("Grass Layer", Range(0,1)) = 0.2
        _RockLayer ("Rock Layer", Range(0,1)) = 0.8
        _NormalMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
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
