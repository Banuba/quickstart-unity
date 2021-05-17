// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Face"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4x4 _TextureMVP;
            int _TextureRotate; // bool
            int _TextureYFlip; // bool

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float4 texvert = mul(_TextureMVP, v.vertex);
                float2 uv = ((texvert.xy / texvert.w) * 0.5 + 0.5);

                uv = TRANSFORM_TEX(uv, _MainTex);
                uv.x = abs(_TextureYFlip - uv.x);
                uv.y = abs(_TextureYFlip - uv.y);

                if (_TextureRotate > 0)
                {
                    o.uv.x = uv.y;
                    o.uv.y = 1.0 - uv.x; // rotate also require selfie X-axis flip
                }
                else
                {
                    o.uv = uv;
                }

                // float2 uv = (o.vertex.xy / o.vertex.w) * 0.5 + 0.5;
                // o.uv = TRANSFORM_TEX(uv, _MainTex);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
