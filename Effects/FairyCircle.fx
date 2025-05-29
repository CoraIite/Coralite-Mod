sampler twistTex : register(s0);

float uTime;
float r;
float tr;
float4 edgeColor;
float4 innerColor;

float2 RectToPolar(float2 uv, float2 centerUV)
{
    uv = uv - centerUV; // 移动坐标中心到中间（以前的原点在左下角）
    float theta = atan2(uv.y, uv.x); // atan()值域[-π/2, π/2]一般不用; atan2()值域[-π, π]（输入的是y坐标，x坐标）
    float r = length(uv);
    return float2(theta, r);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    coords = (coords * tr * 2 - coords * tr * 2 % 2) / (tr * 2);
    
    // 直角坐标转极坐标
    float2 thetaR = RectToPolar(coords, float2(0.5, 0.5));
    // 极坐标转纹理采样UV
    float2 polarUV = float2(
        thetaR.x / 3.141593 * 0.5 + 0.5, // θ从[-π, π]映射到[0, 1]
        thetaR.y + uTime);
    
    //采样图片
    float4 twist = tex2D(twistTex, polarUV);
    
    //计算扭曲值
    float2 uvt = (twist.xy - float2(0.5, 0.5)) * 6;
    
    //计算扭曲后的新uv
    float2 newuv = coords * tr * 2 + uvt;
    
    newuv = newuv / (tr * 2); //(newuv - newuv % 2) / (tr * 2);
    
    thetaR = RectToPolar(newuv, float2(0.5, 0.5));
    polarUV = float2(
        thetaR.x / 3.141593 * 0.5 + 0.5, // θ从[-π, π]映射到[0, 1]
        thetaR.y);
        
    float cuL = polarUV.y * 2 * tr;
    if (cuL < r - 2)
        return innerColor;
    if (cuL < r)
        return edgeColor;

    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass GlowingDustPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}