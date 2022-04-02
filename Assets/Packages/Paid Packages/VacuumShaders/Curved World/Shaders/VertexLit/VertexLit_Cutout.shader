Shader "Hidden/VacuumShaders/Curved World/VertexLit/Cutout" 
{
	Properties 
	{ 
		[CurvedWorldGearMenu] V_CW_Label_Tag("", float) = 0
		[CurvedWorldLabel] V_CW_Label_UnityDefaults("Default Visual Options", float) = 0


		//Albedo
		[CurvedWorldLargeLabel] V_CW_Label_Albedo("Albedo", float) = 0	
		_Color("  Color", color) = (1, 1, 1, 1)
		_MainTex ("  Map (RGB) Trans (A)", 2D) = "white" {}
		[CurvedWorldUVScroll] _V_CW_MainTex_Scroll("    ", vector) = (0, 0, 0, 0)

		//Cutoff
		[CurvedWorldLargeLabel] V_CW_Label_Cutoff("Cutout", float) = 0	
		_Cutoff ("  Alpha cutoff", Range(0,1)) = 0.5		



		//CurvedWorld Options
		[CurvedWorldLabel] V_CW_CW_OPTIONS("Unity Advanced Rendering Options", float) = 0
		
		[HideInInspector] _V_CW_IncludeVertexColor("", float) = 0

		[HideInInspector] _V_CW_Rim_Color("", color) = (1, 1, 1, 1)
		[HideInInspector] _V_CW_Rim_Bias("", Range(-1, 1)) = 0.2
		[HideInInspector] _V_CW_Rim_Power("", Range(0.5, 8.0)) = 3
	} 

	Category    
	{
		Tags { "Queue"="AlphaTest" 
		       "IgnoreProjector"="True" 
			   "RenderType"="CurvedWorld_TransparentCutout" 
			   "CurvedWorldTag"="VertexLit/Cutout" 
			   "CurvedWorldNoneRemoveableKeywords"=""  
			   "CurvedWorldAvailableOptions"="V_CW_VERTEX_COLOR;V_CW_RIM;V_CW_FOG;VERTEX_COLOR;V_CW_RANGE_FADE;" 
			 }
		LOD 150
	
		SubShader 
		{			 
		
			// Vertex Lit, emulated in shaders (4 lights max, no specular)
			Pass  
			{
				Tags { "LightMode" = "Vertex" }
				Lighting On 

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing


				#pragma shader_feature _ V_CW_RANGE_FADE
				#pragma shader_feature _ V_CW_RIM
				#pragma shader_feature _ V_CW_FOG
				#ifdef V_CW_FOG
					#pragma multi_compile_fog
				#endif

				#define V_CW_CUTOUT

				#include "../cginc/CurvedWorld_VertexLit.cginc"
				 
				
				ENDCG
			}
		 
			// Lightmapped
			Pass 
			{
				Tags { "LightMode" = "VertexLM" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
#pragma multi_compile_instancing


				#pragma shader_feature _ V_CW_RANGE_FADE
				#pragma shader_feature _ V_CW_RIM
				#pragma shader_feature _ V_CW_FOG
				#ifdef V_CW_FOG
					#pragma multi_compile_fog
				#endif

				#define V_CW_VERTEX_LIGHTMAP
				#define V_CW_CUTOUT

				#include "../cginc/CurvedWorld_VertexLit.cginc"


				ENDCG 
			} 
		   
			// Lightmapped, encoded as RGBM
			Pass 
			{
				Tags { "LightMode" = "VertexLMRGBM" }
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
#pragma multi_compile_instancing


				#pragma shader_feature _ V_CW_RANGE_FADE
				#pragma shader_feature _ V_CW_RIM
				#pragma shader_feature _ V_CW_FOG
				#ifdef V_CW_FOG
					#pragma multi_compile_fog
				#endif

				#define V_CW_VERTEX_LIGHTMAP
				#define V_CW_CUTOUT

				#include "../cginc/CurvedWorld_VertexLit.cginc"
				 
				ENDCG
			}
			 
			// Pass to render object as a shadow caster
			Pass {
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }
				ZWrite On ZTest LEqual

				CGPROGRAM
				// compile directives
				#pragma vertex vert_surf
				#pragma fragment frag_surf
				#pragma target 3.0
				#pragma multi_compile_instancing
				#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
				#pragma multi_compile_shadowcaster
				#include "HLSLSupport.cginc"
				
				#ifndef UNITY_INSTANCED_LOD_FADE
				#define UNITY_INSTANCED_LOD_FADE
				#endif

				#ifndef UNITY_INSTANCED_SH
				#define UNITY_INSTANCED_SH
				#endif

				#ifndef UNITY_INSTANCED_LIGHTMAPSTS
				#define UNITY_INSTANCED_LIGHTMAPSTS
				#endif

				#include "UnityShaderVariables.cginc"
				#include "UnityShaderUtilities.cginc"

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "UnityPBSLighting.cginc"
			

				#pragma shader_feature _ V_CW_RANGE_FADE
				#define V_CW_CUTOUT

				#include "../cginc/CurvedWorld_Base.cginc"				  
				#include "../cginc/CurvedWorld_RangeFade.cginc"


				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				fixed4 _Color;


				// vertex-to-fragment interpolation data
				struct v2f_surf {
				  V2F_SHADOW_CASTER;
				  float3 worldPos : TEXCOORD1;
				  float2  uv : TEXCOORD2;
				  UNITY_VERTEX_INPUT_INSTANCE_ID
				  UNITY_VERTEX_OUTPUT_STEREO

				  CURVEDWORLD_RANGE_FADE_COORDINATE(3)
				};

				// vertex shader
				v2f_surf vert_surf (appdata_full v) {
				  UNITY_SETUP_INSTANCE_ID(v);
				  v2f_surf o;
				  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
				  UNITY_TRANSFER_INSTANCE_ID(v,o);
				  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


				  CURVEDWORLD_RANGE_FADE_SETUP(o,v.vertex)
				  V_CW_TransformPoint(v.vertex);


				  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				  o.worldPos.xyz = worldPos;
				  o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				  return o;
				}

				// fragment shader
				fixed4 frag_surf (v2f_surf IN) : SV_Target 
				{
				  UNITY_SETUP_INSTANCE_ID(IN);


				  fixed4 texcol = tex2D( _MainTex, IN.uv );

				  //Distance Fade
				  CURVEDWORLD_RANGE_FADE_CALCULATE(IN) 

				  clip( texcol.a*_Color.a - _Cutoff - CURVEDWORLD_RANGE_FADE_VALUE);



				  SHADOW_CASTER_FRAGMENT(IN)
				}


				ENDCG

			}
		}
	}

	FallBack Off
	CustomEditor "CurvedWorld_Material_Editor"
}
