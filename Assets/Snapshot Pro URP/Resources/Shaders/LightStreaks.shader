Shader "SnapshotProURP/LightStreaks"
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
			sampler2D _BlurTex;

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_TexelSize;
				uint _KernelSize;
				float _Spread;
				float _LuminanceThreshold;
			CBUFFER_END
		ENDHLSL

        Pass
        {
			Name "HorizontalBlur"

			HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_horizontal

			float gaussian(int x)
			{
				float sigmaSqu = _Spread * _Spread;
				return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
			}

			float4 frag_horizontal(v2f i) : SV_Target
			{
				float3 light = 0.0f;
				float kernelSum = 0.0f;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				float2 uv;

				for (int x = lower; x <= upper; ++x)
				{
					float gauss = gaussian(x);
					kernelSum += gauss;
					uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);

					float3 newLight = tex2D(_MainTex, uv).xyz;
					float lum = dot(newLight, float3(0.3f, 0.59f, 0.11f));
					light += step(_LuminanceThreshold, lum) * newLight * gauss;
				}

				light /= kernelSum;

				return float4(light, 1.0f);
			}
            ENDHLSL
        }

		Pass
		{
			Name "Overlay"

			HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_overlay

			float4 frag_overlay(v2f i) : SV_Target
			{
				float4 mainCol = tex2D(_MainTex, i.uv);
				float4 blurCol = tex2D(_BlurTex, i.uv);

				return mainCol + blurCol;
			}
            ENDHLSL
		}
    }
}
