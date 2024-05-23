sampler uImage0 : register(s0);
//缩放矩阵
matrix transformMatrix;
texture noiseTexture;
float uTime;
float lightRange;
//亮部颜色
float4 brightC;
//暗部颜色
float4 darkC;

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoords : TEXCOORD0;
    //float4 NewPosition : POSITION1;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    output.Position = mul(input.Position, transformMatrix);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TexCoords;

    float4 tc = tex2D(uImage0, coords);
    
    //获取灰度图中的灰度
    float2 nCoords = (float2(input.Position.x, input.Position.y) % float2(256, 256)) / float2(256, 256);
    float gray = tex2D(noiseTex, nCoords).r;

    //计算基本颜色
    float4 c1 = lerp(darkC, brightC, gray);

    //计算最终混合后的颜色
    float4 fc = tc * c1;
    
    //按时间变亮
    float t = frac(uTime);
    float a = abs(t - gray);
    if (a < lightRange)
    {
        fc = lerp(fc, float4(1, 1, 1, 1), (lightRange - a) / lightRange);
    }
    
    return fc;
}

technique Technique1
{
    pass CrystalPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};