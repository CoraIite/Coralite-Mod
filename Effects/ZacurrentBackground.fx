//matrix transformMatrix;

float iTime;
float bright;
float divide;
float2 worldSize;
texture noise;

sampler2D noiseTex = sampler_state
{
    texture = <noise>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //Ñ­»·UV
};

//À´Ô´£ºhttps://www.shadertoy.com/view/XsX3DS

float rand(float2 p)
{
    return tex2D(noiseTex, p / divide).x;
}

float sn(float2 p)
{
    float2 i = floor(p - float2(.5,.5));
    float2 f = frac(p - float2(.5, .5));
    f = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    float rt = lerp(rand(i), rand(i + float2(1., 0.)), f.x);
    float rb = lerp(rand(i + float2(0., 1.)), rand(i + float2(1., 1.)), f.x);
    return lerp(rt, rb, f.y);
}

//struct VertexShaderInput
//{
//    float4 Position : SV_POSITION;
//    float4 Color : COLOR0;
//    float2 TexCoords : TEXCOORD0;
//};

//struct VertexShaderOutput
//{
//    float4 Position : SV_POSITION;
//    float4 Color : COLOR0;
//    float2 TexCoords : TEXCOORD0;
//    //float4 NewPosition : POSITION1;
//};

//VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
//{
//    VertexShaderOutput output;
    
//    output.Color = input.Color;
//    output.Position = /*mul(*/input.Position/*, transformMatrix)*/;
//    output.TexCoords = input.TexCoords;
//    //output.Position = output.NewPosition;
//    return output;
//}

float4 PixelShaderFunction(float4 color:COLOR0,float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = float2(coords.x *worldSize.x/ worldSize.y, coords.y);
    uv+=float2(0.,0.65);
	
    float2 p = uv.xy * float2(2., 3.3);
    float f =
	.5 * sn(p)
	+ .25 * sn(2. * p)
	+ .125 * sn(4. * p)
	+ .0625 * sn(8. * p)
	+ .03125 * sn(16. * p) +
	.015 * sn(32. * p);
    
    float newT = iTime * 0.4 + sn(float2(iTime * 1.0, iTime * 1.0)) * 0.1;
    p.x -= iTime * 0.2;
	
    p.y *= 1.3;
    float f2 =
	.5 * sn(p)
	+ .25 * sn(2.04 * p + newT * 1.1)
	- .125 * sn(4.03 * p - iTime * 0.3)
	+ .0625 * sn(8.02 * p - iTime * 0.4)
	+ .03125 * sn(16.01 * p + iTime * 0.5) +
	.018 * sn(24.02 * p);
	
    float f4 = f2 * smoothstep(0.0, 1., uv.y);
	
    float3 clouds = lerp(float3(-0.4, -0.4, -0.15), float3(1.5, 1.3, 1.6), f4 * f);

    float2 moonp = float2(0.5, 0.8);
    float moon = smoothstep(0.95, 0.956, 1. - length(uv - moonp));
    float2 moonp2 = moonp + float2(0.015, 0);
    moon -= smoothstep(0.93, 0.956, 1. - length(uv - moonp2));
    moon = clamp(moon, 0., 1.);
    moon += 0.3 * smoothstep(0.80, 0.956, 1. - length(uv - moonp));

    //clouds += pow(1. - length(uv - moonp), 1.7) * 0.3;

    clouds *= 0.8;
    clouds += moon + 0.2;

    float2 newUV = uv;
    newUV.x -= iTime * 0.3;
    newUV.y += iTime * 3.;
    float s = sin(iTime * 0.5 + sn(newUV)) * 0.1 + bright;
	
    float3 painting = clouds + clamp((s - 0.1), 0., 1.);
	
    float r = 1. - length(max(abs(coords * 2. - float2(1., 1.)) - float2(.5, .5), float2(0.,0.)));
    painting *= r;
	
    return float4(painting, color.a);
}

technique Technique1
{
    pass ZacurrentPass
    {
        //VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};