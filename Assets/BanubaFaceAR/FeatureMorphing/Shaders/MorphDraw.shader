Shader "BNB/MorphDraw"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StaticPosTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100
        ZTest less
        ZWrite off

        cull back

        Pass
        {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


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
                float2 var_c : VAR_C;
                //float4 norm : NORMAL;
            };

            uniform int fxr_DrawID;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _UVMorphTex;
            float4 _UVMorphTex_ST;
            sampler2D _StaticPosTex;
            float4 _StaticPosTex_ST;
            int _DrawID;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                //o.norm = v.vertex;
                const int EXPAND_PASSES = 8;
                const float NPUSH = 75.;
                float scale = 1.0 - float(_DrawID)/float(EXPAND_PASSES+1);
                scale = scale*scale*(3. - 2.*scale); // smoothstep fall-off
                float d0 = float(_DrawID)/float(EXPAND_PASSES+1);
                float d1 = float(_DrawID+1)/float(EXPAND_PASSES+1);
                float4 npush_scale = float4(NPUSH*float(_DrawID)/float(EXPAND_PASSES), scale*0.5, (d1-d0)*0.5, (d0+d1)*0.5);

                const float max_range = 40.0;
                float4 trans = tex2Dlod( _UVMorphTex, float4(smoothstep(0.,1., float2(1.-o.uv.x, o.uv.y)), 0, 0) );
                float3 translation = trans.xyz * (2.0 * max_range) - max_range;
                float3 vpos = v.vertex.xyz + translation;

                o.vertex = UnityObjectToClipPos(float4( vpos * (1.0 + npush_scale.x / length(vpos)), 1.0 ));
                o.vertex.z = o.vertex.z*npush_scale.z + o.vertex.w*npush_scale.w;
                float4 pos_no_push = UnityObjectToClipPos(float4( vpos, 1. ));

                const float3 pos_min = float3(-80.5, -109.4, -19.63);
                const float3 pos_max = float3(80.5, 111.85, 113.25);
                float3 st_tex = tex2Dlod(_StaticPosTex, float4(1.-o.uv.x, 1.-o.uv.y, 0, 0)).xyz;
                float3 st_pos = pos_min + st_tex * (pos_max - pos_min);

                float4 trans_pos = UnityObjectToClipPos(float4( st_pos + translation, 1. ));

                o.var_c = npush_scale.y*(trans_pos.xy/trans_pos.w - pos_no_push.xy/pos_no_push.w);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                //float3 N = normalize(cross(ddx(i.norm), ddy(i.norm)));
                //float l = dot(N, float3(0., 0.8, 0.6)) * 0.5 + 0.5;
                //return float4(l * float3(1,0,0), 1.);

                return float4(i.var_c, 0, 1);
            }
            ENDCG
        }
    }
}
