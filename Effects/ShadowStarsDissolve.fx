matrix transformMatrix;

texture baseTexture;
texture exTexture;
float2 worldSize;

float uTime;

float uExchange;
float baseMult;

sampler2D baseTex = sampler_state
{
    texture = <baseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

sampler2D exTex = sampler_state
{
    texture = <exTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
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

#define rot(a) float2x2(cos(a),-sin(a),sin(a),cos(a))

float3 mod(float3 x, float y)
{
    return x - y * floor(x / y);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TexCoords;
    
    //读取贴图1，本身的贴图
    float4 tc = tex2D(baseTex, float2(1 - coords.x, coords.y));
    
    float2 coords2 = coords + float2(-uTime*5, 0);
    //读取图2，获取叠加图
    float4 color2 = tex2D(exTex, coords2);

    //获得叠加后的颜色
    tc = tc * baseMult + color2 * sqrt(1 - coords.x);
    
    if (tc.r < uExchange)//透明度是由传入颜色的透明度乘以刀光灰度图的r
        return input.Color * tc.r;
    
    float4 O = float4(0, 0, 0, 0);
    O -= O;
    float t = uTime * .01 + .25, s = .6, f = 2, a, l;

    float2 R = worldSize; 
    float3 p,
	      D = float3((input.Position.xy - .5 * R.xy) / R.x * .4, .5),
          o = float3(1, .5, .5) + float3(t + t, t, -2);
    
    D.xy /= 3.0;
    float2x2 r1 = rot(.5+1.0/*M.x*/),
	     r2 = rot(.8+1.0/*M.y*/);
    D.xz = mul(D.xz, r1);
    o.xz = mul(o.xz, r1);
    D.xy = mul(D.xy, r2);
    o.xy = mul(o.xy, r2);
	
    for (int i, r = 0; r++ < 4; f *= .93, s += .1)
    {
        p = abs(mod(o + s * D, 1.7) - .85);
        a = t = 0.;
        for (i = 0; i++ < 15; t = l)
            l = length(p = abs(p) / dot(p, p) - .53),
			a += abs(l - t);

        a *= a * a;
        r > 7 ? f *= min(1., .7 + a * a * .001) : f;
        O.rgb += float3(f, f, f) + s * float3(0.3, 0, s * s * s) * a * .0015 * f;
    }
	
    float y = .0015 * length(O);
    O = .0085 * O + float4(y, y, y, y);
    
    return float4(O.xyz, input.Color.a);
}

technique Technique1
{
    pass StarsTrailPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};