﻿//缩放矩阵
matrix transformMatrix;
float uTime;

//样板图，触手本体图
texture sampleTexture;
//颜色图，触手减去的颜色图
texture extraTexture;
//减去的流动贴图是多少
float flowAlpha;
//循环多少次
float warpAmount;

sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

sampler2D extraTex = sampler_state
{
    texture = <extraTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

struct VertexShaderInput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
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
    float4 color = tex2D(samplerTex, input.TexCoords); //读取本体灰度图
    //乘以颜色
    float4 c = float4(input.Color.rgb * color.rgb + (color.w > 0.9 ? ((color.w - 0.9) * 2.8) : float3(0, 0, 0)), color.r);
    float4 c2 = flowAlpha * tex2D(extraTex, float2(uTime + input.TexCoords.x * warpAmount, input.TexCoords.y));
    //减去特殊流动图
    //最终返回
    return (c - c2) * input.Color.a;
}


technique Technique1
{
    pass NightmareTentaclePass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}