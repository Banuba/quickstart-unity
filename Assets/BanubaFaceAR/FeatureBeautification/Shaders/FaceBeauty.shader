Shader "Unlit/FaceBeauty"
{
	Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EnableSharpenEyes ("Eyes sharpen on/off", Int) = 1
        _EnableSharpenTeeth ("Teeth sharpen on/off", Int) = 1
        _EnableSoftSkin ("Soft skin on/off", Int) = 1
        _EnableLookUpEyes ("Beaty eyes on/off", Int) = 1
        _LookUpEyesTexture ("Beaty eyes", 2D) = "white" {}
        _EnableLookUpTeeth ("Beaty teeth on/off", Int) = 1
        _LookUpTeethTexture ("Beaty teeth", 2D) = "white" 
        _EnableEyesFlare ("Eyes flare on/off", Int) = 1
        _EyesFlareTexture ("Eyes flare", 2D) = "black" {}
        _EnableEyesBlush ("Blush on/off", Int) = 1
        _EyesBlushTexture ("Blush", 2D) = "black" 
        _EnableMakeup ("Makeup on/off", Int) = 1
        _MakeupTexture ("Makeup", 2D) = "white" {}
        _LookUpMaskTexture ("Beaty mask", 2D) = "white" {}
        _SkinSoftIntensity("Skin Soft Intensity", Range(0.0, 1.0)) = 0.8
        _TeethSharpenIntensity("TeethSharpenIntensity", Range(0.0, 1.0)) = 0.9
        _EyesSharpenIntensity("Eyes Sharpen Intensity", Range(0.0, 1.0)) = 0.9
        _TeethWhiteningCoeff("Teeth Whitening Coeff", Range(0.0, 1.0)) = 1.0
        _EyesWhiteningCoeff("Eyes Whitening Coeff", Range(0.0, 1.0)) = 0.3
	}
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "LookUp.cginc"

            struct appdata
            {
                    float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 face_uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4x4 uvAveraging : UV_AVERAGING;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4x4 _TextureMVP;
            int _TextureRotate; // bool
            int _TextureYFlip; // bool

            float _SkinSoftIntensity;
            float _TeethSharpenIntensity;
            float _EyesSharpenIntensity;
            float _TeethWhiteningCoeff;
            float _EyesWhiteningCoeff;


            v2f vert(appdata v) {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

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

                o.uv.y = 1.0 - o.uv.y;
                
                //------- UV AVERAGING -------
                const float dx = 1.0 - _ScreenParams.z;
                const float dy = 1.0 - _ScreenParams.w;

                const float deltaOffset = 5.0;

                const float sOfssetXneg = -deltaOffset * dx;
                const float sOffsetYneg = -deltaOffset * dy;
                const float sOffsetXpos = deltaOffset * dx;
                const float sOffsetYpos = deltaOffset * dy;

                o.uvAveraging[0].xy = o.face_uv + float2(sOfssetXneg, sOffsetYneg);
                o.uvAveraging[1].xy = o.face_uv + float2(sOfssetXneg, sOffsetYpos);
                o.uvAveraging[2].xy = o.face_uv + float2(sOffsetXpos, sOffsetYneg);
                o.uvAveraging[3].xy = o.face_uv + float2(sOffsetXpos, sOffsetYpos);
			   
                float2 deltaZW = float2(dx, dy);

                o.uvAveraging[0].zw = o.face_uv + float2(-deltaZW.x, -deltaZW.y);
                o.uvAveraging[1].zw = o.face_uv + float2(deltaZW.x, -deltaZW.y);
                o.uvAveraging[2].zw = o.face_uv + float2(-deltaZW.x, deltaZW.y);
                o.uvAveraging[3].zw = o.face_uv + float2(deltaZW.x, deltaZW.y);
			    //--------------

                return o;
            }

            sampler2D _LookUpEyesTexture;
            sampler2D _LookUpTeethTexture;
            sampler2D _LookUpMaskTexture;
            sampler2D _EyesFlareTexture;
            sampler2D _EyesBlushTexture;
            sampler2D _BackgroundTexture;
            sampler2D _MakeupTexture;

            int _EnableSharpenEyes;
            int _EnableSharpenTeeth;
            int _EnableSoftSkin;

            int _EnableLookUpEyes;
            int _EnableLookUpTeeth;
            int _EnableEyesFlare;
            int _EnableEyesBlush;
            int _EnableMakeup;

            half4 frag(v2f i) : SV_Target
            {
                fixed4 maskColor = tex2D(_LookUpMaskTexture, i.uv);
                float4 res = tex2D(_MainTex, i.face_uv);;

                res = lerp(res, SHARPEN_RGB(res, maskColor.g * _TeethSharpenIntensity, i.uvAveraging, _MainTex),_EnableSharpenTeeth);
                res = lerp(res, SHARPEN_RGB(res, maskColor.b * _EyesSharpenIntensity, i.uvAveraging, _MainTex), _EnableSharpenEyes);
                res = lerp(res, WHITENING(res, maskColor.g * _TeethWhiteningCoeff, _LookUpTeethTexture), _EnableLookUpTeeth);
                res = lerp(res,WHITENING(res, maskColor.b * _EyesWhiteningCoeff, _LookUpEyesTexture), _EnableLookUpEyes);
                res += lerp(float4(0,0,0,0), float4(tex2D(_EyesFlareTexture, i.uv).xyz, 0.0),_EnableEyesFlare);
                res = lerp(res, SOFT_SKIN_RGB(res, maskColor.r * _SkinSoftIntensity, i.uvAveraging, _MainTex), _EnableSoftSkin);
                res = lerp(res, SOFT_LIGHT(res, tex2D(_EyesBlushTexture, i.uv)), _EnableEyesBlush);

                float4 makeup = tex2D(_MakeupTexture, i.uv);
                res.xyz = lerp(res.xyz,lerp(res.xyz, makeup.xyz, makeup.w), _EnableMakeup);
                return res;
            }
            ENDCG
        }

    }
}