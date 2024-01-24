float uTime;
matrix transformMatrix;
texture sampleTexture;
//颜色图，大概是一张横向的色图
texture gradientTexture;

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
    float xcoord = input.TexCoords.x;
    float ycoord = input.TexCoords.y;

    float2 st = float2((xcoord + uTime) % 1.0, ycoord);
    float4 color = tex2D(samplerTex, st).xyzw;
    float4 color2 = tex2D(gradientTex, float2(input.TexCoords.y, 0)).xyzw; //读取颜色图
    float3 bright = color.xyz  * color2.xyz ;

    return float4(bright, input.Color.w * color.r);
}

technique Technique1
{
    pass MyNamePass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}