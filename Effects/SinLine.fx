sampler uImage0 : register(s0);

float flowPercent;
float uTime;
float uFlowTime;
//float4 shineC;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
    float yCoord = coords.y;
    float f = sqrt(1 - abs((coords.x - 0.5f) / 0.5));
    //根据X位置和时间添加浮动
    yCoord += sin(uTime + coords.x * 6.282) * flowPercent * f;
    
    //颜色偏移
    
    return color * tex2D(uImage0, float2(coords.x + uFlowTime, yCoord)) * (0.2 + 0.8 * f);
}

technique Technique1
{
    pass SinLinePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}