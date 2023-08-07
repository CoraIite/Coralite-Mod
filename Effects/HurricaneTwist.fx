sampler uTextImage : register(s0);
sampler uColorImage : register(s1);
float uTime;
float2 uResolution;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Resolution for scaling/centering
    float2 res = uResolution;
    //Centered pixel coordinates
    float2 coord = coords - 0.5 /* - res * 0.5*/;
    
    //Scaled texture coordinates (-1 to +1 and aspect correct)
    float2 u = coord / 0.5 /*res.y*/;
    //Radius
    float r = length(u);
    //Zoom
    float z = log(r) - uTime * 0.4;
    //Polar angle
    float a = atan(u.y/ u.x) / 3.1415926536;
    //Mipmap fix
    //fwidth(a) > 1.0 ? (a = 0.0) : a;
    
    //Get new, polar-coordinates for texture
    float2 uv = float2(z, a - z * 0.5 - uTime * 0.05);
    //Sample texture
    float4 tex = tex2D(uTextImage, uv);
    //Give it a blue color
    return  pow(1. - tex * r, float4(5, 3.5, 4, 1));
}

technique Technique1
{
    pass Hurricane
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}