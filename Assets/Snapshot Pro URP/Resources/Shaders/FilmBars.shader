Shader "SnapshotProURP/FilmBars"
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
				float _Aspect;
			CBUFFER_END

            float4 frag (v2f i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float bars = step(abs(0.5f - i.uv.y) * 2.0f, aspect / _Aspect);

                return col * bars;
            }
            ENDHLSL
        }
    }
}
