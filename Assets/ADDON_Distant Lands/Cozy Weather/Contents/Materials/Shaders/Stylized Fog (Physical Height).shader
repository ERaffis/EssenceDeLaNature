// Made with Amplify Shader Editor v1.9.1.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Fog (Physical Height)"
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
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "HeightFog"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		ZWrite Off
		ZTest Always
		Stencil
		{
			Ref 222
			Comp NotEqual
			Pass Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float4 _FogColor1;
		uniform float4 _FogColor2;
		uniform float _FogDepthMultiplier;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _FogColorStart1;
		uniform float4 _FogColor3;
		uniform float _FogColorStart2;
		uniform float4 _FogColor4;
		uniform float _FogColorStart3;
		uniform float4 _FogColor5;
		uniform float _FogColorStart4;
		uniform float4 _LightColor;
		uniform float _FlareSquish;
		uniform float3 _SunDirection;
		uniform half _LightIntensity;
		uniform half _LightFalloff;
		uniform float _FogSmoothness;
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

		float2 UnStereo( float2 UV )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
			UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
			#endif
			return UV;
		}


		float3 InvertDepthDir72_g8( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
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
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth229 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float temp_output_219_0 = ( _FogDepthMultiplier * sqrt( eyeDepth229 ) );
			float temp_output_1_0_g10 = temp_output_219_0;
			float4 lerpResult28_g10 = lerp( _FogColor1 , _FogColor2 , saturate( ( temp_output_1_0_g10 / _FogColorStart1 ) ));
			float4 lerpResult41_g10 = lerp( saturate( lerpResult28_g10 ) , _FogColor3 , saturate( ( ( _FogColorStart1 - temp_output_1_0_g10 ) / ( _FogColorStart1 - _FogColorStart2 ) ) ));
			float4 lerpResult35_g10 = lerp( lerpResult41_g10 , _FogColor4 , saturate( ( ( _FogColorStart2 - temp_output_1_0_g10 ) / ( _FogColorStart2 - _FogColorStart3 ) ) ));
			float4 lerpResult113_g10 = lerp( lerpResult35_g10 , _FogColor5 , saturate( ( ( _FogColorStart3 - temp_output_1_0_g10 ) / ( _FogColorStart3 - _FogColorStart4 ) ) ));
			float4 temp_output_226_0 = lerpResult113_g10;
			float3 hsvTorgb224 = RGBToHSV( _LightColor.rgb );
			float3 hsvTorgb265 = RGBToHSV( temp_output_226_0.rgb );
			float3 hsvTorgb249 = HSVToRGB( float3(hsvTorgb224.x,hsvTorgb224.y,( hsvTorgb224.z * hsvTorgb265.z )) );
			float3 ase_worldPos = i.worldPos;
			float3 appendResult245 = (float3(1.0 , _FlareSquish , 1.0));
			float3 normalizeResult266 = normalize( ( ( ase_worldPos * appendResult245 ) - _WorldSpaceCameraPos ) );
			float dotResult263 = dot( normalizeResult266 , _SunDirection );
			half LightMask250 = saturate( pow( abs( ( (dotResult263*0.5 + 0.5) * _LightIntensity ) ) , _LightFalloff ) );
			float4 lerpResult260 = lerp( temp_output_226_0 , float4( hsvTorgb249 , 0.0 ) , saturate( ( LightMask250 * ( 1.5 * ( temp_output_226_0.a * saturate( temp_output_219_0 ) ) ) ) ));
			float RawFogAlpha307 = temp_output_226_0.a;
			float2 UV22_g9 = ase_screenPosNorm.xy;
			float2 localUnStereo22_g9 = UnStereo( UV22_g9 );
			float2 break64_g8 = localUnStereo22_g9;
			float clampDepth69_g8 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g8 = ( 1.0 - clampDepth69_g8 );
			#else
				float staticSwitch38_g8 = clampDepth69_g8;
			#endif
			float3 appendResult39_g8 = (float3(break64_g8.x , break64_g8.y , staticSwitch38_g8));
			float4 appendResult42_g8 = (float4((appendResult39_g8*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g8 = mul( unity_CameraInvProjection, appendResult42_g8 );
			float3 temp_output_46_0_g8 = ( (temp_output_43_0_g8).xyz / (temp_output_43_0_g8).w );
			float3 In72_g8 = temp_output_46_0_g8;
			float3 localInvertDepthDir72_g8 = InvertDepthDir72_g8( In72_g8 );
			float4 appendResult49_g8 = (float4(localInvertDepthDir72_g8 , 1.0));
			float4 temp_output_299_0 = mul( unity_CameraToWorld, appendResult49_g8 );
			float lerpResult300 = lerp( RawFogAlpha307 , ( RawFogAlpha307 * saturate( ( 1.0 - ( temp_output_299_0.y * 0.001 ) ) ) ) , ( 1.0 - saturate( ( distance( temp_output_299_0 , float4( _WorldSpaceCameraPos , 0.0 ) ) / ( _ProjectionParams.z * 1.0 ) ) ) ));
			float ModifiedFogAlpha308 = saturate( lerpResult300 );
			float4 lerpResult240 = lerp( screenColor246 , lerpResult260 , ModifiedFogAlpha308);
			o.Emission = lerpResult240.rgb;
			float temp_output_264_0 = ( ase_worldPos.y * ( 0.1 / _FogSmoothness ) );
			o.Alpha = saturate( ( ( 1.0 - saturate( temp_output_264_0 ) ) * _FogIntensity ) );
		}

		ENDCG
	}
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=19103
Node;AmplifyShaderEditor.CommentaryNode;305;-2297.089,1351.057;Inherit;False;2297.495;491.9313;;25;248;277;236;263;278;239;253;225;269;271;241;251;250;258;237;223;254;234;245;247;255;279;266;227;274;Sun/Moon Flares;1,0.7599752,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-2224,1552;Inherit;False;Property;_FlareSquish;Flare Squish;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;248;-2224,1408;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;245;-2048,1504;Inherit;False;FLOAT3;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;255;-1888,1504;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-1888,1408;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;279;-1648,1408;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenDepthNode;229;-2748.727,-129.206;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;227;-1520,1504;Inherit;False;Property;_SunDirection;Sun Direction;10;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0.6622303,0.6346595,0.3983196;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;266;-1520,1408;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SqrtOpNode;261;-2442.767,-124.5049;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;268;-2552.145,-260.7994;Inherit;False;Property;_FogDepthMultiplier;Fog Depth Multiplier;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;263;-1296,1408;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;-1152,1568;Half;False;Property;_LightIntensity;Light Intensity;15;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-2193.365,-140.9622;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;253;-1152,1408;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;299;-3504,432;Inherit;False;Reconstruct World Position From Depth;-1;;8;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;254;-928,1408;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;320;-3200,576;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;313;-3248,656;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ProjectionParams;316;-3008,752;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;226;-1997.266,-236.0786;Inherit;False;Simple Gradient;0;;10;ece53c110c682694c8953a12e134178f;0;1;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;301;-3168,432;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-3008,432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.001;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;312;-2848,592;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;317;-2800,752;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;223;-736,1408;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;-768,1488;Half;False;Property;_LightFalloff;Light Falloff;14;0;Create;False;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;262;-1776,-176;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleDivideOpNode;315;-2656,592;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;307;-1648,-112;Inherit;False;RawFogAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;237;-576,1408;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;306;-2880,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;318;-2544,592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;303;-2736,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;258;-432,1408;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-1408,688;Inherit;False;Property;_FogSmoothness;Fog Smoothness;17;0;Create;True;0;0;0;False;0;False;0.1;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;309;-2784,352;Inherit;False;307;RawFogAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;273;-1637.218,116.753;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;275;-1468.403,63.76376;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;231;-1408,544;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;272;-1904,-656;Inherit;False;Property;_LightColor;Light Color;16;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1.083397,1.392001,1.382235,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;267;-1216,688;Inherit;False;2;0;FLOAT;0.1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;319;-2427.832,527.7066;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;250;-288,1408;Half;False;LightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;-2592,432;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;235;-1453.592,-194.1233;Inherit;False;250;LightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;224;-1654.227,-652.1216;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;264;-1072,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;265;-1643.002,-441.2271;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;-1329.617,-22.96418;Inherit;False;2;2;0;FLOAT;1.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;300;-2416,352;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;259;-816,576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-1415.906,-483.6272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;-1218.707,-126.5659;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;289;-2272,352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;249;-1264,-640;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;221;-816,656;Inherit;False;Property;_FogIntensity;Fog Intensity;18;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;220;-1066.722,-158.5626;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;308;-2144,352;Inherit;False;ModifiedFogAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;228;-688,576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;246;-752.5202,-488.0923;Inherit;False;Global;_GrabScreen0;Grab Screen 0;5;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;-544,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;260;-965.6562,-381.7821;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-752,-208;Inherit;False;308;ModifiedFogAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;251;-288,1680;Half;False;MoonMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;278;-1296,1696;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;244;-928,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;298;-1072,784;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;243;-1232,784;Inherit;False;Property;_FogOffset;Fog Offset;19;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-928,1696;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;225;-592,1680;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;240;-525.2369,-306.8453;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;239;-1152,1696;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;222;-400,576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;274;-1520,1664;Inherit;False;Property;_MoonDirection;Moon Direction;11;1;[HideInInspector];Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;271;-432,1680;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;269;-736,1696;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;544,-16;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/Stylized Fog (Physical Height);False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Front;2;False;;7;False;;False;0;False;;0;False;;True;0;Custom;0.5;True;False;1;True;Custom;HeightFog;Transparent;All;12;all;True;True;True;True;0;False;;True;222;False;;255;False;;255;False;;6;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;20;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;245;1;277;0
WireConnection;247;0;248;0
WireConnection;247;1;245;0
WireConnection;279;0;247;0
WireConnection;279;1;255;0
WireConnection;266;0;279;0
WireConnection;261;0;229;0
WireConnection;263;0;266;0
WireConnection;263;1;227;0
WireConnection;219;0;268;0
WireConnection;219;1;261;0
WireConnection;253;0;263;0
WireConnection;254;0;253;0
WireConnection;254;1;234;0
WireConnection;320;0;299;0
WireConnection;226;1;219;0
WireConnection;301;0;299;0
WireConnection;302;0;301;1
WireConnection;312;0;320;0
WireConnection;312;1;313;0
WireConnection;317;0;316;3
WireConnection;223;0;254;0
WireConnection;262;0;226;0
WireConnection;315;0;312;0
WireConnection;315;1;317;0
WireConnection;307;0;262;3
WireConnection;237;0;223;0
WireConnection;237;1;236;0
WireConnection;306;0;302;0
WireConnection;318;0;315;0
WireConnection;303;0;306;0
WireConnection;258;0;237;0
WireConnection;273;0;219;0
WireConnection;275;0;262;3
WireConnection;275;1;273;0
WireConnection;267;1;242;0
WireConnection;319;0;318;0
WireConnection;250;0;258;0
WireConnection;304;0;309;0
WireConnection;304;1;303;0
WireConnection;224;0;272;0
WireConnection;264;0;231;2
WireConnection;264;1;267;0
WireConnection;265;0;226;0
WireConnection;252;1;275;0
WireConnection;300;0;309;0
WireConnection;300;1;304;0
WireConnection;300;2;319;0
WireConnection;259;0;264;0
WireConnection;238;0;224;3
WireConnection;238;1;265;3
WireConnection;276;0;235;0
WireConnection;276;1;252;0
WireConnection;289;0;300;0
WireConnection;249;0;224;1
WireConnection;249;1;224;2
WireConnection;249;2;238;0
WireConnection;220;0;276;0
WireConnection;308;0;289;0
WireConnection;228;0;259;0
WireConnection;233;0;228;0
WireConnection;233;1;221;0
WireConnection;260;0;226;0
WireConnection;260;1;249;0
WireConnection;260;2;220;0
WireConnection;251;0;271;0
WireConnection;278;0;266;0
WireConnection;278;1;274;0
WireConnection;244;0;264;0
WireConnection;244;1;298;0
WireConnection;298;0;243;0
WireConnection;241;0;239;0
WireConnection;241;1;234;0
WireConnection;225;0;269;0
WireConnection;225;1;236;0
WireConnection;240;0;246;0
WireConnection;240;1;260;0
WireConnection;240;2;310;0
WireConnection;239;0;278;0
WireConnection;222;0;233;0
WireConnection;271;0;225;0
WireConnection;269;0;241;0
WireConnection;0;2;240;0
WireConnection;0;9;222;0
ASEEND*/
//CHKSM=BA297A96936F33E24861BEA88C3352FF89A3FCD5