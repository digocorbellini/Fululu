Shader "Custom/MeshToTerrainBlendingWithLayersAndBending" {
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
        
        // Bend values
        _BlendUpdateTimer("Blend Update Timer", Range(0, 1)) = 0.1
        _Radius("Radius", Range(0, 1)) = 0.1
        _BendAxis ("Bend Axis", Vector) = (1, 0, 0)
        _BendPositive ("Bend Positive", Range(0,1)) = 1
        _BendAmount ("Bend Amount", Range(0,1)) = 0.1

        //Lighting
        _ToonShading ("Toon Shading", Range(0, 1)) = 1
        _ToonColorRamp ("Toon Color Ramp", 2D) = "white" {}
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
        sampler2D _CliffTex;
        sampler2D _NormalMap;
        sampler2D _GrassAO;
        sampler2D _RockAO;
        sampler2D _CliffAO;
        sampler2D _GrassHeightMap;
        sampler2D _RockHeightMap;
        sampler2D _CliffHeightMap;
        sampler2D _GrassRoughness;
        sampler2D _RockRoughness;
        sampler2D _CliffRoughness;
        sampler2D _ToonColorRamp;
        float _Blend;
        float _GrassLayer;
        float _RockLayer;
        float _CliffLayer;
        float _ToonShading;
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 worldPos = IN.worldPos;
            worldPos.y = 0; // Flatten mesh to terrain
          float4 blend = tex2D(_MainTex, IN.uv_MainTex);
          float4 grass = tex2D(_GrassTex, IN.uv_GrassTex);
          float4 rock = tex2D(_RockTex, IN.uv_RockTex);
          float4 cliff = tex2D(_CliffTex, IN.uv_MainTex);
          float4 grassAO = tex2D(_GrassAO, IN.uv_GrassTex);
          float4 rockAO = tex2D(_RockAO, IN.uv_RockTex);
          float4 cliffAO = tex2D(_CliffAO, IN.uv_MainTex);
          float4 grassHeight = tex2D(_GrassHeightMap, IN.uv_GrassTex);
          float4 rockHeight = tex2D(_RockHeightMap, IN.uv_RockTex);
          float4 cliffHeight = tex2D(_CliffHeightMap, IN.uv_MainTex);
          float4 grassRoughness = tex2D(_GrassRoughness, IN.uv_GrassTex);
          float4 rockRoughness = tex2D(_RockRoughness, IN.uv_RockTex);
          float4 cliffRoughness = tex2D(_CliffRoughness, IN.uv_MainTex);
      
          // Calculate weights based on blend value
          float grassWeight = saturate((blend.r - _GrassLayer) / _Blend);
          float rockWeight = saturate((_RockLayer - blend.r) / _Blend);
          float cliffWeight = saturate((blend.r - _CliffLayer) / _Blend);
          float3 weights = float3(grassWeight, rockWeight, cliffWeight);
          // Mix textures based on weights
          float4 baseColor = lerp(lerp(lerp(grass, rock, weights.y), cliff, weights.z), blend, weights.x);
          o.Albedo = baseColor.rgb;
          o.Metallic = 0;
          o.Smoothness = baseColor.a;
      
          // Apply normal map
          float4 normal = tex2D(_NormalMap, IN.uv_MainTex);
          float3 worldNormal = UnpackNormal(normal).xyz;
          float3 normalBlend = lerp(normalize(IN.worldNormal), normalize(worldNormal), _NormalMapIntensity);
          o.Normal = PackNormal(normalBlend);
      
          // Apply height map
          float heightBlend = 1.0f;
          if (_UseHeightMap > 0.0f) {
              float height = worldPos.y / _MaxTerrainHeight;
              heightBlend = smoothstep(_MinHeight, _MaxHeight, height);
           }
          float4 heightMap = lerp(lerp(grassHeight, rockHeight, weights.y), cliffHeight, weights.z);
          heightMap = tex2D(_UseHeightMap > 0.0f ? _GrassHeightMap : _MainTex, IN.uv_GrassTex);
          o.Height = lerp(worldPos.y, _MaxTerrainHeight * heightMap.r, heightBlend);
      
          // Apply roughness map
          float4 roughnessMap = lerp(lerp(grassRoughness, rockRoughness, weights.y), cliffRoughness, weights.z);
          float roughness = _UseRoughnessMap > 0.0f ? roughnessMap.r : baseColor.a;
          float2 tiling = float2(1, 1);
          float2 offset = float2(0, 0);
          // Apply tiling and offset to texture coordinates
          uv *= tiling;
          uv += offset;
          // Sample roughness map at modified texture coordinates
          float roughness = SampleTexture(roughnessMap, uv).r;
         
          // Apply roughness to material
          o.Roughness = roughness;
                  
          // Set metalness to 0, since this material is not metallic
          o.Metalness = 0;
                  
          // Set emissive color to black, since this material does not emit light
          o.Emission = float3(0, 0, 0);
           
          // Set alpha to 1, since this material is not transparent
          o.Alpha = 1;
          
          // Set the ambient occlusion factor based on the weights and the ambient occlusion maps
          o.AmbientOcclusion = lerp(lerp(grassAO, rockAO, weights.y), cliffAO, weights.z);
           
          // Set the normal map based on the weights and the normal maps
          o.Normal = perturbedNormal;
          o.Normal = lerp(o.Normal, rockNormal, weights.y);
          o.Normal = lerp(o.Normal, cliffNormal, weights.z);
          
          return o;
          }
          fixed4 frag (v2f i) : SV_Target {
          // Calculate blend weight based on distance to terrain
          float4 blend = tex2D(_BlendTex, i.uv);
          float blendWeight = smoothstep(_BlendTarget - _Radius, _BlendTarget + _Radius, blend.r);
          blendWeight *= _Strength;
       
         // Calculate world space position and normal from heightmap
         float4 heightmap = tex2D(_MainTex, i.uv);
         float height = heightmap.r * _Strength;
         float3 terrainPos = i.worldPos + i.worldNormal * height;
         float3 worldNormal = UnpackNormal(tex2D(_NormalTex, i.uv)) * 2 - 1;
       
         // Calculate weights for blending textures
         float4 weights = tex2D(_BlendMap, i.uv);
       
         // Calculate the perturbed normal using the heightmap
         float3 worldPos = i.worldPos + worldNormal * height * _HeightScale;
         float3 perturbedNormal = UnpackNormal(tex2D(_NormalTex, i.uv + worldNormal * height * _NormalScale)) * 2 - 1;
       
         // Set the normal map based on the weights and the normal maps
         float3 rockNormal = UnpackNormal(tex2D(_RockNormalTex, i.uv)) * 2 - 1;
         float3 cliffNormal = UnpackNormal(tex2D(_CliffNormalTex, i.uv)) * 2 - 1;
         perturbedNormal = lerp(perturbedNormal, rockNormal, weights.y);
         perturbedNormal = lerp(perturbedNormal, cliffNormal, weights.z);
       
         // Calculate the final world-space normal
         worldNormal = normalize(mul(unity_ObjectToWorld, float4(perturbedNormal, 0)).xyz);

         // Calculate lighting
         float3 worldPosView = UnityWorldSpaceViewDir(worldPos);
         float3 diffuseLight = ComputeDiffuseLight(worldNormal, worldPosView, _LightColor0.rgb, _IndirectDiffuse.rgb, _LightAtten0);
         float3 specularLight = ComputeSpecularLight(_WorldSpaceLightPos0, worldNormal, worldPosView, _LightColor0.rgb, _SpecularColor.rgb, _Gloss, _IndirectSpecular.rgb, _LightAtten0);
       
         // Calculate toon shading
         float toonShading = smoothstep(0, 1, dot(worldNormal, _WorldSpaceLightPos0.xyz));
         toonShading = _ToonShading * ceil(toonShading / _ToonShading);
       
         // Apply toon shading to color
         float4 color = tex2D(_MainTex, i.uv) * blendWeight;
         float4 toonColor = tex2D(_ToonColorRamp, float2(toonShading, 0.5));
         color.rgb = lerp(color.rgb, toonColor.rgb, _ToonShading);
         color.rgb = saturate(color.rgb * (diffuseLight + _Ambient.rgb) + specularLight);
         return color;
         }
         ENDCG
         }
         FallBack "Diffuse"
         }