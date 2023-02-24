Shader "BNB/Makeup/Lips"
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
            #include "Assets/BanubaFaceAR/BaseAssets/Shaders/BNB_SHADERS.cginc"

            struct vertData
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 var_uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _CameraTex;
            sampler2D _LipsMaskTex;
            sampler2D _ShineMaskTex;
            sampler2D _NoiseTex;

            float4 _CameraTex_ST;
            float4 _LipsMaskTex_ST;

            float4 _LipsMaskTex_TexelSize;
            float4 _CameraTex_TexelSize;

            float4x4 _LipsTransform;
            float4x4 _AddRotation;
            float4 _Color;
            float4 _SaturationBrightness;
            float4 _ShineIntensityBleedingScale;
            float4 _GlitterBleedingIntensityGrain;
            float4 _LipsShineParams;
            
            int _IsMatt; // bool
            int _IsVerticalRatioCorrection; // bool

            static const float screen_aspect = _ScreenParams.y / _ScreenParams.x;
            static const float camera_aspect = _CameraTex_TexelSize.w / _CameraTex_TexelSize.z;
            static const float camera_aspect_inv = _CameraTex_TexelSize.z / _CameraTex_TexelSize.w;
            
            float3 lipstick(float3 bg)
            {
                float4 lips_shine = float4(
                    _SaturationBrightness.x,
                    _ShineIntensityBleedingScale.x,
                    _ShineIntensityBleedingScale.y,
                    _SaturationBrightness.y
                );

                float sCoef = lips_shine.x;

                float3 color_hsv = rgb2hsv(_Color.rgb);
                float3 bg_color_hsv = rgb2hsv(bg);

                float color_hsv_s = color_hsv.g * sCoef;
                if (sCoef > 1.)
                {
                    color_hsv_s = color_hsv.g + (1. - color_hsv.g) * (sCoef - 1.);
                }

                float3 color_lipstick = float3(
                    color_hsv.r,
                    color_hsv_s,
                    bg_color_hsv.b);

                return color_lipstick;
            }


            v2f vert(vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                float2 uv = v.uv;
                uv.y = lerp(1.0 - uv.y, uv.y, 0);
                
                float4 vert = mul(_AddRotation, o.vertex);
                float2 cam_uv;
                if(_IsVerticalRatioCorrection > 0) {
                    cam_uv = float2(vert.x, vert.y * camera_aspect_inv / screen_aspect); 
                }
                else {
                    cam_uv = float2(vert.x * camera_aspect / screen_aspect, vert.y);
                }
                cam_uv = cam_uv / vert.w * 0.5 + 0.5;
                cam_uv.x = abs(1 - cam_uv.x);
                cam_uv.y = abs(1 - cam_uv.y);
                
                o.var_uv = float4(cam_uv, uv.xy);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                const float v_norm = 1. / 0.85;
                float v_scale = rgb_v(_Color.rgb) * v_norm;

                float2 uv = i.var_uv.xy;
                float3 bg = tex2D(_CameraTex, uv.xy).xyz;

                float maskAlpha = bnb_texture_bicubic(_LipsMaskTex, i.var_uv.zw, _LipsMaskTex_TexelSize.zw).x;
                
                if(_IsMatt > 0)
                {
                    float4 lips_brightness_contrast = float4(1.0 - _SaturationBrightness.x, _SaturationBrightness.y, 0.,0.);
                    return float4(blendColor(bg, _Color.xyz, maskAlpha * _Color.w, lips_brightness_contrast.xy), maskAlpha);
                }
                
                float4 lips_shine = float4(
                    _SaturationBrightness.x,
                    _ShineIntensityBleedingScale.x,
                    _ShineIntensityBleedingScale.y,
                    _SaturationBrightness.y
                );
                float4 lips_glitter = float4(
                    _GlitterBleedingIntensityGrain.x,
                    _GlitterBleedingIntensityGrain.y,
                    _GlitterBleedingIntensityGrain.z,
                    _ShineIntensityBleedingScale.z
                );
                
                float nUVScale = _CameraTex_TexelSize.w / (lips_glitter.z * 256.);
                float4 noise = tex2D(_NoiseTex, i.var_uv.zw * nUVScale) * 2. - 1.;
                float nCoeff = lips_glitter.x * 0.0025;
                float3 bg_noised = tex2D(_CameraTex, uv.xy + noise.xy * nCoeff).xyz;

                // Lipstick
                float3 color_lipstick = lipstick(bg);
                float nCoeff2 = lips_glitter.y * 0.02;
                float color_lipstick_b_noised = lipstick(bg_noised).z + noise.z * nCoeff2;

                float vCoef = lips_shine.y;
                float sCoef1 = lips_shine.z;
                float bCoef = lips_shine.w * v_scale;
                float a = 20.;
                float b = .75;

                float3 color_lipstick_b = color_lipstick * float3(1., 1., bCoef);
                float3 color = maskAlpha * hsv2rgb(color_lipstick_b) + (1. - maskAlpha) * bg;

                // Shine
                float4 shineColor = tex2D(_ShineMaskTex, i.var_uv.zw);
                float shineAlpha = shineColor.x;

                float scale = 1. - (lips_glitter.w - 1.);

                float v_min = _LipsShineParams.x;
                float v_max = _LipsShineParams.y * scale;

                float x = (color_lipstick_b_noised - v_min) / (v_max - v_min);
                float y = 1. / (1. + exp(-(x - b) * a * (1. + x)));

                float v1 = color_lipstick_b_noised * (1. - maskAlpha) + color_lipstick_b_noised * maskAlpha * bCoef;
                float v2 = color_lipstick_b_noised + (1. - color_lipstick_b_noised) * vCoef * y;
                float v3 = lerp(v1, v2, y);

                float3 color_shine = float3(
                    color_lipstick.x,
                    color_lipstick.y * (1. - sCoef1 * y),
                    v3
                );
                
                color = lerp(color, hsv2rgb(color_shine), shineAlpha);
                return float4(lerp(bg, color, _Color.w), maskAlpha);
            }
            ENDCG
        }
    }
}