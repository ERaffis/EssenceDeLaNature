Shader "SnapshotProURP/Cutout"
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
			#include "SnapshotUtility.hlsl"

			sampler2D _MainTex;
			sampler2D _CutoutTex;

			CBUFFER_START(UnityPerMaterial)
				float4 _BorderColor;
				int _Stretch;
				float _Zoom;
				float2 _Offset;
				float4x4 _Rotation;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 UVs = (i.uv - 0.5f) / _Zoom;

				float aspect = (_Stretch == 0) ? _ScreenParams.x / _ScreenParams.y : 1.0f;
				UVs = float2(aspect * UVs.x, UVs.y);

				float2x2 rotationMatrix = float2x2(_Rotation._m00, _Rotation._m01, _Rotation._m10, _Rotation._m11);
				UVs = mul(rotationMatrix, UVs);
				UVs += float2(_Offset.x * aspect, _Offset.y) + 0.5f;

				float cutoutAlpha = tex2D(_CutoutTex, UVs).a;
				float4 col = tex2D(_MainTex, i.uv);
				return lerp(col, _BorderColor, cutoutAlpha);
            }
            ENDHLSL
        }
    }
}
