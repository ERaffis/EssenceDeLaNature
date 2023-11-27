Shader "HQSurvival/PBR-Metallic (With texture mask)" {
    Properties {
    	
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _Metallic ("Metallic/Roughness/AO (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (RGB)", 2D) = "black" {}
        _MaskedTint ("MaskedTint", Color) = (1,1,1,1)
        _MaskedTexture ("MaskedTexture (RGB)", 2D) = "white" {}

    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows 
        #pragma target 3.0

        sampler2D _MaskedTexture,_MainTex,_MaskTex,_Metallic,_Normal,_Color; 
        fixed3 _MaskedTint;

        struct Input {
           float2 uv_MaskedTexture;
           float2 uv_MainTex; 
           float2 uv_MaskTex; 
           float2 uv_Metallic; 
           float2 uv_Normal;
	};

        void surf (Input IN, inout SurfaceOutputStandard o) {

       
           float4 c = tex2D (_MainTex, IN.uv_MainTex);
           float mask = tex2D (_MaskTex, IN.uv_MainTex).r; //only using 1 channel so no need to use all channels with a half3/4 
           float3 mro = tex2D (_Metallic, IN.uv_Metallic);
           float3 i = tex2D (_MaskedTexture, IN.uv_MaskedTexture);
           float4 n = tex2D (_Normal, IN.uv_Normal);
           c.rgb = c.rgb * (1 - mask) + _MaskedTint * i.rgb * mask;
           // (1 - mask) this part inverts the mask and removes it from the texture.
           // "+ Color * Mask" adds the mask back into the texture with the Color.

           o.Albedo = c.rgb;
           o.Normal = UnpackNormal (n); 
           o.Metallic = mro.r;
           o.Smoothness = mro.g;
           o.Occlusion = mro.b;
           o.Alpha = c.a;
	}
        ENDCG
    }
    FallBack "Diffuse"
}
