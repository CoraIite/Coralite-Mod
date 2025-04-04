sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float3 RGB2HSV(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsb2rgb(float3 c)
{
    float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6) - 3.0) - 1.0, 0, 1);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * lerp(float3(1, 1, 1), rgb, c.y);
}

float4 ArmorMyShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
  // cordinate system from -2~2 将坐标拉伸一下
    float fx = (coords.x * uImageSize0.x - uSourceRect.x) / uSourceRect.z;
    float fy = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float2 p = (float2(-0.5, 0.3) + float2(fx, fy)) * float2(0.5, 1.5);

    float yOff = cos(p.x + p.y * 2 + uTime * 1.5 + sin(uTime * 2) * 0.785)*1.1f;
    //和盔甲亮度混合
    float4 color2 = tex2D(uImage0, coords);
    
    if (!any(color2))
        return float4(0, 0, 0, 0);
    
    //变透明
    if (yOff < 0)
    {
        return color2 * sampleColor * (1 + yOff * 0.4f); //透明度
    }
    else //变颜色
    {
        float f = sin(uTime * 4 + 0.785) / 2 + 0.5;
        //目标颜色
        float3 cuC = RGB2HSV(color2.rgb);
        float h = lerp(cuC.x, 0.51 + 0.29 * (sin(uTime + p.y * 3 + p.x * 3) / 2 + 0.5), yOff);
        float3 tC = hsb2rgb(float3(h, lerp(0.1, cuC.y * 0.6, 1 - yOff * yOff * yOff * yOff), cuC.z));
        
        return float4(tC.rgb, color2.a) * sampleColor;
    }
}

technique Technique1
{
    pass ArmorMyShader
    {
        PixelShader = compile ps_3_0 ArmorMyShader();
    }
}