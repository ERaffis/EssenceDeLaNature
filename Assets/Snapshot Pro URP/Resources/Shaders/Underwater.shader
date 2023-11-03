Shader "SnapshotProURP/Underwater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "SnapshotUtility.hlsl"

			sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
				sampler2D _BumpMap;
				float _Strength;
				float4 _WaterColor;
				float _FogStrength;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 timeUV = (i.uv + _Time.x) % 1.0f;
				float4 bumpTex = tex2D(_BumpMap, timeUV);
				float2 normal = bumpTex.wy * 2.0f - 1.0f;

				float2 normalUV = i.uv + (1.0f / _ScreenParams.xy) * normal.xy * _Strength;
				float3 col = tex2D(_MainTex, normalUV).xyz;

#if UNITY_REVERSED_Z
				float depth = SampleSceneDepth(i.uv);
#else
				float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.uv));
#endif
				depth = Linear01Depth(depth, _ZBufferParams);

				col = lerp(col, _WaterColor.xyz, depth * _FogStrength);
				return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
