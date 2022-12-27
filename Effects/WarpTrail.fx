﻿sampler uImage0 : register(s0);
texture2D tex0;
sampler2D uImage1=sampler_state
{
    texture = <tex0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float i;
float4 PixelShaderFunction(float2 coords:TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage1, coords);
    if (!any(color2))
        return color;
    else
    {
        float2 vec = float2(0, 0);
        float rot = color2.r * 6.28;
        vec = float2(cos(rot), sin(rot)) * color2.g * i;
        return tex2D(uImage0, coords + vec);
    }
}

technique Technique1
{
    pass PrimitivesPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};
