Shader "BNB/Makeup/Brow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 cam_uv : TEXCOORD0;
                float2 mask_uv : TEXCOORD1;
            };

            sampler2D _BrowMaskTex;
            sampler2D _CameraTex;
            
            float4 _Color;
            float4x4 _AddRotation;
            float4x4 _MaskTransform;

            float blendSoftLight(float base, float blend)
            {
                return blend < 0.5
                           ? 2.0 * base * blend + base * base * (1.0 - 2.0 * blend)
                           : sqrt(base) * (2.0 * blend - 1.0) + 2.0 * base * (1.0 - blend);
            }

            float3 blendSoftLight(float3 base, float3 blend)
            {
                return float3(blendSoftLight(base.r, blend.r), blendSoftLight(base.g, blend.g),
                              blendSoftLight(base.b, blend.b));
            }

            float3 blendSoftLight(float3 base, float3 blend, float opacity)
            {
                return blendSoftLight(base, blend) * opacity + base * (1.0 - opacity);
            }

            v2f vert(vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 mask_uv = v.uv;
                mask_uv.y = lerp(1.0 - mask_uv.y, mask_uv.y, 0);
                o.mask_uv = mask_uv;
                
                float2 cam_uv = v.uv;
                cam_uv = cam_uv * 2 - 1;
                cam_uv = mul(_MaskTransform, cam_uv);
                cam_uv = mul(_AddRotation, cam_uv);
                cam_uv = (cam_uv + 1) / 2;
                o.cam_uv = cam_uv;
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 camera = tex2D(_CameraTex, i.cam_uv);
                float mask = tex2D(_BrowMaskTex, i.mask_uv).x;
                float3 colored = blendSoftLight(camera.rgb, _Color.rgb, _Color.a);
                return float4(colored, mask);
            }
            ENDCG
        }
    }
}