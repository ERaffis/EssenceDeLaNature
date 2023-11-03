Shader "SnapshotProURP/BasicDither"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
		{ 
			"RenderType" = "Opaque" 
			"RenderPipeline" = "UniversalPipeline"
		}

        Pass
        {
            HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ USE_SCENE_TEXTURE_ON

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "SnapshotUtility.hlsl"

			sampler2D _MainTex;
			sampler2D _NoiseTex;

			CBUFFER_START(UnityPerMaterial)
				float4 _NoiseTex_TexelSize;
				float _NoiseSize;
				float _ThresholdOffset;

				float4 _LightColor;
				float4 _DarkColor;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;
				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));

				float2 noiseUV = i.uv * _NoiseTex_TexelSize.xy * _ScreenParams.xy * 2.0f / _NoiseSize;
				float3 noiseColor = tex2D(_NoiseTex, noiseUV).xyz;
				float threshold = dot(noiseColor, float3(0.3f, 0.59f, 0.11f)) + _ThresholdOffset;

#ifdef USE_SCENE_TEXTURE_ON
				col = lum < threshold ? _DarkColor.xyz : col;
#else
				col = lum < threshold ? _DarkColor.xyz : _LightColor.xyz;
#endif

				return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
