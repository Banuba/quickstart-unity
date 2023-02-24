Shader "BNB/Makeup/EyeFace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Cull back
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        BlendOp Add
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"

            struct vertData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 face_uv : TEXCOORD1;
            };

            sampler2D _ContourTex;
            sampler2D _BlushesTex;
            sampler2D _HighlighterTex;
            sampler2D _EyeshadowTex;
            sampler2D _EyelinerTex;
            sampler2D _LashesTex;
            sampler2D _MakeupTex;

            float4 _ContourColor;
            float4 _BlushesColor;
            float4 _HighlighterColor;
            float4 _EyeshadowColor;
            float4 _EyelinerColor;
            float4 _LashesColor;


            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4x4 _TextureMVP;
            int _TextureRotate; // bool
            int _TextureYFlip; // bool

            int _IsMakeupTex; // bool

            float4 blend(float4 base, float4 target, sampler2D tex_mask, float2 uv)
            {
                float4 tex = tex2D(tex_mask, uv);
                tex.rgb += target.rgb;
                tex.a *= target.a;
                
                if (tex.a == 0.)
                    return base;
                if (base.a == 0.)
                    return tex;

                float a = 1. - (1. - base.a) * (1. - tex.a);
                float3 rgb = lerp(base.rgb * base.a, tex.rgb, tex.a) / a;

                return float4(rgb, a);
            }

            float4 blend(float4 base, sampler2D tex_mask, float2 uv)
            {
                return blend(base, float4(0., 0., 0., 1.), tex_mask, uv);
            }

            v2f vert(vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float4 texvert = mul(_TextureMVP, v.vertex);
                float2 uv_bg = ((texvert.xy / texvert.w) * 0.5 + 0.5);
                uv_bg = TRANSFORM_TEX(uv_bg, _MainTex);

                uv_bg.x = abs(_TextureYFlip - uv_bg.x);
                uv_bg.y = abs(_TextureYFlip - uv_bg.y);

                if (_TextureRotate > 0)
                {
                    o.face_uv.x = uv_bg.y;
                    o.face_uv.y = 1.0 - uv_bg.x; // rotate also require selfie X-axis flip
                }
                else
                {
                    o.face_uv = uv_bg;
                }
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.face_uv);
                color = float4(0,0,0,0);
                if(_IsMakeupTex > 0)
                {
                    color = blend(color, _MakeupTex, i.uv);    
                }
                else
                {
                    color = blend(color, _ContourColor, _ContourTex, i.uv);
                    color = blend(color, _BlushesColor, _BlushesTex, i.uv);
                    color = blend(color, _HighlighterColor, _HighlighterTex, i.uv);
                    color = blend(color, _EyeshadowColor, _EyeshadowTex, i.uv);
                    color = blend(color, _EyelinerColor, _EyelinerTex, i.uv);
                    color = blend(color, _LashesColor, _LashesTex, i.uv);
                }
                return color;
            }
            ENDCG
        }
    }
}