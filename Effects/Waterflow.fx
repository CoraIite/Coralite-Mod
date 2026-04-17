sampler uImage0 : register(s0);

texture uFlowTex;
float uTime;
float addCount;
float yScale2;

sampler2D flowTex = sampler_state
{
    texture = <uFlowTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords) * color;
    
    float f = sin(uTime) * 0.1f + uTime * 0.1f; //u的偏移量
    float2 newUV = float2(coords.x + f, coords.y * yScale2);
    
    float4 addColor1 = tex2D(flowTex, newUV);
    f = cos(uTime) * 0.1f + uTime * 0.12f;
    newUV.x = coords.x + f;
    newUV.y = coords.y * yScale2 * 2 + 0.5f;
    float4 addColor2 = tex2D(flowTex, newUV);
    
    return baseColor + (addColor1 * addCount + addColor2 * addCount) * baseColor.a;
}

technique Technique1
{
    pass WaterFlowPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}