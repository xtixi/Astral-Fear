Shader "VacuumShaders/Curved World/Sprites/Default"
{
	Properties
	{
		[CurvedWorldGearMenu] V_CW_Label_Tag("", float) = 0

		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0


		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull("Face Cull", Int) = 0

		 //CurvedWorld Options
		[CurvedWorldRangeFade] V_CW_RangeFadeDrawer("Range Fade", float) = 0
	}

	SubShader 
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"CurvedWorldTag"="Sprites/Default" 
			"CurvedWorldNoneRemoveableKeywords"="" 
			"CurvedWorldAvailableOptions"=""
		} 

		Cull[_Cull]
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex SpriteVert
				#pragma fragment SpriteFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

				#pragma shader_feature _ V_CW_RANGE_FADE
				#include "../cginc/CurvedWorld_UnitySprites.cginc"
			ENDCG
		}
	}
}
