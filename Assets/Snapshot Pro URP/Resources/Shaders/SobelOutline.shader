Shader "SnapshotProURP/SobelOutline"
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
			#pragma multi_compile __ USE_SCENE_TEXTURE_ON

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "SnapshotUtility.hlsl"

			sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
				float2 _MainTex_TexelSize;
				float _Threshold;
				float4 _OutlineColor;
				float4 _BackgroundColor;
			CBUFFER_END

			float sobel(float2 uv)
			{
				float3 x = 0;
				float3 y = 0;

				float2 pixel = _MainTex_TexelSize;

				x += tex2D(_MainTex, uv + float2(-pixel.x, -pixel.y)).xyz * -1.0f;
				x += tex2D(_MainTex, uv + float2(-pixel.x, 0)).xyz * -2.0f;
				x += tex2D(_MainTex, uv + float2(-pixel.x, pixel.y)).xyz * -1.0f;

				x += tex2D(_MainTex, uv + float2(pixel.x, -pixel.y)).xyz * 1.0f;
				x += tex2D(_MainTex, uv + float2(pixel.x, 0)).xyz * 2.0f;
				x += tex2D(_MainTex, uv + float2(pixel.x, pixel.y)).xyz * 1.0f;

				y += tex2D(_MainTex, uv + float2(-pixel.x, -pixel.y)).xyz * -1.0f;
				y += tex2D(_MainTex, uv + float2(0, -pixel.y)).xyz * -2.0f;
				y += tex2D(_MainTex, uv + float2(pixel.x, -pixel.y)).xyz * -1.0f;

				y += tex2D(_MainTex, uv + float2(-pixel.x, pixel.y)).xyz * 1.0f;
				y += tex2D(_MainTex, uv + float2(0, pixel.y)).xyz * 2.0f;
				y += tex2D(_MainTex, uv + float2(pixel.x, pixel.y)).xyz * 1.0f;

				float xLum = dot(x, float3(0.2126729, 0.7151522, 0.0721750));
				float yLum = dot(y, float3(0.2126729, 0.7151522, 0.0721750));

				return sqrt(xLum * xLum + yLum * yLum);
			}

            float4 frag (v2f i) : SV_Target
            {
				float s = sobel(i.uv);
				float4 col = tex2D(_MainTex, i.uv);

#ifdef USE_SCENE_TEXTURE_ON
				float4 background = col;
#else
				float4 background = _BackgroundColor;
#endif

				return lerp(background, _OutlineColor, s);
            }
            ENDHLSL
        }
    }
}
