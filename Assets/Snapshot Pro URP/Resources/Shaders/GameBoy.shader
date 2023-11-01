Shader "SnapshotProURP/GameBoy"
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
				float4 _GBDarkest;
				float4 _GBDark;
				float4 _GBLight;
				float4 _GBLightest;
				float _PowerRamp;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;
				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));
				lum = pow(lum, _PowerRamp);
				int gb = lum * 4;

				col = lerp(_GBDarkest, _GBDark, saturate(gb));
				col = lerp(col, _GBLight, saturate(gb - 1.0f));
				col = lerp(col, _GBLightest, saturate(gb - 2.0f));

                return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
