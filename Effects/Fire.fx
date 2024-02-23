//
// Description : Array and textureless GLSL 2D/3D/4D simplex 
//							 noise functions.
//			Author : Ian McEwan, Ashima Arts.
//	Maintainer : ijm
//		 Lastmod : 20110822 (ijm)
//		 License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//							 Distributed under the MIT License. See LICENSE file.
//							 https://github.com/ashima/webgl-noise
// 

float3 mod289(float3 x)
{
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 mod289(float4 x)
{
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 permute(float4 x)
{
    return mod289(((x * 34.0) + 1.0) * x);
}

float4 taylorInvSqrt(float4 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}

float snoise(float3 v)
{
    const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);
    const float4 D = float4(0.0, 0.5, 1.0, 2.0);

// First corner
    float3 i = floor(v + dot(v, C.yyy));
    float3 x0 = v - i + dot(i, C.xxx);

// Other corners
    float3 g = step(x0.yzx, x0.xyz);
    float3 l = 1.0 - g;
    float3 i1 = min(g.xyz, l.zxy);
    float3 i2 = max(g.xyz, l.zxy);

	//	 x0 = x0 - 0.0 + 0.0 * C.xxx;
	//	 x1 = x0 - i1	+ 1.0 * C.xxx;
	//	 x2 = x0 - i2	+ 2.0 * C.xxx;
	//	 x3 = x0 - 1.0 + 3.0 * C.xxx;
    float3 x1 = x0 - i1 + C.xxx;
    float3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
    float3 x3 = x0 - D.yyy; // -1.0+3.0*C.x = -0.5 = -D.y

// Permutations
    i = mod289(i);
    float4 p = permute(permute(permute(
						 i.z + float4(0.0, i1.z, i2.z, 1.0))
					 + i.y + float4(0.0, i1.y, i2.y, 1.0))
					 + i.x + float4(0.0, i1.x, i2.x, 1.0));

// Gradients: 7x7 points over a square, mapped onto an octahedron.
// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
    float n_ = 0.142857142857; // 1.0/7.0
    float3 ns = n_ * D.wyz - D.xzx;

    float4 j = p - 49.0 * floor(p * ns.z * ns.z); //	mod(p,7*7)

    float4 x_ = floor(j * ns.z);
    float4 y_ = floor(j - 7.0 * x_); // mod(j,N)

    float4 x = x_ * ns.x + ns.yyyy;
    float4 y = y_ * ns.x + ns.yyyy;
    float4 h = 1.0 - abs(x) - abs(y);

    float4 b0 = float4(x.xy, y.xy);
    float4 b1 = float4(x.zw, y.zw);

	//float4 s0 = float4(lessThan(b0,0.0))*2.0 - 1.0;
	//float4 s1 = float4(lessThan(b1,0.0))*2.0 - 1.0;
    float4 s0 = floor(b0) * 2.0 + 1.0;
    float4 s1 = floor(b1) * 2.0 + 1.0;
    float4 sh = -step(h, float4(0.0));

    float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
    float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

    float3 p0 = float3(a0.xy, h.x);
    float3 p1 = float3(a0.zw, h.y);
    float3 p2 = float3(a1.xy, h.z);
    float3 p3 = float3(a1.zw, h.w);

//Normalise gradients
	//float4 norm = taylorInvSqrt(float4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
    float4 norm = inversesqrt(float4(dot(p0, p0), dot(p1, p1), dot(p2, p2), dot(p3, p3)));
    p0 *= norm.x;
    p1 *= norm.y;
    p2 *= norm.z;
    p3 *= norm.w;

// Mix final noise value
    float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
    m = m * m;
    return 42.0 * dot(m * m, float4(dot(p0, x0), dot(p1, x1),
																dot(p2, x2), dot(p3, x3)));
}

//////////////////////////////////////////////////////////////

// PRNG
// From https://www.shadertoy.com/view/4djSRW
float prng(in float2 seed)
{
    seed = fract(seed * float2(5.3983, 5.4427));
    seed += dot(seed.yx, seed.xy + float2(21.5351, 14.3137));
    return fract(seed.x * seed.y * 95.4337);
}

//////////////////////////////////////////////////////////////

float PI = 3.1415926535897932384626433832795;

float noiseStack(float3 pos, int octaves, float falloff)
{
    float noise = snoise(float3(pos));
    float off = 1.0;
    if (octaves > 1)
    {
        pos *= 2.0;
        off *= falloff;
        noise = (1.0 - off) * noise + off * snoise(float3(pos));
    }
    if (octaves > 2)
    {
        pos *= 2.0;
        off *= falloff;
        noise = (1.0 - off) * noise + off * snoise(float3(pos));
    }
    if (octaves > 3)
    {
        pos *= 2.0;
        off *= falloff;
        noise = (1.0 - off) * noise + off * snoise(float3(pos));
    }
    return (1.0 + noise) / 2.0;
}

float2 noiseStackUV(float3 pos, int octaves, float falloff, float diff)
{
    float displaceA = noiseStack(pos, octaves, falloff);
    float displaceB = noiseStack(pos + float3(3984.293, 423.21, 5235.19), octaves, falloff);
    return float2(displaceA, displaceB);
}

void mainImage(out float4 fragColor, in float2 fragCoord)
{
    float time = iTime;
    float2 resolution = iResolution.xy;

		//
    float xpart = fragCoord.x / resolution.x;
    float ypart = fragCoord.y / resolution.y;
	//
    float clip = 210.0;
    float ypartClip = fragCoord.y / clip;
    float ypartClippedFalloff = clamp(2.0 - ypartClip, 0.0, 1.0);
    float ypartClipped = min(ypartClip, 1.0);
    float ypartClippedn = 1.0 - ypartClipped;
	//
    float xfuel = 1.0 - abs(2.0 * xpart - 1.0); //pow(1.0-abs(2.0*xpart-1.0),0.5);
	//
    float timeSpeed = 0.5;
    float realTime = timeSpeed * time;
	//
    float2 coordScaled = 0.01 * fragCoord;
    float3 position = float3(coordScaled, 0.0);
    float3 flow = float3(4.1 * (0.5 - xpart) * pow(ypartClippedn, 4.0), -2.0 * xfuel * pow(ypartClippedn, 64.0), 0.0);
    float3 timing = realTime * float3(0.0, -1.7, 1.1) + flow;
	//
    float3 displacePos = float3(1.0, 0.5, 1.0) * 2.4 * position + realTime * float3(0.01, -0.7, 1.3);
    float3 displace3 = float3(noiseStackUV(displacePos, 2, 0.4, 0.1), 0.0);
	//
    float3 noiseCoord = (float3(2.0, 1.0, 1.0) * position + timing + 0.4 * displace3) / 1.0;
    float noise = noiseStack(noiseCoord, 3, 0.4);
	//
    float flames = pow(ypartClipped, 0.3 * xfuel) * pow(noise, 0.3 * xfuel);
	//
    float f = ypartClippedFalloff * pow(1.0 - flames * flames * flames, 4.0);
    float fff = f * f * f;
    float3 fire = 1.5 * float3(fff, fff, f);
	//
	// smoke
    float smokeNoise = 0.5 + snoise(0.4 * position + timing * float3(1.0, 1.0, 0.2)) / 2.0;
    float3 smoke = float3(0.3 * pow(xfuel, 3.0) * pow(ypart, 2.0) * (smokeNoise + 0.4 * (1.0 - noise)));
	//
	// sparks
    float sparkGridSize = 30.0;
    float2 sparkCoord = fragCoord - float2(2.0, 190.0 * realTime);
    sparkCoord -= 30.0 * noiseStackUV(0.01 * float3(sparkCoord, 30.0 * time), 1, 0.4, 0.1);
    sparkCoord += 100.0 * flow.xy;
    if (mod(sparkCoord.y / sparkGridSize, 2.0) < 1.0)
        sparkCoord.x += 0.5 * sparkGridSize;
    float2 sparkGridIndex = float2(floor(sparkCoord / sparkGridSize));
    float sparkRandom = prng(sparkGridIndex);
    float sparkLife = min(10.0 * (1.0 - min((sparkGridIndex.y + (190.0 * realTime / sparkGridSize)) / (24.0 - 20.0 * sparkRandom), 1.0)), 1.0);
    float3 sparks = float3(0.0);
    if (sparkLife > 0.0)
    {
        float sparkSize = xfuel * xfuel * sparkRandom * 0.08;
        float sparkRadians = 999.0 * sparkRandom * 2.0 * PI + 2.0 * time;
        float2 sparkCircular = float2(sin(sparkRadians), cos(sparkRadians));
        float2 sparkOffset = (0.5 - sparkSize) * sparkGridSize * sparkCircular;
        float2 sparkModulus = mod(sparkCoord + sparkOffset, sparkGridSize) - 0.5 * float2(sparkGridSize);
        float sparkLength = length(sparkModulus);
        float sparksGray = max(0.0, 1.0 - sparkLength / (sparkSize * sparkGridSize));
        sparks = sparkLife * sparksGray * float3(1.0, 0.3, 0.0);
    }
	//
    fragColor = float4(max(fire, sparks) + smoke, 1.0);
}