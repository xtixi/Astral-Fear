Shader "Custom/Standard RangeFade (No Curved World)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[HDR]_EmisionColor("Emission Color", Color) = (0, 0, 0, 0)
		[NoScaleOffset]_EmisionMap("Emission Map", 2D) = "white" {}
		[NoScaleOffset]_BumpMap ("Normalmap", 2D) = "bump" {}

		//CurvedWorld Options
		[CurvedWorldRangeFade] V_CW_RangeFadeDrawer("Range Fade", float) = 0
	}
	SubShader {
		 Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="CurvedWorld_TransparentCutout"}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard  vertex:vert fullforwardshadows alphatest:_Cutoff addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		//CurvedWorld RangeFade API
		#pragma shader_feature _ V_CW_RANGE_FADE
		#include "Assets/VacuumShaders/Curved World/Shaders/cginc/CurvedWorld_Base.cginc"
		#include "Assets/VacuumShaders/Curved World/Shaders/cginc/CurvedWorld_RangeFade.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmisionMap;

		struct Input {
			float2 uv_MainTex;
			float3 worldPosBeforeBend;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _EmisionColor;


		void vert (inout appdata_full v, out Input o) 
		{
        	UNITY_INITIALIZE_OUTPUT(Input,o);

			CURVEDWORLD_RANGE_FADE_SETUP(o, v.vertex)
        }

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			CURVEDWORLD_RANGE_FADE_CALCULATE(IN)

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Emission = tex2D (_EmisionMap, IN.uv_MainTex).rgb * _EmisionColor.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Alpha -= CURVEDWORLD_RANGE_FADE_VALUE;			 
		}
		ENDCG
	}
	FallBack "Hidden/VacuumShaders/Curved World/VertexLit/Cutout"
}
