Shader "VacuumShaders/Curved World/Sprites/Diffuse"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0


        //CurvedWorld Options
		[CurvedWorldRangeFade] V_CW_RangeFadeDrawer("Range Fade", float) = 0
    }

    SubShader 
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        
        
        #pragma shader_feature _ V_CW_RANGE_FADE
		#include "../cginc/CurvedWorld_UnitySprites.cginc"        

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
            float3 worldPosBeforeBend;
        };

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);


            CURVEDWORLD_RANGE_FADE_SETUP(o, v.vertex)

			V_CW_TransformPoint(v.vertex);



            v.vertex = UnityFlipSprite(v.vertex, _Flip);

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap (v.vertex);
            #endif
            
            o.color = v.color * _Color * _RendererColor;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
             CURVEDWORLD_RANGE_FADE_CALCULATE(IN)


            fixed4 c = SampleSpriteTexture (IN.uv_MainTex) * IN.color;
            o.Albedo = c.rgb * c.a;
            o.Alpha = c.a;

            o.Albedo *= 1 - CURVEDWORLD_RANGE_FADE_VALUE;
            o.Alpha  *= 1 - CURVEDWORLD_RANGE_FADE_VALUE;
        }
        ENDCG
    }

Fallback "Hidden/VacuumShaders/Curved World/VertexLit/Transparent"
}
