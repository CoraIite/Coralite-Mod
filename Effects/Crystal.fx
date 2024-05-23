sampler uImage0 : register(s0);
//���ž���
matrix transformMatrix;
texture noiseTexture;
float uTime;
float lightRange;
//������ɫ
float4 brightC;
//������ɫ
float4 darkC;

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //ѭ��UV
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
    
    //��ȡ�Ҷ�ͼ�еĻҶ�
    float2 nCoords = (float2(input.Position.x, input.Position.y) % float2(256, 256)) / float2(256, 256);
    float gray = tex2D(noiseTex, nCoords).r;

    //���������ɫ
    float4 c1 = lerp(darkC, brightC, gray);

    //�������ջ�Ϻ����ɫ
    float4 fc = tc * c1;
    
    //��ʱ�����
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