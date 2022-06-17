#include "UnityCG.cginc"

#define PSI 0.1

#define eps 0.000001
#define lutSz 64.0
#define pxSz 512.0

static const fixed4 rgb2y = fixed4(0.299, 0.587, 0.114, 0.0);

uniform fixed _MORPHING_ENABLED;
uniform fixed _MORPHING_DEBUG;

//----------------- HELPING METHODS ----------------- 

inline fixed4 GET_LUMINANCE4(fixed4x4 color)
 {
    return mul(color, rgb2y);
}


inline fixed GET_LUMINANCE(fixed4 color)
{
    return dot(color, rgb2y);
}


inline fixed4 GET_WEIGHT(fixed intensity, fixed4 nextIntensity) 
{
    fixed4 lglg = log(nextIntensity / intensity) * log(nextIntensity / intensity);
    return exp(lglg / (-2.0 *  PSI  *  PSI ));
}

//-------- Blend modes ---------

float SOFT_LIGHT_CHANNEL(float colorA, float colorB)
{
	float result = (1.0 - 2.0 * colorB) * colorA * colorA + 2.0 * colorB * colorA;
	return result;
}


float4 SOFT_LIGHT(float4 colorA, float4 colorB)
{
	float r = SOFT_LIGHT_CHANNEL(colorA.r, colorB.r);
	float g = SOFT_LIGHT_CHANNEL(colorA.g, colorB.g);
	float b = SOFT_LIGHT_CHANNEL(colorA.b, colorB.b);

	return float4 (r, g, b, 1);
}

//-----------------

inline fixed4 TEXTURE_LOOKUP(fixed4 orignalColor, sampler2D lookUpTexture)
{
    orignalColor.x = saturate(orignalColor.x);
    orignalColor.y = saturate(orignalColor.y);
    orignalColor.z = saturate(orignalColor.z);
    orignalColor.w = saturate(orignalColor.w);
    
    fixed bValue = (orignalColor.b * 255.0) / 4.0;
    	    
    fixed2 mulB = clamp(floor(bValue) + fixed2(0.0, 1.0), 0.0, (lutSz - 1.0));
    fixed2 row = floor(mulB / 8.0 + eps);
    fixed4 row_col = fixed4(row, mulB - row * 8.0);
    fixed4 lookup = orignalColor.ggrr * ((lutSz - 1.0)/pxSz) + row_col * (lutSz/pxSz) + (0.5/pxSz);
    fixed b1w = bValue - mulB.x;
    
    fixed3 sampled1 = tex2D(lookUpTexture, lookup.zx).rgb;
    fixed3 sampled2 = tex2D(lookUpTexture, lookup.wy).rgb;
    
    fixed3 res = lerp(sampled1, sampled2, b1w);
    
    return fixed4(res, orignalColor.a);
}


inline fixed4 SOFT_SKIN_RGB(fixed4 originalColor, fixed factor, fixed4x4 uvAveraging, sampler2D _Texture) 
{
    fixed4 screenColor = originalColor;
    fixed intensity = GET_LUMINANCE(screenColor);
    fixed summ = 1.0;
        
    fixed4x4 nextColor;
    nextColor[0] = tex2D(_Texture, uvAveraging[0].xy);
    nextColor[1] = tex2D(_Texture, uvAveraging[1].xy);
    nextColor[2] = tex2D(_Texture, uvAveraging[2].xy);
    nextColor[3] = tex2D(_Texture, uvAveraging[3].xy);
    fixed4 nextIntensity = GET_LUMINANCE4(nextColor);
    fixed4 curr = 0.367 * GET_WEIGHT(intensity, nextIntensity);
    summ += dot(curr, fixed4(1.0, 1.0, 1.0, 1.0));
    screenColor += mul(curr, nextColor);
    screenColor = screenColor / summ;
        
    screenColor = lerp(originalColor, screenColor, factor);
    return screenColor;
}


inline fixed4 SHARPEN_RGB(fixed4 originalColor, fixed factor, fixed4x4 uvAveraging, sampler2D _Texture) 
{
    fixed4 total = 5.0 * originalColor - tex2D(_Texture, uvAveraging[0].zw) - 
    									 tex2D(_Texture, uvAveraging[1].zw) -
    									 tex2D(_Texture, uvAveraging[2].zw) -
    									 tex2D(_Texture, uvAveraging[3].zw);
    fixed4 result = lerp(originalColor, total, factor);
    result = clamp(result, 0.0, 1.0);
    return result;
}


inline fixed4 WHITENING(fixed4 originalColor, fixed factor, sampler2D lookup)
{
	fixed4 color = TEXTURE_LOOKUP(originalColor, lookup);
	color = lerp(originalColor, color, factor);
 	return color;
}