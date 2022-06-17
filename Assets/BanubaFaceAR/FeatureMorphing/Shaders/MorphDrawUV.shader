Shader "BNB/MorphDrawUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 vertexMorphing : VERTEX_MORPHING;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			v2f vert (appdata v)
			{
                v2f o;
                float2 vertex = v.uv;
                float2 ver = smoothstep(0.,1.,vertex)*2.0-1.0;
                o.vertex = float4(ver, 0, 1.0);
                const float max_range = 40.0;
                o.vertexMorphing = ((v.vertex-v.normal)/max_range) * 0.5 + 0.5;
                return o;
			}


			float4 frag (v2f i) : SV_Target
			{
                return float4(i.vertexMorphing, 1);
			}
            ENDCG
        }
    }
}
