//缩放矩阵
//matrix transformMatrix;

sampler uImage0 : register(s0);
texture exTexture;
float uTime;
float4 uSourceRect;
float2 uImageSize0;

float uExchange;
float uLerp;

sampler2D exTex = sampler_state
{
    texture = <exTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

//struct VertexShaderInput
//{
//    float4 Position : SV_POSITION;
//    float2 TexCoords : TEXCOORD0;
//    float4 Color : COLOR0;
//};

//struct VertexShaderOutput
//{
//    float4 Position : SV_POSITION;
//    float2 TexCoords : TEXCOORD0;
//    //float4 NewPosition : POSITION1;
//    float4 Color : COLOR0;
//};

//VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
//{
//    VertexShaderOutput output;
    
//    output.Color = input.Color;
//    output.TexCoords = input.TexCoords;
//    output.Position = mul(input.Position, transformMatrix);
//    //output.Position = output.NewPosition;
//    return output;
//}


// Star Nest by Pablo Roman Andrioli
// License: MIT

//#define iterations 17
//#define formuparam 0.53

//#define volsteps 20
//#define stepsize 0.1

//#define zoom   0.800
//#define tile   0.850
//#define speed  0.010 

//#define brightness 0.0015
//#define darkmatter 0.300
//#define distfading 0.730
//#define saturation 0.850

#define rot(a) float2x2(cos(a),-sin(a),sin(a),cos(a))

float3 mod(float3 x, float y)
{
    return x - y * floor(x / y);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //float2 coords = input.TexCoords;

    float4 tc = tex2D(uImage0, coords);
    
    float3 bright = tc.rgb * sampleColor.rgb; //input.Color.rgb;
    
    float fx = (coords.x * uImageSize0.x - uSourceRect.x) / uSourceRect.z;
    float fy = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float2 uv = float2(fx, fy);
    
    float4 color2 = tex2D(exTex, uv);

    float finR = tc.r * color2.r;
    float4 baseC = float4(bright, /*input.Color.w **/finR);
    if (tc.r < uExchange)
    {
    //透明度是由传入颜色的透明度诚意刀光灰度图的r
        return baseC;
    }
    
    //float2 R = input.NewPosition.xy / worldSize.xy - float2(0.5f,0.5f);
    
    float4 O = float4(0, 0, 0, 0);
    O -= O;
    float t = uTime * .01 + .25, s = .6, f = 2, a, l;

    //float2 R = worldSize; /*float2(800, 450);*/

    float3 p,
	      D = float3((uv - float2(0.5f, 0.5f)) * .4, .5),
          //M = 2. * iMouse.xyz / R,
          o = float3(1, .5, .5) + float3(t + t, t, -2);
    //O -= O;
    D.xy /= 3.0;
    float2x2 r1 = rot(.5+1.0/*M.x*/),
	     r2 = rot(.8+1.0/*M.y*/);
    D.xz = mul(D.xz, r1);
    o.xz = mul(o.xz, r1);
    D.xy = mul(D.xy, r2);
    o.xy = mul(o.xy, r2);
    //D.xz *= r1;
    //o.xz *= r1;
    //D.xy *= r2;
    //o.xy *= r2;
	
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
    O = .0085 * O + float4(y, y, y, 1);
    O = lerp(baseC, O, clamp(((baseC.r - uExchange) / uLerp), 0, 1));
    
    return float4(O.xyz, sampleColor.a* O.a);
}

technique Technique1
{
    pass StarsTrailPass
    {
        //VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};