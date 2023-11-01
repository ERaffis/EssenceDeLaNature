Shader "SnapshotProURP/Vortex"
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
				float2 _Center;
				float _Strength;
				float2 _Offset;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float2 distance = i.uv - _Center;
				float angle = length(distance) * _Strength;
				float x = cos(angle) * distance.x - sin(angle) * distance.y;
				float y = sin(angle) * distance.x + cos(angle) * distance.y;
				float2 uv = float2(x + _Center.x + _Offset.x, y + _Center.y + _Offset.y);

				float4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
}
