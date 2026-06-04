sampler uImage0 : register(s0);

texture uFlowTex;
float uTime;
float addCount;
float addCount2;
float yScale2;
float2 uResolution;

sampler2D flowTex = sampler_state
{
    texture = <uFlowTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

float rayStrength(float2 raySource, float2 rayRefDirection, float2 coord, float seedA, float seedB, float speed)
{
    float2 sourceToCoord = coord - raySource;
    float cosAngle = dot(normalize(sourceToCoord), rayRefDirection);
	
    return clamp(
		(0.45 + 0.15 * sin(cosAngle * seedA + uTime * speed)) +
		(0.3 + 0.2 * cos(-cosAngle * seedB + uTime * speed)),
		0.0, 1.0) *
		clamp((uResolution.x - length(sourceToCoord)) / uResolution.x, 0.5, 1.0);
}

float4 mainImage( float2 fragCoord)
{
    float2 uv = fragCoord.xy / uResolution.xy;
    uv.y = 1.0 - uv.y;
    float2 coord = float2(fragCoord.x, uResolution.y - fragCoord.y);
	
	
	// Set the parameters of the sun rays
    float2 rayPos1 = float2(uResolution.x * 0.7, uResolution.y * -0.4);
    float2 rayRefDir1 = normalize(float2(1.0, -0.116));
    float raySeedA1 = 36.2214;
    float raySeedB1 = 21.11349;
    float raySpeed1 = 1.5;
	
    float2 rayPos2 = float2(uResolution.x * 0.8, uResolution.y * -0.6);
    float2 rayRefDir2 = normalize(float2(1.0, 0.241));
    const float raySeedA2 = 22.39910;
    const float raySeedB2 = 18.0234;
    const float raySpeed2 = 1.1;
	
	// Calculate the colour of the sun rays on the current fragment
    float4 rays1 =
		float4(1.0, 1.0, 1.0, 1.0) *
		rayStrength(rayPos1, rayRefDir1, coord, raySeedA1, raySeedB1, raySpeed1);
	 
    float4 rays2 =
		float4(1.0, 1.0, 1.0, 1.0) *
		rayStrength(rayPos2, rayRefDir2, coord, raySeedA2, raySeedB2, raySpeed2);
	
    float4 fragColor = rays1 * 0.5 + rays2 * 0.4;
	
	// Attenuate brightness towards the bottom, simulating light-loss due to depth.
	// Give the whole thing a blue-green tinge as well.
    float brightness = 1.0 - (coord.y / uResolution.y);
    fragColor.x *= 0.1 + (brightness * 0.8);
    fragColor.y *= 0.3 + (brightness * 0.6);
    fragColor.z *= 0.5 + (brightness * 0.5);
    
    return fragColor;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords) * color;
    
    float f = sin(uTime) * 0.15f + uTime * 0.1f; //u的偏移量
    float2 newUV = float2(coords.x + f, coords.y * yScale2);
    
    float4 addColor1 = tex2D(flowTex, newUV);
    f = cos(uTime) * 0.17f + uTime * 0.12f;
    newUV.x = coords.x + f;
    newUV.y = coords.y * yScale2 * 2 + 0.5f;
    float4 addColor2 = tex2D(flowTex, newUV);
    float f2 = 1 - coords.y;
    
    return (baseColor + (addColor1 * addCount + addColor2 * addCount + mainImage(float2(coords.x, 1 - coords.y) * uResolution) * addCount2) * baseColor.a) * f2;
}

technique Technique1
{
    pass WaterFlowPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}