﻿sampler uImage0 : register(s0);
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

float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float2 uTargetPosition;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float3 bright = color.xyz * uColor + (color.r > uOpacity ? ((color.r - uOpacity) * uSaturation) : float3(0, 0, 0));
	 
    //float avg = ((uColor.x + uColor.y + uColor.z) / 3.0);

    return float4(bright, color.r)*uSecondaryColor.r;
}

technique Technique1
{
    pass StarsDustPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
