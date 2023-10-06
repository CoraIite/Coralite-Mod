﻿//缩放矩阵
matrix transformMatrix;
//样板图，就是刀光灰度图
texture sampleTexture;
//颜色图，大概是一张横向的色图
texture gradientTexture;
float alpha;

sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

sampler2D gradientTex = sampler_state
{
    texture = <gradientTexture>;
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
    float4 color = tex2D(samplerTex, input.TexCoords).xyzw; //读取刀光灰度图
    float4 color2 = tex2D(gradientTex, float2(input.TexCoords.y, 0)).xyzw; //读取颜色图
    // 如果刀光灰度图上的r大于0.8f，也就是它颜色比较白的话那么就给它加地更加亮
    float3 bright = color.xyz /** (1.0 + color.x * 2.0)*/ * color2.xyz + (color.r > 0.8 ? ((color.r - 0.8) * 3.5) : float3(0, 0, 0));
    //透明度是由传入颜色的透明度诚意刀光灰度图的r
    return float4(bright, color.r) * alpha * input.Color.a;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};