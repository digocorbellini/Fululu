Shader "Custom/MeshToTerrainBlendingWithLayersAndBending" {
Properties {
 _MainTex ("Base Texture", 2D) = "white" { }
 _GrassTex ("Grass Texture", 2D) = "white" { }
 _GrassColor ("Grass Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _GrassTint ("Grass Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
 _GrassNormalMap ("Grass Normal Map", 2D) = "bump" { }
 _GrassHeightMap ("Grass Height Map", 2D) = "white" { }
 _GrassRoughness ("Grass Roughness", 2D) = "white" { }
 _GrassAO ("Grass Ambient Occlusion", 2D) = "white" { }
 _RockTex ("Rock Texture", 2D) = "white" { }
 _RockColor ("Rock Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _RockTint ("Rock Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
 _RockNormalMap ("Rock Normal Map", 2D) = "bump" { }
 _RockHeightMap ("Rock Height Map", 2D) = "white" { }
 _RockRoughness ("Rock Roughness", 2D) = "white" { }
 _RockAO ("Rock Ambient Occlusion", 2D) = "white" { }
 _CliffTex ("Cliff Texture", 2D) = "white" { }
 _CliffColor ("Cliff Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _CliffTint ("Cliff Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
 _CliffNormalMap ("Cliff Normal Map", 2D) = "bump" { }
 _CliffHeightMap ("Cliff Height Map", 2D) = "white" { }
 _CliffRoughness ("Cliff Roughness", 2D) = "white" { }
 _CliffAO ("Cliff Ambient Occlusion", 2D) = "white" { }
 _GrassLayer ("Grass Layer", Range(0.000000,1.000000)) = 0.200000
 _RockLayer ("Rock Layer", Range(0.000000,1.000000)) = 0.500000
 _CliffLayer ("Cliff Layer", Range(0.000000,1.000000)) = 0.800000
 _BlendRange ("Blend Range", Range(0.000000,1.000000)) = 0.100000
 _UseHeightMap ("Use Height Map", Range(0.000000,1.000000)) = 0.000000
 _MinHeight ("Minimum Height", Range(0.000000,1.000000)) = 0.000000
 _MaxHeight ("Maximum Height", Range(0.000000,1.000000)) = 1.000000
 _UseRoughnessMap ("Use Roughness Map", Range(0.000000,1.000000)) = 0.000000
 _BlendUpdateTimer ("Blend Update Timer", Range(0.000000,1.000000)) = 0.100000
 _Radius ("Radius", Range(0.000000,1.000000)) = 0.100000
 _BendAxis ("Bend Axis", Vector) = (1.000000,0.000000,0.000000,1.000000)
 _BendPositive ("Bend Positive", Range(0.000000,1.000000)) = 1.000000
 _BendAmount ("Bend Amount", Range(0.000000,1.000000)) = 0.100000
 _ToonShading ("Toon Shading", Range(0.000000,1.000000)) = 1.000000
 _ToonColorRamp ("Toon Color Ramp", 2D) = "white" { }
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Opaque" }
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#include "UnityCG.cginc"
#pragma multi_compile_fog
#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

// uniforms

// vertex shader input data
struct appdata {
  float3 pos : POSITION;
  half4 color : COLOR;
  UNITY_VERTEX_INPUT_INSTANCE_ID
};

// vertex-to-fragment interpolators
struct v2f {
  fixed4 color : COLOR0;
  #if USING_FOG
    fixed fog : TEXCOORD0;
  #endif
  float4 pos : SV_POSITION;
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f vert (appdata IN) {
  v2f o;
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  half4 color = IN.color;
  float3 eyePos = mul (UNITY_MATRIX_MV, float4(IN.pos,1)).xyz;
  half3 viewDir = 0.0;
  o.color = saturate(color);
  // compute texture coordinates
  // fog
  #if USING_FOG
    float fogCoord = length(eyePos.xyz); // radial fog distance
    UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
    o.fog = saturate(unityFogFactor);
  #endif
  // transform position
  o.pos = UnityObjectToClipPos(IN.pos);
  return o;
}

// fragment shader
fixed4 frag (v2f IN) : SV_Target {
  fixed4 col;
  col = IN.color;
  // fog
  #if USING_FOG
    col.rgb = lerp (unity_FogColor.rgb, col.rgb, IN.fog);
  #endif
  return col;
}
ENDCG
 }
}
Fallback "Diffuse"
}