sampler uImage0 : register(s0);

float uTime;

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
    float a = uTime;
    
    float2 coord = coords - 0.5 /* - res * 0.5*/;
    
    //Scaled texture coordinates (-1 to +1 and aspect correct)
    float2 uv = coord / 0.5 /*res.y*/;

    float2x2 rotMartix = float2x2(cos(a), sin(a), -sin(a), cos(a));

    float2 uv2 = mul(uv, rotMartix);

    uv2 += float2(0.5, 0.5);
    
    return tex2D(uImage0, uv);
}

technique Technique1
{
    pass Rotate
    {
        PixelShader = compile ps_2_0 Main();
    }
}