sampler uLineTex : register(s0);
texture uFlowTex;

//用于控制浮动的流动速度
float uTime;
//用于控制浮动的叠加量
float flowAdd;
//线段的偏移量，0~1调整线段绘制范围
float lineO;
//线段颜色
float4 lineC;
float powC;
float lineEx;
matrix transformMatrix;


sampler2D FlowTex = sampler_state
{
    texture = <uFlowTex>;
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
    if (input.TexCoords.x > lineO)
        return float4(0, 0, 0, 0);
    
    //获取连线贴图的颜色
    float4 c = tex2D(uLineTex, float2(input.TexCoords.x / lineO, input.TexCoords.y));
    float4 lC = c * lineC;
    float4 fC = tex2D(FlowTex, float2(input.TexCoords.x + uTime, input.TexCoords.y)) * lineC;
    float f = 1 - abs(input.TexCoords.x - 0.5) / lineEx;
    lC.rgb += pow(f, powC) * flowAdd * fC.rgb * lC.rgb;
    
    return lC;
}

technique Technique1
{
    pass MyNamePass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}