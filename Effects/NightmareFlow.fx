sampler uTextImage : register(s0);

float uTime;
float uOpacity;

const float4 cHashA4 = float4(0.0, 1.0, 57.0, 58.0);
const float3 cHashA3 = float3(1.0, 57.0, 113.0);
const float cHashM = 43758.54;

float4 Hashv4f(float p)
{
    return fmod(sin(p + cHashA4) * cHashM, float4(1.0,1.0,1.0,1.0));
}

float Noisefv2(float2 p)
{
    float2 i = floor(p);
    float2 f = fmod(p, 1.0);
    f = f * f * (3.0 - 2.0 * f);
    float4 t = Hashv4f(dot(i, cHashA3.xy));
    return lerp(lerp(t.x, t.y, f.x), lerp(t.z, t.w, f.x), f.y);
}

float Fbm2(float2 p)
{
    float s = 0.0;
    float a = 1.0;
    for (int i = 0; i < 6; i++)
    {
        s += a * Noisefv2(p);
        a *= 0.5;
        p *= 2.0;
    }
    return s;
}

float2 VortF(float2 q, float2 c)
{
    float2 d = q - c;
    return 0.25 * float2(d.y, -d.x) / (dot(d, d) + 0.05);
}

float2 FlowField(float2 q)
{
    float2 vr, c;
    float dir = 1.0;
    c = float2(fmod(uTime, 10.) - 20.0, 0.6 * dir);
    vr = float2(0.,0.);
    for (int k = 0; k < 30; k++)
    {
        vr += dir * VortF(4.0 * q, c);
        c = float2(c.x + 1.0, -c.y);
        dir = -dir;
    }
    return vr;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords - 0.5;
    uv.x *= 2;
    float2 p = uv;
    for (int i = 0; i < 5; i++)
        p -= FlowField(p) * 0.03;
    float3 col = Fbm2(5.0 * p + float2(-0.1 * uTime, 0.0)) * float3(0.3, 0.3, 0.8);
    
    float4 texC = tex2D(uTextImage, coords);
    return float4(col * texC.rgb, 1.0) * uOpacity;
}

technique Technique1
{
    pass NightmareFlowPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}