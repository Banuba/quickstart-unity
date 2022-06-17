Shader "UI/BackgroundShader"
{
    Properties
    {
        _MainTex ("Segmentation Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            //#pragma surface surf Standard fullforwardshadows alpha

            #include "UnityCG.cginc"

            struct vertData
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color  : COLOR;
                float4 vertex : SV_POSITION;
                float2 bg_uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _BGMaskTex;
            float4x4 _MaskTransform;
            float _Inverse;
            int _IsVerticallyMirrored;
            fixed4 _Color;
            float4 _MainTex_ST;

            v2f vert (vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.y = lerp(1.0 - o.uv.y, o.uv.y, _IsVerticallyMirrored);
                o.bg_uv = mul(float4(o.uv, 0, 1), _MaskTransform).xy;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mask_color = tex2D(_BGMaskTex, i.uv);
                float4 color = tex2D(_MainTex, i.bg_uv) * i.color;
                color = float4(color.xyz, abs(_Inverse - mask_color.x) * color.a);
                return color;
            }
            ENDCG
        }
    }
}
