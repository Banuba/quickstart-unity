Shader "BNB/Makeup/Skin"
{
    Properties
    {
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Cull off
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
            #include "Assets/BanubaFaceAR/BaseAssets/Shaders/BNB_SHADERS.cginc"

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
            
            sampler2D _MaskTex;
            sampler2D _CameraTex;
            float4x4 _MaskTransform;
            float4x4 _AddRotation;
            float _SofteningStrength;
            float4 _Color;

            float4 skin_color(float4 camera_color, sampler2D mask_tex, float2 mask_uv, float4 target_skin_color)
            {
                float4 js_yuva = bnb_rgba_to_yuva(target_skin_color);

                float2 maskColor = js_yuva.yz;
                float beta = js_yuva.x;

                float y = bnb_rgba_to_yuva(camera_color).x;
                float2 uv_src = bnb_rgba_to_yuva(camera_color).yz;

                float alpha = abs(_MaskTransform[1].w - tex2D(mask_tex, mask_uv).a) * js_yuva.w;

                float2 uv = (1.0 - alpha) * uv_src + alpha * ((1.0 - beta) * maskColor + beta * uv_src);

                float u = uv.x - 0.5;
                float v = uv.y - 0.5;

                float r = y + 1.402 * v;
                float g = y - 0.3441 * u - 0.7141 * v;
                float b = y + 1.772 * u;

                return float4(r, g, b, 1.0);
            }

            float4 soften(sampler2D _MaskTex, float2 uv, float factor)
            {
                float4 camera = tex2D(_MaskTex, uv);
                float3 originalColor = camera.xyz;
                float3 screenColor = originalColor;

                float dx = 4.5 / _ScreenParams.x;
                float dy = 4.5 / _ScreenParams.y;

                float3 nextColor0 = tex2D(_MaskTex, float2(uv.x - dx, uv.y - dy)).xyz;
                float3 nextColor1 = tex2D(_MaskTex, float2(uv.x + dx, uv.y - dy)).xyz;
                float3 nextColor2 = tex2D(_MaskTex, float2(uv.x - dx, uv.y + dy)).xyz;
                float3 nextColor3 = tex2D(_MaskTex, float2(uv.x + dx, uv.y + dy)).xyz;

                float intensity = screenColor.g;
                float4 nextIntensity = float4(nextColor0.g, nextColor1.g, nextColor2.g, nextColor3.g);
                float4 lg = nextIntensity - intensity;

                const float PSI = 0.05;
                float4 curr = max(0.367 - abs(lg * (0.367 * 0.6 / (1.41 * PSI))), 0.);

                float summ = 1.0 + curr.x + curr.y + curr.z + curr.w;
                screenColor += (nextColor0 * curr.x + nextColor1 * curr.y + nextColor2 * curr.z + nextColor3 * curr.w);
                screenColor = screenColor * (factor / summ);

                screenColor = originalColor * (1. - factor) + screenColor;
                return float4(screenColor, camera.a);
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
                float mask = tex2D(_MaskTex, i.mask_uv).x;
                float4 softened = soften(_CameraTex, i.cam_uv, _SofteningStrength);
                float4 colored = skin_color(softened, _MaskTex, i.mask_uv, _Color);
                return float4(colored.rgb, mask);
            }
            ENDCG
        }
    }
}