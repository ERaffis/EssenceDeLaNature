Shader "SnapshotProURP/Drawing"
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
			sampler2D _DrawingTex;

			CBUFFER_START(UnityPerMaterial)
				float _OverlayOffset;
				float _Strength;
				float _Tiling;
				float _Smudge;
				float _DepthThreshold;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 drawingUV = (i.uv + _OverlayOffset) * _Tiling;
				drawingUV.y *= _ScreenParams.y / _ScreenParams.x;

				float3 drawingCol = (tex2D(_DrawingTex, drawingUV).xyz + 
					tex2D(_DrawingTex, drawingUV / 3.0f).xyz) / 2.0f;

				float2 texUV = i.uv + drawingCol * _Smudge;
				float3 col = tex2D(_MainTex, texUV).xyz;

				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));
				float3 drawing = lerp(col, drawingCol * col, (1.0f - lum) * _Strength);

#if UNITY_REVERSED_Z
				float depth = SampleSceneDepth(i.uv);
#else
				float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.uv));
#endif
				depth = Linear01Depth(depth, _ZBufferParams);

				return float4(depth < _DepthThreshold ? drawing : col, 1.0f);
            }
            ENDHLSL
        }
    }
}
