Shader "SnapshotProURP/BarrelDistortion"
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
				float4 _BackgroundColor;
				float _Strength;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 UVs = i.uv - 0.5f;
				UVs = UVs * (1 + _Strength * length(UVs) * length(UVs)) + 0.5f;

				float4 col = tex2D(_MainTex, UVs);
				col = (UVs.x >= 0.0f && UVs.x <= 1.0f && UVs.y >= 0.0f && UVs.y <= 1.0f) ? col : _BackgroundColor;
                return col;
            }
            ENDHLSL
        }
    }
}
