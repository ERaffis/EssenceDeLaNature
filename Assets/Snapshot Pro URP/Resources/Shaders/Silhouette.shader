Shader "SnapshotProURP/Silhouette"
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
				float4 _NearColor;
				float4 _FarColor;
				float _PowerRamp;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
#if UNITY_REVERSED_Z
				float depth = SampleSceneDepth(i.uv);
#else
				float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.uv));
#endif
				depth = pow(Linear01Depth(depth, _ZBufferParams), _PowerRamp);

				return lerp(_NearColor, _FarColor, depth);
            }
            ENDHLSL
        }
    }
}
