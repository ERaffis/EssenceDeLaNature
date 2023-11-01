Shader "SnapshotProURP/TextAdventure"
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
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "SnapshotUtility.hlsl"

			#define EPSILON 1.0e-6

			sampler2D _MainTex;
			sampler2D _CharacterAtlas;

			CBUFFER_START(UnityPerMaterial)
				float2 _MainTex_TexelSize;
				int _CharacterCount;
				float2 _CharacterSize;
				float4 _BackgroundColor;
				float4 _CharacterColor;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).rgb;

				float luminance = saturate(Luminance(LinearToSRGB(col)));
				luminance = saturate(luminance - EPSILON);

				float2 uv = (i.uv * _CharacterSize) % 1.0f;
				uv.x = (floor(luminance * _CharacterCount) + uv.x) / _CharacterCount;

				float characterMask = tex2D(_CharacterAtlas, uv).r;
				return lerp(_BackgroundColor, _CharacterColor, characterMask);
            }
            ENDHLSL
        }
    }
}
