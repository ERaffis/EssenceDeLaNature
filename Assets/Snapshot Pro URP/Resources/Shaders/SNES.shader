Shader "SnapshotProURP/SNES"
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

			#define EPSILON 1e-06

			sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
				int _BandingLevels;
				float _PowerRamp;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).rgb;
				col = pow(col, _PowerRamp);

				int r = (col.r - EPSILON) * _BandingLevels;
				int g = (col.g - EPSILON) * _BandingLevels;
				int b = (col.b - EPSILON) * _BandingLevels;

				float divisor = _BandingLevels - 1.0f;

                return float4(r / divisor, g / divisor, b / divisor, 1.0f);
            }
            ENDHLSL
        }
    }
}
