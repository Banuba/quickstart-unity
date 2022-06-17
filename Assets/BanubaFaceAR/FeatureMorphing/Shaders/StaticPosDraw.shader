Shader "Unlit/StaticPosDraw"
{
    Properties
    {
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

            uniform float _MorphStr;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 real_vertex : VERTEX_MORPHING;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			v2f vert (appdata v)
			{
                v2f o;
                o.uv = v.uv;
                float2 ver = smoothstep(0.,1.,o.uv)*2.0-1.0;
                o.vertex = float4(ver, 0, 1.0);
                o.real_vertex = v.vertex;
                return o;
			}


			float4 frag (v2f i) : SV_Target
			{
                return float4(i.real_vertex.xyz, 1.0);
			}
            ENDCG
        }
    }
}
