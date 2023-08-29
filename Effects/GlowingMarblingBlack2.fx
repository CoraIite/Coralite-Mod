//float3 uColor;
//float viewAlpha;
sampler uImage0 : register(s0);

float uTime;
float viewRange;
float3 uC1;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = (2.0 * coords - 1) /*/ min(iResolution.x, iResolution.y)*/;
    float i = length(uv);
    
    //uv.x += 0.6 / cos(2.5 * uv.y + uTime) + (0.6 / 2 * cos(5 * uv.y + uTime));
    //uv.y += 0.6 / cos(1.5 * uv.x + uTime) + (0.6 / 2 * cos(3 * uv.x + uTime));
    for (float j = 1.0; j < 10.0; j++) //计算神奇的扭曲图片
    {
        uv.x += 0.6 / j * cos(j * 2.5 * uv.y + uTime);
        uv.y += 0.6 / j * cos(j * 1.5 * uv.x + uTime);
    }
    
    float4 c = float4(float3(0.1, 0.1, 0.1) / abs(sin(uTime - uv.y - uv.x)), 1.0);
    
    float s = viewRange;  //主要用于控制视野范围的东西，最好从1-0，为1时视野最大（虽说也无法覆盖屏幕）
    float d = 0.2; //也是控制中心大小的，只不过增加的时候视野减小
    float k0 = 2.5; //外边缘大小
    i = d + (i - s) * k0;
    
    c = 1.0 - float4(i, i, i, 1.0) + c; //将神奇的扭曲图片和椭圆形范围相叠加
    
    float final = c.r > 1.0 ? 1.0 : c.r; //把上面那个东西限制一下，不然超过1一乘的话颜色就变白了
    float4 texC = tex2D(uImage0, float2(uTime + coords.x, coords.y)); //读取图片
    return float4(texC.rgb * uC1 * final, 1.0); //相乘并得到结果
}

technique Technique1
{
    pass Marbling
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}