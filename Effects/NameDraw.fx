sampler uTextImage : register(s0);
sampler uColorImage : register(s1);
float uTime;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uTextImage, coords);

    if (!any(color))
        return color;
	
    float2 barCoord = float2((coords.x + uTime) % 1.0, 0);
    return tex2D(uColorImage, barCoord) * color;
}

technique Technique1
{
    pass MyNamePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}