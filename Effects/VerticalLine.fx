sampler uImage0 : register(s0);
matrix transformMatrix;
float uTime;

float Random(float2 st)
{
    float a = sin(dot(st, float2(12.9898f, 78.233f))) * 43758.5453123f;
    int a2 = (int) a;
    return a2 - a;
}

// Value noise by Inigo Quilez - iq/2013
// https://www.shadertoy.com/view/lsf3WH
float Noise(float2 st)
{
    float2 i = float2(floor(st.x), floor(st.y));
    float2 f = float2(st.x - i.x, st.y - i.y);
    float2 u = f * f * (float2(3f) - 2f * f);

    float a = Random(i);
    float b = Random(i + float2(1f, 0f));
    float c = Random(i + float2(0f, 1f));
    float d = Random(i + float2(1f, 1f));

    float x1 = lerp(a, b, u.x);
    float x2 = lerp(c, d, u.x);

    return lerp(x1, x2, u.y);
}

        // 2D旋转矩阵
Matrix Rotate2D(float2 pos, float angle)
{
    float cosTheta = cos(angle);
    float sinTheta = sin(angle);
    return
    Matrix(
                cosTheta, -sinTheta, 0f, 0f,
                sinTheta, cosTheta, 0f, 0f,
                0f, 0f, 1f, 0f,
                0f, 0f, 0f, 1f
            );
}

float SmoothStep(float edge0, float edge1, float x)
{
    float t = clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
    return t * t * (3f - 2f * t);
}

float Lines(float2 pos, float b)
{
    float scale = 10f;
    pos *= scale;
    return SmoothStep(0f, 0.5f + b * 0.5f, abs(sin(pos.x * 3.1415926) + b * 2f) * 0.5f);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 p1 = coords - float2(0.5);
    float2 r = float2(length(p1), atan2(p1.y, p1.x)/3.141);//极坐标变换
    float2 pos = r * float2(7f, 8f);

    float noiseValue = Noise(pos);
    matrix m = Rotate2D(pos, noiseValue);
    pos = float2((pos.x * m._11) + (pos.y * m._21) + m._41, (pos.x * m._12) + (pos.y * m._22) + m._42);
    
    float v = Lines(pos, 0.5);
    float4 tc = tex2D(uImage0, coords);
    float4 c = lerp(tc, sampleColor, v);
    c.a = 1;
    
    return c * tc.a;
}

technique Technique1
{
    pass StarsTrailPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
