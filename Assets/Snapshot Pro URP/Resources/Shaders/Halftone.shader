Shader "SnapshotProURP/Halftone"
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
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "SnapshotUtility.hlsl"

			sampler2D _MainTex;
			sampler2D _HalftoneTexture;

			CBUFFER_START(UnityPerMaterial)
				float _Softness;
				float _TextureSize;
				float2 _MinMaxLuminance;

				float4 _DarkColor;
				float4 _LightColor;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 halftoneUVs = i.uv * _ScreenParams.xy / _TextureSize;

				float4 col = tex2D(_MainTex, i.uv);
				float lum = saturate(Luminance(col.rgb));

				float halftone = tex2D(_HalftoneTexture, halftoneUVs).r;
				halftone = lerp(_MinMaxLuminance.x, _MinMaxLuminance.y, halftone);

				float halftoneSmooth = fwidth(halftone) * _Softness;
				halftoneSmooth = smoothstep(halftone - halftoneSmooth, halftone + halftoneSmooth, lum);

#ifdef USE_SCENE_TEXTURE_ON
				col = lerp(_DarkColor, col, halftoneSmooth);
#else
				col = lerp(_DarkColor, _LightColor, halftoneSmooth);
#endif

                return col;
            }
            ENDHLSL
        }
    }
}
