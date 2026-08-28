// XNA Effect File (Shader Model 3.0)
#define PI 3.1415926

// 外部参数（由 XNA 程序设置）
float Time; // 对应 GLSL 的 iTime
float alpha; // 对应 GLSL 的 iTime
float2 Resolution; // 对应 GLSL 的 iResolution（屏幕分辨率）
float2 offset;
// 随机函数（与 GLSL 版本一致）
float rand(float t)
{
    return frac(sin(dot(float2(t, t), float2(12.9898, 78.233))) * 43758.5453);
}

// 顶点输入结构（通常来自 SpriteBatch 或自定义全屏四边形）
//struct VertexInput
//{
//    float4 Position : POSITION0;
//    float2 TexCoord : TEXCOORD0;
//};

// 顶点输出结构
//struct VertexOutput
//{
//    float4 Position : POSITION0;
//    float2 TexCoord : TEXCOORD0;
//};

// 顶点着色器：直接传递位置和纹理坐标
//VertexOutput vs_main(VertexInput input)
//{
//    VertexOutput output;
//    output.Position = input.Position;
//    output.TexCoord = input.TexCoord;
//    return output;
//}

// 像素着色器：对应 GLSL 的 mainImage
float4 ps_main(float2 position : POSITIONT, float2 TexCoord:TEXCOORD) : COLOR0
{
    // 重建像素坐标（fragCoord）
    // 注意：XNA 纹理坐标原点在左上角，而 GLSL 的 fragCoord 原点在左下角，
    // 可能导致图像垂直翻转。若需要保持 GLSL 的朝向，请取消下面一行的注释：
    // input.TexCoord.y = 1.0 - input.TexCoord.y;
    float2 fragCoord = TexCoord * Resolution;

    // 坐标映射
    float2 uv = (fragCoord.xy * 2.0 - Resolution.xy) / Resolution.y;
    float2 uv1 = uv + offset;
    float r = length(uv1) * 111.0;

    float t = ceil(r);
    float a = frac(atan2(uv1.y, uv1.x) / PI + Time * rand(t) * 0.1 + t * 0.1);

    float ang = rand(t);
    for (float i = 0.0; i < 2.0; i++)
    {
        ang = smoothstep(ang, rand(t + i), 0.5);
    }
    float c = smoothstep(ang, ang - 1.5, a * 5.0);

    float3 col = float3(0.3, 0.3, 0.5) * 3.0;
    float rr = length(uv - float2(0.6, 1.4)) - 0.8;
    float3 coll = rr * float3(0.15, 0.08, 0.24) * alpha;

    coll = lerp(coll, col * rand(t), c * step(0.1, r / 111.0));

    return float4(coll, 1.0);
}

// 技术定义
technique Main
{
    pass Pass0
    {
        //VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}