matrix transformMatrix;
float uTime;
float uTimeG;
float udissolveS;

texture uBaseImage;
texture uFlow;
texture uGradient;
texture uDissolve;

sampler2D baseTex = sampler_state
{
    texture = <uBaseImage>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //ѭ��UV
};

sampler2D flowTex = sampler_state
{
    texture = <uFlow>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //ѭ��UV
};

sampler2D gradientTex = sampler_state
{
    texture = <uGradient>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //ѭ��UV
};

sampler2D dissolveTex = sampler_state
{
    texture = <uDissolve>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //ѭ��UV
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
    
    //�ӵ�ͼ��ȡɫ
    float4 baseC = tex2D(baseTex, input.TexCoords).xyzw;
    //������ͼ��ȡɫ
    float4 flowC = tex2D(flowTex, st).xyzw;
    
    //��ɫ������
    float a = baseC.r + baseC.r * flowC.r;
    
    //��ɫ����ȡɫ
    float4 gradientC = tex2D(gradientTex, float2((ycoord + flowC.r *0.7) % 1.0, 0.5)).xyzw;
    
    st.r = (xcoord * 1.5 + 0.5 + uTime) % 1.0;
    //�ܽ�ɫ
    float4 dissolveC = tex2D(dissolveTex, st).xyzw;
    
    float f = input.TexCoords.x;
    f = f * f;
    
    a = lerp(a, 0, (udissolveS * (1 - f) * dissolveC.r));
    
    return gradientC * a;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}