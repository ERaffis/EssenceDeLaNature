Shader "SnapshotProURP/Scanlines"
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
			sampler2D _ScanlineTex;

			CBUFFER_START(UnityPerMaterial)
				int _Size;
				float _Strength;
				float _ScrollSpeed;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;

				float2 scanlineUV = i.uv * _ScreenParams.xy / _Size;
				scanlineUV.y += _Time.y * _ScrollSpeed;
				float3 scanlines = tex2D(_ScanlineTex, scanlineUV).xyz;

				col = lerp(col, col * scanlines, _Strength);

				return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
