Shader "BNB/PostEffects/Morphing" 
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Pass
        {

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            sampler2D _TexWarp;
            
            float4 _MainTex_ST;
            float _isVertical;

            v2f vert(vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half2 uv_morph = tex2D(_TexWarp, float2(i.uv.x, i.uv.y)).xy;
                half2 uv = half2(i.uv.x + uv_morph.x, i.uv.y - uv_morph.y);
                half4 col = tex2D(_MainTex, uv);
                return col;
            }
        ENDCG
		}
	} 
}
