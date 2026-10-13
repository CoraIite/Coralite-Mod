// SpriteBatch binds the drawn image to texture slot s0.
sampler uImage0 : register(s0);

// Screen pixel position (VPOS in ps_3_0, equivalent to SV_POSITION.xy)
// plus the camera's world position. Add any desired pattern offset here too.
float2 uWorldOffset;
float4 uContourColor;   // RGB: line color; A: line opacity, 0..1
float uNoiseScale;      // World pixels per noise cell; must be > 0
float uContourLevels;   // Number of contour intervals across the noise range; must be > 0
float uLineWidth;       // Full width of a contour in screen pixels; must be >= 0

float RandomValue(float2 cell)
{
    return frac(sin(dot(cell, float2(127.1, 311.7))) * 43758.5453);
}

float ValueNoise(float2 Point)
{
    float2 cell = floor(Point);
    float2 local = frac(Point);
    float2 blend = local * local * (3.0 - 2.0 * local);

    float low = lerp(RandomValue(cell), RandomValue(cell + float2(1.0, 0.0)), blend.x);
    float high = lerp(RandomValue(cell + float2(0.0, 1.0)),
                      RandomValue(cell + float2(1.0, 1.0)), blend.x);
    return lerp(low, high, blend.y);
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0, float4 vertexColor : COLOR0,
                           float2 screenPosition : VPOS) : COLOR0
{
    float4 image = tex2D(uImage0, uv) * vertexColor;
    float2 worldPosition = screenPosition + uWorldOffset;
    float height = ValueNoise(worldPosition / max(uNoiseScale, 0.001));
    float phase = height * max(uContourLevels, 0.001);

    // The integer levels of phase are the contours. Convert phase distance
    // to screen pixels using derivatives so uLineWidth stays pixel based.
    float fraction = frac(phase);
    float distanceToLine = min(fraction, 1.0 - fraction);
    float phasePerPixel = max(fwidth(phase), 0.00001);
    float lineDistancePixels = distanceToLine / phasePerPixel;
    float coverage = saturate((max(uLineWidth, 0.0) + 1.0) * 0.5 - lineDistancePixels);

    // Image alpha defines the shape. RGB is premultiplied for SpriteBatch's
    // default AlphaBlend; the supplied line opacity only affects the overlay.
    float3 lineRgb = uContourColor.rgb * image.a;
    float3 rgb = lerp(image.rgb, lineRgb, coverage * saturate(uContourColor.a));
    return float4(rgb, image.a);
}

technique Technique1
{
    pass ContourPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
