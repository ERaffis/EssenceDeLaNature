Shader "SnapshotProURP/WorldScan"
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
			sampler2D _OverlayRampTex;

			CBUFFER_START(UnityPerMaterial)
				float3 _ScanOrigin;
				float _ScanDist;
				float _ScanWidth;
				float4 _OverlayColor;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
#if UNITY_REVERSED_Z
				float depth = SampleSceneDepth(i.uv);
#else
				float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.uv));
#endif

				float3 worldPos = ComputeWorldSpacePosition(i.uv, depth, UNITY_MATRIX_I_VP);

				float fragDist = distance(worldPos, _ScanOrigin);

				float4 scanColor = 0.0f;

				if (fragDist < _ScanDist && fragDist > _ScanDist - _ScanWidth)
				{
					float scanUV = (fragDist - _ScanDist) / (_ScanWidth * 1.01f);

					scanColor = tex2D(_OverlayRampTex, float2(scanUV, 0.5f));
					scanColor *= _OverlayColor;
				}

				float4 textureSample = tex2D(_MainTex, i.uv);

				return lerp(textureSample, scanColor, scanColor.a);
            }
            ENDHLSL
        }
    }
}
