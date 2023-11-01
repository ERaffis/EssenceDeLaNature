Shader "SnapshotProURP/Greyscale"
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

            sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
				float _Strength;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
				col.rgb = lerp(col.rgb, Luminance(col.rgb), _Strength);
                return col;
            }
            ENDHLSL
        }
    }
}
