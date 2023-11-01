Shader "SnapshotProURP/Glitch"
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
			sampler2D _OffsetTex;

			CBUFFER_START(UnityPerMaterial)
				float _OffsetStrength;
				float _VerticalTiling;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv;

				float offset = tex2D(_OffsetTex, float2(i.uv.x, i.uv.y * _VerticalTiling));
				uv.x += (offset - 0.5f) * _OffsetStrength + 1.0f;
				uv.x %= 1;

				return tex2D(_MainTex, uv);
            }
            ENDHLSL
        }
    }
}
