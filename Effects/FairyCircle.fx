sampler twistTex : register(s0);

float uTime;
float r;
float tr;
float4 edgeColor;
float4 innerColor;

float2 RectToPolar(float2 uv, float2 centerUV)
{
    uv = uv - centerUV; // �ƶ��������ĵ��м䣨��ǰ��ԭ�������½ǣ�
    float theta = atan2(uv.y, uv.x); // atan()ֵ��[-��/2, ��/2]һ�㲻��; atan2()ֵ��[-��, ��]���������y���꣬x���꣩
    float r = length(uv);
    return float2(theta, r);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    coords = (coords * tr * 2 - coords * tr * 2 % 2) / (tr * 2);
    
    // ֱ������ת������
    float2 thetaR = RectToPolar(coords, float2(0.5, 0.5));
    // ������ת�������UV
    float2 polarUV = float2(
        thetaR.x / 3.141593 * 0.5 + 0.5, // �ȴ�[-��, ��]ӳ�䵽[0, 1]
        thetaR.y + uTime);
    
    //����ͼƬ
    float4 twist = tex2D(twistTex, polarUV);
    
    //����Ť��ֵ
    float2 uvt = (twist.xy - float2(0.5, 0.5)) * 6;
    
    //����Ť�������uv
    float2 newuv = coords * tr * 2 + uvt;
    
    newuv = newuv / (tr * 2); //(newuv - newuv % 2) / (tr * 2);
    
    thetaR = RectToPolar(newuv, float2(0.5, 0.5));
    polarUV = float2(
        thetaR.x / 3.141593 * 0.5 + 0.5, // �ȴ�[-��, ��]ӳ�䵽[0, 1]
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