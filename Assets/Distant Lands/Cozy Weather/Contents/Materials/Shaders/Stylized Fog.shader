// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/Stylized Fog"
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
		LightIntensity("Light Intensity", Float) = 0
		[HDR]_LightColor("Light Color", Color) = (0,0,0,0)
		_FogSmoothness("Fog Smoothness", Float) = 0.1
		_FogIntensity("Fog Intensity", Float) = 1
		_FogOffset("Fog Offset", Float) = 1
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
		uniform half LightIntensity;
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
			float4 screenColor286 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPos.xy/ase_grabScreenPos.w);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth242 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float temp_output_280_0 = ( _FogDepthMultiplier * sqrt( eyeDepth242 ) );
			float temp_output_1_0_g17 = temp_output_280_0;
			float4 lerpResult28_g17 = lerp( _FogColor1 , _FogColor2 , saturate( ( temp_output_1_0_g17 / _FogColorStart1 ) ));
			float4 lerpResult41_g17 = lerp( saturate( lerpResult28_g17 ) , _FogColor3 , saturate( ( ( _FogColorStart1 - temp_output_1_0_g17 ) / ( _FogColorStart1 - _FogColorStart2 ) ) ));
			float4 lerpResult35_g17 = lerp( lerpResult41_g17 , _FogColor4 , saturate( ( ( _FogColorStart2 - temp_output_1_0_g17 ) / ( _FogColorStart2 - _FogColorStart3 ) ) ));
			float4 lerpResult113_g17 = lerp( lerpResult35_g17 , _FogColor5 , saturate( ( ( _FogColorStart3 - temp_output_1_0_g17 ) / ( _FogColorStart3 - _FogColorStart4 ) ) ));
			float4 temp_output_284_0 = lerpResult113_g17;
			float3 hsvTorgb238 = RGBToHSV( _LightColor.rgb );
			float3 hsvTorgb231 = RGBToHSV( temp_output_284_0.rgb );
			float3 hsvTorgb257 = HSVToRGB( float3(hsvTorgb238.x,hsvTorgb238.y,( hsvTorgb238.z * hsvTorgb231.z )) );
			float3 ase_worldPos = i.worldPos;
			float3 appendResult272 = (float3(1.0 , _FlareSquish , 1.0));
			float3 normalizeResult232 = normalize( ( ( ase_worldPos * appendResult272 ) - _WorldSpaceCameraPos ) );
			float dotResult229 = dot( normalizeResult232 , _SunDirection );
			half LightMask235 = saturate( pow( abs( ( (dotResult229*0.5 + 0.5) * LightIntensity ) ) , _LightFalloff ) );
			float temp_output_248_0 = ( temp_output_284_0.a * saturate( temp_output_280_0 ) );
			float4 lerpResult227 = lerp( temp_output_284_0 , float4( hsvTorgb257 , 0.0 ) , saturate( ( LightMask235 * ( 1.5 * temp_output_248_0 ) ) ));
			float4 lerpResult276 = lerp( screenColor286 , lerpResult227 , temp_output_248_0);
			o.Emission = lerpResult276.rgb;
			o.Alpha = saturate( ( ( 1.0 - saturate( ( ( ( ( ase_worldPos - _WorldSpaceCameraPos ).y * 0.1 ) * ( 1.0 / _FogSmoothness ) ) + ( 1.0 - _FogOffset ) ) ) ) * _FogIntensity ) );
		}

		ENDCG
	}
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-912,544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;227;-912,-720;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;228;-1552,-448;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DotProductOpNode;229;-1424,544;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-1616,16;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;231;-1600,-784;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;232;-1744,544;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;233;-1792,64;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;234;-720,816;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;235;-272,544;Half;False;LightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;236;-1888,-48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;237;-1952,-1088;Inherit;False;Property;_LightColor;Light Color;16;1;[HDR];Create;False;0;0;0;False;0;False;0,0,0,0;1.083397,1.392001,1.382235,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;238;-1600,-992;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;239;-1104,16;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;240;-1520,176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-1168,-464;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;242;-2896,-464;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;243;-2784,-576;Inherit;False;Property;_FogDepthMultiplier;Fog Depth Multiplier;13;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;244;-416,816;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;-1280,-368;Inherit;False;2;2;0;FLOAT;1.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;246;-2688,-464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;247;-2736,544;Inherit;False;Property;_FlareSquish;Flare Squish;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;248;-1424,-288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;249;-1216,528;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;250;-1744,832;Inherit;False;Property;_MoonDirection;Moon Direction;11;1;[HideInInspector];Create;False;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;251;-1584,-224;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;252;-1936,544;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;253;-1424,880;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;254;-2288,624;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;255;-416,544;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;256;-2480,48;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;257;-1216,-912;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;258;-1776,656;Inherit;False;Property;_SunDirection;Sun Direction;10;1;[HideInInspector];Create;False;0;0;0;False;0;False;0,0,0;0.6622303,0.6346595,0.3983196;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;259;-912,16;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;260;-2032,-80;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WorldPosInputsNode;261;-2544,-128;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;262;-2176,-80;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;-704,16;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;264;-272,816;Half;False;MoonMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-1408,-544;Inherit;False;235;LightMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;266;-1136,704;Half;False;Property;LightIntensity;Light Intensity;15;0;Create;False;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;267;-560,544;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;268;-912,304;Inherit;False;Property;_FogIntensity;Fog Intensity;18;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;269;-2512,320;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;270;-2176,464;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;271;-752,608;Half;False;Property;_LightFalloff;Light Falloff;14;0;Create;False;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;272;-2432,528;Inherit;False;FLOAT3;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-1776,224;Inherit;False;Property;_FogOffset;Fog Offset;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;274;-2080,176;Inherit;False;Property;_FogSmoothness;Fog Smoothness;17;0;Create;False;0;0;0;False;0;False;0.1;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;275;-912,816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;276;-480,-656;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;277;-1232,816;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;-1360,-832;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;279;-1344,16;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-2496,-528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;281;-1024,-496;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;282;-720,544;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;283;-416,-64;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;284;-1952,-576;Inherit;False;Simple Gradient;0;;17;ece53c110c682694c8953a12e134178f;0;1;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;285;-576,816;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-42.8,-536.019;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/Stylized Fog;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Front;2;False;;7;False;;False;0;False;;0;False;;True;0;Custom;0.5;True;False;1;True;Custom;HeightFog;Transparent;All;12;all;True;True;True;True;0;False;;True;222;False;;255;False;;255;False;;6;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;20;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ScreenColorNode;286;-744.1548,-847.7794;Inherit;False;Global;_GrabScreen0;Grab Screen 0;12;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;226;0;249;0
WireConnection;226;1;266;0
WireConnection;227;0;284;0
WireConnection;227;1;257;0
WireConnection;227;2;281;0
WireConnection;228;0;284;0
WireConnection;229;0;232;0
WireConnection;229;1;258;0
WireConnection;230;0;236;0
WireConnection;230;1;233;0
WireConnection;231;0;284;0
WireConnection;232;0;252;0
WireConnection;233;1;274;0
WireConnection;234;0;275;0
WireConnection;235;0;255;0
WireConnection;236;0;260;1
WireConnection;238;0;237;0
WireConnection;239;0;279;0
WireConnection;240;0;273;0
WireConnection;241;0;265;0
WireConnection;241;1;245;0
WireConnection;244;0;285;0
WireConnection;245;1;248;0
WireConnection;246;0;242;0
WireConnection;248;0;228;3
WireConnection;248;1;251;0
WireConnection;249;0;229;0
WireConnection;251;0;280;0
WireConnection;252;0;270;0
WireConnection;252;1;254;0
WireConnection;253;0;232;0
WireConnection;253;1;250;0
WireConnection;255;0;267;0
WireConnection;257;0;238;1
WireConnection;257;1;238;2
WireConnection;257;2;278;0
WireConnection;259;0;239;0
WireConnection;260;0;262;0
WireConnection;262;0;261;0
WireConnection;262;1;256;0
WireConnection;263;0;259;0
WireConnection;263;1;268;0
WireConnection;264;0;244;0
WireConnection;267;0;282;0
WireConnection;267;1;271;0
WireConnection;270;0;269;0
WireConnection;270;1;272;0
WireConnection;272;1;247;0
WireConnection;275;0;277;0
WireConnection;275;1;266;0
WireConnection;276;0;286;0
WireConnection;276;1;227;0
WireConnection;276;2;248;0
WireConnection;277;0;253;0
WireConnection;278;0;238;3
WireConnection;278;1;231;3
WireConnection;279;0;230;0
WireConnection;279;1;240;0
WireConnection;280;0;243;0
WireConnection;280;1;246;0
WireConnection;281;0;241;0
WireConnection;282;0;226;0
WireConnection;283;0;263;0
WireConnection;284;1;280;0
WireConnection;285;0;234;0
WireConnection;285;1;271;0
WireConnection;0;2;276;0
WireConnection;0;9;283;0
ASEEND*/
//CHKSM=0DC0AA556FDCBDDA8B849DEB4110281D345F9FA6