Shader "Custom/Distortion Particle" {
	Properties {
		[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Main Texture", 2D) = "black" {}
		_BumpTex ("Normalmap", 2D) = "bump" {}
		_BumpAmt ("Distortion", Float) = 10
		_InvFade ("Soft Particles Factor", Range(0, 3)) = 0.5
	}

	Category {

		Tags { "Queue"="Transparent"  "IgnoreProjector"="True"  "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog { Mode Off}

		SubShader 
		{
			Pass 
			{
				//Name "BASE"
				Tags { "LightMode" = "Always" }
				Blend SrcAlpha One
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_particles
				#include "UnityCG.cginc"


				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _TintColor;
				sampler2D _CameraDepthTexture;
				float _InvFade;

				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
				};

				struct v2f {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;

					#ifdef SOFTPARTICLES_ON
						float4 projPos : TEXCOORD1;
					#endif
				};

				

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord.rg = TRANSFORM_TEX( v.texcoord, _MainTex);

					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					
					return o;
				}

				

				half4 frag( v2f i ) : COLOR
				{
					float fade = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						fade = saturate (_InvFade * (sceneZ-partZ));
					#endif

					half4 tex = tex2D( _MainTex, i.texcoord.rg);
					return tex * _TintColor * 5 * fade;
				}
				ENDCG
			}

			GrabPass 
			{							
				//"_GrabTexture"
			}

			Pass 
			{
				//Name "BASE"
				Tags { "LightMode" = "Always" }
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_particles
				#include "UnityCG.cginc"


				sampler2D _BumpTex;
				float _BumpAmt;
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				float4 _BumpTex_ST;
					sampler2D _CameraDepthTexture;
				float _InvFade;

				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : POSITION;
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;
					
					#ifdef SOFTPARTICLES_ON
						float4 projPos : TEXCOORD3;
					#endif
				};

				

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif

					#if UNITY_UV_STARTS_AT_TOP
						float scale = -1.0;
					#else
						float scale = 1.0;
					#endif


					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.w;

					#if UNITY_SINGLE_PASS_STEREO
						o.uvgrab.xy = TransformStereoScreenSpaceTex(o.uvgrab.xy, o.uvgrab.w);
					#endif

					o.uvgrab.z /= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpTex );

					return o;
				}

			

				half4 frag( v2f i ) : COLOR
				{
					float fade = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						fade = saturate (_InvFade * (sceneZ-partZ));
					#endif

					half3 bump = UnpackNormal(tex2D( _BumpTex, i.uvbump));
					half alphaBump = (abs(bump.r + bump.g) - 0.01) * 25;
					half2 offset = bump.rg * _BumpAmt * _GrabTexture_TexelSize.xy * fade;
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

					float4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					col.a = saturate(col.a * alphaBump);

					return col;
				}
				ENDCG
			}
		}
	}
}
