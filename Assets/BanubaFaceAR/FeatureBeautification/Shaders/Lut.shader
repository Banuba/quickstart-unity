Shader "Unlit/Lut"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}
        _LutTex ("Lut Texture", 2D) = "white" {}
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "LookUp.cginc"

            struct vertData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _LutTex;
            float4 _MainTex_ST;

            v2f vert(vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 rendered_color = tex2D(_MainTex, i.uv);
                float4 color = TEXTURE_LOOKUP(rendered_color, _LutTex);
                return color;
            }
            ENDCG
        }
    }
}