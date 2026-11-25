// SpriteBatch binds the drawn image to texture slot s0.
sampler uImage0 : register(s0);

// Radius space: the full texture UV range (0..1) maps to (-1..1).
// The single expansion source stays fixed at UV (0.5, 0.5).
// Domain warping and height noise are bounded near the source to keep it centered.
float uTime = 0.0;                  // Continuous elapsed seconds; do not wrap to 0..1
float uExpandSpeed = 0.18;          // Radius units per second; positive moves contours outward
float uWarpStrength = 0.55;         // Spatial domain displacement; 0 disables domain warping
float uDistortStrength = 0.45;      // Additional height perturbation; 0 disables this detail
float uDistortSpeed = 0.40;         // Noise evolution speed; 0 freezes the distortion
float3 uContourColorCenter = float3(1.0, 0.80, 0.32); // Straight RGB at UV center, 0..1
float3 uContourColorEdge = float3(1.0, 0.25, 0.04);   // Straight RGB at radius 1 and beyond
float uContourOpacity = 0.65;       // Independent overlay opacity, 0..1
float uNoiseScale = 0.50;           // Radius-space units per noise cell; must be > 0
float uContourLevels = 9.0;         // Contour intervals per radius unit; must be > 0
float uLineWidth = 1.5;             // Full width of a contour in screen pixels; 0 disables lines

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

float4 PixelShaderFunction(float2 uv : TEXCOORD0, float4 vertexColor : COLOR0) : COLOR0
{
    float4 textureColor = tex2D(uImage0, uv);
    float4 image = textureColor * vertexColor;
    float2 centeredUV = (uv - float2(0.5, 0.5)) * 2.0;
    float radius = length(centeredUV);

    // Bend the coordinates BEFORE evaluating distances. Unlike a small radial
    // offset, a 2D domain warp produces broad folds and asymmetric lobes.
    float2 noisePosition = centeredUV / max(uNoiseScale, 0.001);
    float noiseTime = uTime * uDistortSpeed;
    float2 warp = float2(
        ValueNoise(noisePosition + noiseTime * float2(0.73, -0.51)),
        ValueNoise(noisePosition + float2(17.1, 3.7) + noiseTime * float2(-0.41, 0.67))
    ) * 2.0 - 1.0;
    // Retain the strong warp, limiting displacement near the origin so it
    // cannot move the source away from the center or create another zero.
    float2 displacement = warp * uWarpStrength;
    displacement *= min(1.0, radius * 0.75 / max(length(displacement), 0.0001));
    float distance = length(centeredUV + displacement);

    // Preserve the finer animated noise and irregular local contour bends.
    float detail = ValueNoise(noisePosition * 2.03 + warp * 1.3 + float2(13.7, 9.2)
                             + noiseTime * float2(-0.59, -0.37)) * 2.0 - 1.0;
    // Bound detail close to the center so height is zero only at the source.
    // Farther out, the full distortion strength is preserved.
    float detailAmplitude = min(max(uDistortStrength, 0.0), distance * 0.75);
    float height = distance + detail * detailAmplitude;
    float phase = (height - uTime * uExpandSpeed) * max(uContourLevels, 0.001);

    // The integer levels of phase are the contours. Convert phase distance
    // to screen pixels using derivatives so uLineWidth stays pixel based.
    float fraction = frac(phase);
    float distanceToLine = min(fraction, 1.0 - fraction);
    float phasePerPixel = max(fwidth(phase), 0.00001);
    float lineDistancePixels = distanceToLine / phasePerPixel;
    float halfWidth = max(uLineWidth, 0.0) * 0.5;
    float coverage = saturate(halfWidth + 0.5 - lineDistancePixels)
                   - saturate(0.5 - halfWidth - lineDistancePixels);

    // Keep the original lerp overlay, using premultiplied RGBA for AlphaBlend.
    // Only the image uses vertexColor. Contours use the UNTINTED texture alpha,
    // so fading/tinting the image does not also fade/tint the contour layer.
    // Use the original UV radius so color progresses steadily from the center
    // to the inscribed circle (edge midpoints), independent of animated warping.
    float gradient = saturate(radius);
    float3 contourColor = lerp(uContourColorCenter, uContourColorEdge, gradient);
    float4 contour = float4(contourColor * textureColor.a, textureColor.a);
    // Clamp the per-pixel blend weight, not the uniform alpha: the latter
    // triggers a preshader compilation failure in the legacy XNA compiler.
    float overlayOpacity = saturate(coverage * uContourOpacity);
    return lerp(image, contour, overlayOpacity);
}

technique Technique1
{
    pass ContourPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
