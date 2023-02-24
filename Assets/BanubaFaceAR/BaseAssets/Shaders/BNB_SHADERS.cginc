#ifndef BNB_SHADERS
#define BNB_SHADERS

#define EPSILON 1e-10
#define mod(x, y) (x-y * floor(x/y))

#define bnb_color_spaces_YUV2RGB_RED_CrV 1.402;
#define bnb_color_spaces_YUV2RGB_GREEN_CbU 0.3441;
#define bnb_color_spaces_YUV2RGB_GREEN_CrV 0.7141;
#define bnb_color_spaces_YUV2RGB_BLUE_CbU 1.772;

float4 bnb_rgba_to_yuva(float4 rgba)
{
    float4 yuva = 0;
    yuva.x = rgba.r * 0.299 + rgba.g * 0.587 + rgba.b * 0.114;
    yuva.y = rgba.r * -0.169 + rgba.g * -0.331 + rgba.b * 0.5 + 0.5;
    yuva.z = rgba.r * 0.5 + rgba.g * -0.419 + rgba.b * -0.081 + 0.5;
    yuva.w = rgba.a;
    return yuva;
}

float3 hsv2rgb(in float3 HSV)
{
    float R = abs(HSV.x * 6 - 3) - 1;
    float G = 2 - abs(HSV.x * 6 - 2);
    float B = 2 - abs(HSV.x * 6 - 4);
    float3 RGB = saturate(float3(R,G,B));
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float3 rgb2hsv(in float3 c)
{
    float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.zy, k.wz), float4(c.yz, k.xy), c.z < c.y ? 1.0 : 0.0);
    float4 q = lerp(float4(p.xyw, c.x), float4(c.x, p.yzx), p.x < c.x ? 1.0 : 0.0);
    float d = q.x - min(q.w, q.y);
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + EPSILON)), d / (q.x + EPSILON), q.x);
}

float2 rgb_hs(float3 rgb)
{
    float cmax = max(rgb.r, max(rgb.g, rgb.b));
    float cmin = min(rgb.r, min(rgb.g, rgb.b));
    float delta = cmax - cmin;
    float2 hs = float2(0, 0);
    if (cmax > cmin) {
        hs.y = delta / cmax;
        if (rgb.r == cmax)
            hs.x = (rgb.g - rgb.b) / delta;
        else {
            if (rgb.g == cmax)
                hs.x = 2. + (rgb.b - rgb.r) / delta;
            else
                hs.x = 4. + (rgb.r - rgb.g) / delta;
        }
        hs.x = frac(hs.x / 6.);
    }
    return hs;
}

float rgb_v(float3 rgb)
{
    return max(rgb.r, max(rgb.g, rgb.b));
}

float3 blendColor(float3 base, float3 blend, float2 brightness_contrast)
{
    float v = rgb_v(base) * brightness_contrast.y;
    float2 hs = rgb2hsv(blend).xy;
    return hsv2rgb(float3(hs.x, hs.y - brightness_contrast.x, v));
}

float3 blendColor(float3 base, float3 blend, float opacity, float2 brightness_contrast)
{
    return blendColor(base, blend, brightness_contrast) * opacity + base * (1.0 - opacity);
}

float2x2 transpose_local(float2x2 inMatrix)
{
    float2 i0 = inMatrix[0];
    float2 i1 = inMatrix[1];

    float2x2 outMatrix = float2x2(
        float2(i0.x, i1.x),
        float2(i0.y, i1.y)
    );

    return outMatrix;
}

float3x3 bnb_inverse_trs2d(float3x3 m)
{
    float2 s = 1. / float2(dot(float2(m[0].x, m[1].x), float2(m[0].x, m[1].x)),
                           dot(float2(m[0].y, m[1].y), float2(m[0].y, m[1].y)));
    float2x2 r = transpose_local(float2x2(m[0].xy * s, m[1].xy * s));
    float2 t = -1 * mul(float2(m[0].z, m[1].z), r);
    return float3x3(float3(r[0], t.x), float3(r[1], t.y), float3(0., 0., 1.));
}

float4 cubic(float v)
{
    float4 n = float4(1.0, 2.0, 3.0, 4.0) - v;
    float4 s = n * n * n;
    float x = s.x;
    float y = s.y - 4.0 * s.x;
    float z = s.z - 4.0 * s.y + 6.0 * s.x;
    float w = 6.0 - x - y - z;
    return float4(x, y, z, w) * (1.0 / 6.0);
}

float4 bnb_texture_bicubic(sampler2D tex, float2 uv, float2 tex_size)
{
    float2 invtex_size = 1.0 / tex_size;

    uv = uv * tex_size - 0.5;

    float2 fxy = frac(uv);
    uv -= fxy;

    float4 xcubic = cubic(fxy.x);
    float4 ycubic = cubic(fxy.y);

    float4 c = uv.xxyy + float2(-0.5, +1.5).xyxy;

    float4 s = float4(xcubic.xz + xcubic.yw, ycubic.xz + ycubic.yw);
    float4 offset = c + float4(xcubic.yw, ycubic.yw) / s;

    offset *= invtex_size.xxyy;

    float4 sample0 = tex2D(tex, offset.xz);
    float4 sample1 = tex2D(tex, offset.yz);
    float4 sample2 = tex2D(tex, offset.xw);
    float4 sample3 = tex2D(tex, offset.yw);

    float sx = s.x / (s.x + s.y);
    float sy = s.z / (s.z + s.w);

    return lerp(
        lerp(sample3, sample2, sx),
        lerp(sample1, sample0, sx),
        sy
    );
}

#endif // BNB_SHADERS
