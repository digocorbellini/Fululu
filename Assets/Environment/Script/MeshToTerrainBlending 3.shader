Shader "Custom/BendIntoTerrain" {
    Properties {
        _MainTex ("Texture", 2D) = "" {}
        _BlendTex ("Blend Texture", 2D) = "" {}
        _NormalTex ("Normal Texture", 2D) = "Normal" {}
        _BlendTarget ("Blend Target", Range(0, 1)) = 0.5
        _BlendUpdateTimer ("Blend Update Timer", Float) = 1.0
        _Radius ("Radius", Float) = 5.0
        _Strength ("Strength", Float) = 1.0
        _TextureBlend ("Texture Blend", Float) = 1.0
        _NormalBlend ("Normal Blend", Float) = 1.0
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBinormal : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _BlendTex;
            sampler2D _NormalTex;
            float _BlendTarget;
            float _BlendUpdateTimer;
            float _Radius;
            float _Strength;
            float _TextureBlend;
            float _NormalBlend;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Pass world space position and normal to the fragment shader
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = mul(unity_ObjectToWorld, float4(UNITY_MATRIX_IT_MV[2].xyz, 0.0)).xyz;
                o.worldTangent = mul(unity_ObjectToWorld, float4(UNITY_MATRIX_IT_MV[0].xyz, 0.0)).xyz;
                o.worldBinormal = mul(unity_ObjectToWorld, float4(UNITY_MATRIX_IT_MV[1].xyz, 0.0)).xyz;

                return o;
            }

    fixed4 frag (v2f i) : SV_Target {
            {
            // Calculate blend weight based on distance to terrain
            float4 blend = tex2D(_BlendTex, i.uv);
            float blendWeight = smoothstep(_BlendTarget - _Radius, _BlendTarget + _Radius, blend.r);
            blendWeight *= _Strength;
        
            // Calculate world space position and normal from heightmap
            float4 heightmap = tex2D(_MainTex, i.uv);
            float height = heightmap.r * _Strength;
            float3 terrainPos = i.worldPos + i.worldNormal * height;
            float3 worldNormal = UnpackNormal(tex2D(_NormalTex, i.uv)) * 2 - 1;
            worldNormal = normalize(i.worldNormal + worldNormal * blendWeight);
            worldNormal = lerp(i.worldNormal, worldNormal, _NormalBlend);
        
            // Blend object texture with terrain texture
            float4 objectColor = tex2D(_MainTex, i.uv);
            float4 terrainColor = tex2D(_BlendTex, i.uv);
            float4 finalColor = lerp(objectColor, terrainColor, _TextureBlend);
        
            // Calculate final lighting using world space normal
            fixed4 c = finalColor * UNITY_LIGHTMODEL_AMBIENT;
            c.rgb += UnityLightingGI(i.worldPos, worldNormal);
            return c;

            } 
            ENDCG
            }
        FallBack "Diffuse"
        }