// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stepped Fog"
{
	Properties
	{
		[HDR]_FogColor1("Fog Color 1", Color) = (1,0,0.8999224,1)
		[HDR]_FogColor2("Fog Color 2", Color) = (1,0,0,1)
		[HDR]_FogColor3("Fog Color 3", Color) = (1,0,0.7469492,1)
		[HDR]_FogColor4("Fog Color 4", Color) = (0,0.8501792,1,1)
		[HDR]_FogColor5("Fog Color 5", Color) = (0.164721,0,1,1)
		_FogColorStart1("FogColorStart1", Float) = 1
		_FogColorStart2("FogColorStart2", Float) = 2
		_FogColorStart3("FogColorStart3", Float) = 3
		_FogColorStart4("FogColorStart4", Float) = 4
		[HideInInspector]_SunDirection("Sun Direction", Vector) = (0,0,0,0)
		_FlareSquish("Flare Squish", Float) = 1
		_FogDepthMultiplier("Fog Depth Multiplier", Float) = 1
		_LightFalloff("Light Falloff", Float) = 1
		_LightIntensity("Light Intensity", Float) = 0
		[HDR]_LightColor("Light Color", Color) = (0,0,0,0)
		_FogSmoothness("Fog Smoothness", Float) = 0.1
		_FogIntensity("Fog Intensity", Float) = 1
		_FogOffset("Fog Offset", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		ZTest Always
		Stencil
		{
			Ref 222
			Comp NotEqual
			Pass Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _FogDepthMultiplier;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _FogColorStart4;
		uniform float4 _FogColor5;
		uniform float _FogColorStart3;
		uniform float4 _FogColor4;
		uniform float _FogColorStart2;
		uniform float4 _FogColor3;
		uniform float _FogColorStart1;
		uniform float4 _FogColor2;
		uniform float4 _FogColor1;
		uniform float4 _LightColor;
		uniform float _FlareSquish;
		uniform float3 _SunDirection;
		uniform half _LightIntensity;
		uniform half _LightFalloff;
		uniform float _FogSmoothness;
		uniform float _FogOffset;
		uniform float _FogIntensity;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float2 UnStereo( float2 UV )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
			UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
			#endif
			return UV;
		}


		float3 InvertDepthDir72_g1( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor246 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPos.xy/ase_grabScreenPos.w);
			float2 appendResult331 = (float2(_WorldSpaceCameraPos.x , _WorldSpaceCameraPos.z));
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 UV22_g3 = ase_screenPosNorm.xy;
			float2 localUnStereo22_g3 = UnStereo( UV22_g3 );
			float2 break64_g1 = localUnStereo22_g3;
			float clampDepth69_g1 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g1 = ( 1.0 - clampDepth69_g1 );
			#else
				float staticSwitch38_g1 = clampDepth69_g1;
			#endif
			float3 appendResult39_g1 = (float3(break64_g1.x , break64_g1.y , staticSwitch38_g1));
			float4 appendResult42_g1 = (float4((appendResult39_g1*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g1 = mul( unity_CameraInvProjection, appendResult42_g1 );
			float3 temp_output_46_0_g1 = ( (temp_output_43_0_g1).xyz / (temp_output_43_0_g1).w );
			float3 In72_g1 = temp_output_46_0_g1;
			float3 localInvertDepthDir72_g1 = InvertDepthDir72_g1( In72_g1 );
			float4 appendResult49_g1 = (float4(localInvertDepthDir72_g1 , 1.0));
			float4 break333 = mul( unity_CameraToWorld, appendResult49_g1 );
			float2 appendResult332 = (float2(break333.x , break333.z));
			float Distance314 = ( _FogDepthMultiplier * sqrt( distance( appendResult331 , appendResult332 ) ) );
			float4 break343 = ( Distance314 > _FogColorStart4 ? _FogColor5 : ( Distance314 > _FogColorStart3 ? _FogColor4 : ( Distance314 > _FogColorStart2 ? _FogColor3 : ( Distance314 > _FogColorStart1 ? _FogColor2 : _FogColor1 ) ) ) );
			float temp_output_1_0_g1 = Distance314;
			float4 appendResult342 = (float4(_FogColorStart1 , _FogColorStart2 , _FogColorStart3 , _FogColorStart4));
			float4 break116_g1 = appendResult342;
			float lerpResult28_g1 = lerp( _FogColor1.a , _FogColor2.a , saturate( ( temp_output_1_0_g1 / break116_g1.x ) ));
			float lerpResult41_g1 = lerp( saturate( lerpResult28_g1 ) , _FogColor3.a , saturate( ( ( break116_g1.x - temp_output_1_0_g1 ) / ( 0.0 - break116_g1.y ) ) ));
			float lerpResult35_g1 = lerp( lerpResult41_g1 , _FogColor4.a , saturate( ( ( break116_g1.y - temp_output_1_0_g1 ) / ( break116_g1.y - break116_g1.z ) ) ));
			float lerpResult113_g1 = lerp( lerpResult35_g1 , _FogColor5.a , saturate( ( ( break116_g1.z - temp_output_1_0_g1 ) / ( break116_g1.z - break116_g1.w ) ) ));
			float4 appendResult344 = (float4(break343.r , break343.g , break343.b , lerpResult113_g1));
			float4 FogColors317 = appendResult344;
			float3 hsvTorgb224 = RGBToHSV( _LightColor.rgb );
			float3 hsvTorgb265 = RGBToHSV( FogColors317.xyz );
			float3 hsvTorgb249 = HSVToRGB( float3(hsvTorgb224.x,hsvTorgb224.y,( hsvTorgb224.z * hsvTorgb265.z )) );
			float3 ase_worldPos = i.worldPos;
			float3 appendResult245 = (float3(1.0 , _FlareSquish , 1.0));
			float3 normalizeResult266 = normalize( ( ( ase_worldPos * appendResult245 ) - _WorldSpaceCameraPos ) );
			float dotResult263 = dot( normalizeResult266 , _SunDirection );
			half LightMask250 = saturate( pow( abs( ( (dotResult263*0.5 + 0.5) * _LightIntensity ) ) , _LightFalloff ) );
			float temp_output_275_0 = ( FogColors317.w * saturate( Distance314 ) );
			float4 lerpResult260 = lerp( FogColors317 , float4( hsvTorgb249 , 0.0 ) , saturate( ( LightMask250 * ( 1.5 * temp_output_275_0 ) ) ));
			float4 lerpResult240 = lerp( screenColor246 , lerpResult260 , temp_output_275_0);
			o.Emission = lerpResult240.xyz;
			o.Alpha = saturate( ( ( 1.0 - saturate( ( ( ( ( ase_worldPos - _WorldSpaceCameraPos ).y * 0.1 ) * ( 1.0 / _FogSmoothness ) ) + ( 1.0 - _FogOffset ) ) ) ) * _FogIntensity ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=18935
0;1080;2194.286;607.5715;1668.925;1186.725;1;True;False
Node;AmplifyShaderEditor.FunctionNode;330;-3534,-8;Inherit;False;Reconstruct World Position From Depth;-1;;1;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;328;-3300,-193;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;333;-3200,0;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;332;-3024,0;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;331;-3024,-128;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;329;-2880,-64;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;261;-2576,-176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;268;-2752,-256;Inherit;False;Property;_FogDepthMultiplier;Fog Depth Multiplier;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-2432,-256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-2778.25,890.0894;Inherit;False;Property;_FlareSquish;Flare Squish;12;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;314;-2304,-256;Inherit;False;Distance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;303;-1616,-912;Inherit;False;Property;_FogColor1;Fog Color 1;1;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0.8999224,1;0,0,0,0.007843138;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;304;-1616,-1088;Inherit;False;Property;_FogColor2;Fog Color 2;2;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,1;0.002941191,0.01078431,0.01176471,0.1372549;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;245;-2484.25,880.0894;Inherit;False;FLOAT3;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-1616,-1168;Inherit;False;Property;_FogColorStart1;FogColorStart1;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;315;-1616,-1248;Inherit;False;314;Distance;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;248;-2553.066,664.5895;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;311;-1312,-976;Inherit;False;Property;_FogColorStart2;FogColorStart2;7;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;283;-1312,-896;Inherit;False;Property;_FogColor3;Fog Color 3;3;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0.7469492,1;0.002941191,0.01078431,0.01176471,0.5019608;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;325;-1152,-1152;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;319;-1312,-1120;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-2222.824,815.7579;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;255;-2341.629,971.1777;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Compare;320;-1056,-1120;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;324;-832,-1136;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;313;-1056,-976;Inherit;False;Property;_FogColorStart3;FogColorStart3;8;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;279;-1981.584,881.3034;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;293;-1056,-896;Inherit;False;Property;_FogColor4;Fog Color 4;4;1;[HDR];Create;True;0;0;0;False;0;False;0,0.8501792,1,1;0.01237367,0.04032898,0.05331356,0.7568628;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;227;-1819.237,997.1949;Inherit;False;Property;_SunDirection;Sun Direction;10;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;-0.8785914,-0.2893077,0.3799715;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;294;-752,-976;Inherit;False;Property;_FogColorStart4;FogColorStart4;9;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;266;-1789.585,881.3034;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;323;-624,-1152;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;291;-752,-896;Inherit;False;Property;_FogColor5;Fog Color 5;5;1;[HDR];Create;True;0;0;0;False;0;False;0.164721,0,1,1;0.07165718,0.1627603,0.1935297,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;321;-752,-1120;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;341;-719.3082,-734.3081;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;336;-1338.528,-742.5726;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;337;-820.3082,-726.3081;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;340;-929.3082,-706.3081;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;339;-683.3082,-670.3081;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;342;-414.3082,-858.3081;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Compare;322;-512,-1120;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;263;-1469.583,881.3034;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;338;-380.3082,-628.3081;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;-1187.327,1043.645;Half;False;Property;_LightIntensity;Light Intensity;15;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;253;-1266.119,871.5823;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;334;-224,-768;Inherit;False;Simple Alpha Gradient;-1;;1;56cd1a9a3eb3fb94db3fd3227a7bec18;0;7;115;FLOAT4;0,0,0,0;False;117;FLOAT;0;False;118;FLOAT;0;False;119;FLOAT;0;False;120;FLOAT;0;False;121;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;343;-128,-1104;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;254;-957.5832,881.3034;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;257;-1888,496;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;231;-1888,352;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;344;80,-1104;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;317;208,-1104;Inherit;False;FogColors;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;232;-1632,352;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;223;-765.5842,881.3034;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;-799.2432,955.9188;Half;False;Property;_LightFalloff;Light Falloff;14;0;Create;False;0;0;0;False;0;False;1;21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;230;-1504,352;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;242;-1568,480;Inherit;False;Property;_FogSmoothness;Fog Smoothness;17;0;Create;True;0;0;0;False;0;False;0.1;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;318;-2000,-256;Inherit;False;317;FogColors;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;316;-1808,112;Inherit;False;314;Distance;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;237;-615.6221,890.1;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;270;-1376,352;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;262;-1604.629,-103.2568;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;273;-1637.218,116.753;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;267;-1376,448;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;243;-1376,544;Inherit;False;Property;_FogOffset;Fog Offset;19;0;Create;True;0;0;0;False;0;False;1;0.85;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;258;-461.1883,889.8445;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;256;-1184,448;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;272;-2006.491,-736.3435;Inherit;False;Property;_LightColor;Light Color;16;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;2.979118,1.513802,1.430881,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;264;-1184,352;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;250;-315.8317,884.873;Half;False;LightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;275;-1468.403,63.76376;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;-1329.617,-22.96418;Inherit;False;2;2;0;FLOAT;1.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;244;-1056,352;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;265;-1643.002,-441.2271;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RGBToHSVNode;224;-1654.227,-652.1216;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;235;-1453.592,-194.1233;Inherit;False;250;LightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;-1218.707,-126.5659;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-1415.906,-483.6272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;259;-944,352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;228;-816,352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-832,432;Inherit;False;Property;_FogIntensity;Fog Intensity;18;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;220;-1066.722,-158.5626;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;249;-1263.036,-561.0774;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;-672,352;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;260;-965.6562,-381.7821;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;246;-752.5202,-488.0923;Inherit;False;Global;_GrabScreen0;Grab Screen 0;5;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;240;-525.2369,-306.8453;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;225;-622.1776,1159.289;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;222;-532,352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;274;-1795.329,1175.465;Inherit;False;Property;_MoonDirection;Moon Direction;11;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;271;-463.4237,1160.474;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;269;-769.2592,1164.896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-955.9682,1164.443;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;251;-316.6268,1154.062;Half;False;MoonMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;278;-1470.379,1225.391;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;239;-1284.198,1168.138;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/Stepped Fog;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Front;0;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;1;False;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;True;222;False;-1;255;False;-1;255;False;-1;6;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;333;0;330;0
WireConnection;332;0;333;0
WireConnection;332;1;333;2
WireConnection;331;0;328;1
WireConnection;331;1;328;3
WireConnection;329;0;331;0
WireConnection;329;1;332;0
WireConnection;261;0;329;0
WireConnection;219;0;268;0
WireConnection;219;1;261;0
WireConnection;314;0;219;0
WireConnection;245;1;277;0
WireConnection;325;0;315;0
WireConnection;319;0;315;0
WireConnection;319;1;310;0
WireConnection;319;2;304;0
WireConnection;319;3;303;0
WireConnection;247;0;248;0
WireConnection;247;1;245;0
WireConnection;320;0;325;0
WireConnection;320;1;311;0
WireConnection;320;2;283;0
WireConnection;320;3;319;0
WireConnection;324;0;315;0
WireConnection;279;0;247;0
WireConnection;279;1;255;0
WireConnection;266;0;279;0
WireConnection;323;0;315;0
WireConnection;321;0;324;0
WireConnection;321;1;313;0
WireConnection;321;2;293;0
WireConnection;321;3;320;0
WireConnection;341;0;303;4
WireConnection;336;0;315;0
WireConnection;337;0;304;4
WireConnection;340;0;283;4
WireConnection;339;0;293;4
WireConnection;342;0;310;0
WireConnection;342;1;311;0
WireConnection;342;2;313;0
WireConnection;342;3;294;0
WireConnection;322;0;323;0
WireConnection;322;1;294;0
WireConnection;322;2;291;0
WireConnection;322;3;321;0
WireConnection;263;0;266;0
WireConnection;263;1;227;0
WireConnection;338;0;291;4
WireConnection;253;0;263;0
WireConnection;334;115;342;0
WireConnection;334;117;341;0
WireConnection;334;118;337;0
WireConnection;334;119;340;0
WireConnection;334;120;339;0
WireConnection;334;121;338;0
WireConnection;334;1;336;0
WireConnection;343;0;322;0
WireConnection;254;0;253;0
WireConnection;254;1;234;0
WireConnection;344;0;343;0
WireConnection;344;1;343;1
WireConnection;344;2;343;2
WireConnection;344;3;334;0
WireConnection;317;0;344;0
WireConnection;232;0;231;0
WireConnection;232;1;257;0
WireConnection;223;0;254;0
WireConnection;230;0;232;0
WireConnection;237;0;223;0
WireConnection;237;1;236;0
WireConnection;270;0;230;1
WireConnection;262;0;318;0
WireConnection;273;0;316;0
WireConnection;267;1;242;0
WireConnection;258;0;237;0
WireConnection;256;0;243;0
WireConnection;264;0;270;0
WireConnection;264;1;267;0
WireConnection;250;0;258;0
WireConnection;275;0;262;3
WireConnection;275;1;273;0
WireConnection;252;1;275;0
WireConnection;244;0;264;0
WireConnection;244;1;256;0
WireConnection;265;0;318;0
WireConnection;224;0;272;0
WireConnection;276;0;235;0
WireConnection;276;1;252;0
WireConnection;238;0;224;3
WireConnection;238;1;265;3
WireConnection;259;0;244;0
WireConnection;228;0;259;0
WireConnection;220;0;276;0
WireConnection;249;0;224;1
WireConnection;249;1;224;2
WireConnection;249;2;238;0
WireConnection;233;0;228;0
WireConnection;233;1;221;0
WireConnection;260;0;318;0
WireConnection;260;1;249;0
WireConnection;260;2;220;0
WireConnection;240;0;246;0
WireConnection;240;1;260;0
WireConnection;240;2;275;0
WireConnection;225;0;269;0
WireConnection;225;1;236;0
WireConnection;222;0;233;0
WireConnection;271;0;225;0
WireConnection;269;0;241;0
WireConnection;241;0;239;0
WireConnection;241;1;234;0
WireConnection;251;0;271;0
WireConnection;278;0;266;0
WireConnection;278;1;274;0
WireConnection;239;0;278;0
WireConnection;0;2;240;0
WireConnection;0;9;222;0
ASEEND*/
//CHKSM=F5DA8270075338BC50462AC5CECD52612A0C3BF0