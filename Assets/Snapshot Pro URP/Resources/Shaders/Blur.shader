Shader "SnapshotProURP/Blur"
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

		HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "SnapshotUtility.hlsl"

		#define E 2.71828f

		sampler2D _MainTex;

		CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_TexelSize;
			uint _KernelSize;
			float _Spread;
		CBUFFER_END

		float gaussian(int x)
		{
			float sigmaSqu = _Spread * _Spread;
			return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
		}

		ENDHLSL

        Pass
        {
			Name "Horizontal"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_horizontal

            float4 frag_horizontal (v2f i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				float kernelSum = 0.0f;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				float2 uv;

				for (int x = lower; x <= upper; ++x)
				{
					float gauss = gaussian(x);
					kernelSum += gauss;
					uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
					col += gauss * tex2D(_MainTex, uv).xyz;
				}

				col /= kernelSum;

				return float4(col, 1.0f);
			}
            ENDHLSL
        }

		Pass
        {
			Name "Vertical"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_vertical

            float4 frag_vertical (v2f i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				float kernelSum = 0.0f;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				float2 uv;

				for (int y = lower; y <= upper; ++y)
				{
					float gauss = gaussian(y);
					kernelSum += gauss;
					uv = i.uv + float2(0.0f, _MainTex_TexelSize.y * y);
					col += gauss * tex2D(_MainTex, uv).xyz;
				}

				col /= kernelSum;
				return float4(col, 1.0f);
			}
            ENDHLSL
        }
    }
}
