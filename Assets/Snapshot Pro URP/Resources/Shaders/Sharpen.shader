Shader "SnapshotProURP/Sharpen"
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

			CBUFFER_START(UnityPerMaterial)
				float2 _MainTex_TexelSize;
				float _Intensity;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).rgb;
				col += 4.0f * col * _Intensity;

				float2 s = _MainTex_TexelSize;
				col -= tex2D(_MainTex, i.uv + float2(0,	   -s.y)).rgb * _Intensity;
				col -= tex2D(_MainTex, i.uv + float2(-s.x,    0)).rgb * _Intensity;
				col -= tex2D(_MainTex, i.uv + float2(s.x,     0)).rgb * _Intensity;
				col -= tex2D(_MainTex, i.uv + float2(0,     s.y)).rgb * _Intensity;

				return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
