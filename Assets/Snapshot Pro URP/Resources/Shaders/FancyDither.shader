Shader "SnapshotProURP/FancyDither"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
		{ 
			"RenderType" = "Opaque" 
			"RenderPipeline" = "UniversalPipeline"
		}

		HLSLINCLUDE
			

			
		ENDHLSL

        Pass
        {
            HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "SnapshotUtility.hlsl"

			sampler2D _MainTex;
			sampler2D _NoiseTex;

			sampler2D _CameraDepthNormalsTexture;

			CBUFFER_START(UnityPerMaterial)
				float4 _NoiseTex_TexelSize;
				float _NoiseSize;
				float _ThresholdOffset;

				float4 _LightColor;
				float4 _DarkColor;

				float _Blend;
			CBUFFER_END

			// Credit to https://alexanderameye.github.io/outlineshader.html:
			float3 DecodeNormal(float4 enc)
			{
				float kScale = 1.7777;
				float3 nn = enc.xyz*float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
				float g = 2.0 / dot(nn.xyz, nn.xyz);
				float3 n;
				n.xy = g * nn.xy;
				n.z = g - 1;
				return n;
			}

            float4 frag (v2f i) : SV_Target
            {
				float3 col = tex2D(_MainTex, i.uv).xyz;

#if UNITY_REVERSED_Z
				float depth = SampleSceneDepth(i.uv);
#else
				float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(i.uv));
#endif

				float3 worldPos = ComputeWorldSpacePosition(i.uv, depth, UNITY_MATRIX_I_VP);

				float3 noiseUV = worldPos / _NoiseSize;

				float4 noiseX = tex2D(_NoiseTex, noiseUV.zy);
				float4 noiseY = tex2D(_NoiseTex, noiseUV.xz);
				float4 noiseZ = tex2D(_NoiseTex, noiseUV.xy);

				float3 normal = DecodeNormal(tex2D(_CameraDepthNormalsTexture, i.uv));

				float3 blend = pow(abs(normal), _Blend);
				blend /= dot(blend, 1.0f);

				float lum = Luminance(col);

				float3 noiseColor = noiseX * blend.x + noiseY * blend.y + noiseZ * blend.z;
				float threshold = dot(noiseColor, 1.0f) + _ThresholdOffset;

				col = lum < threshold ? _DarkColor.xyz : _LightColor.xyz;

				return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
