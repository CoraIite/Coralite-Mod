float uTime;
matrix transformMatrix;
texture uTextImage;

sampler2D samplerTex = sampler_state
{
    texture = <uTextImage>;
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

    float2 st = float2(xcoord, (ycoord + uTime) % 1.0);
    float4 color = tex2D(samplerTex, st).xyzw;
	
    return input.Color * color;
}

technique Technique1
{
    pass MyNamePass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}