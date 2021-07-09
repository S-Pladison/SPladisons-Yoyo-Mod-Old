sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uShaderSpecificData;

float time;
float width;

float2 RotateUV(float2 uv, float rotation, float2 mid)
{
    return float2(
      cos(rotation) * (uv.x - mid.x) + sin(rotation) * (uv.y - mid.y) + mid.x,
      cos(rotation) * (uv.y - mid.y) - sin(rotation) * (uv.x - mid.x) + mid.y
    );
}

float4 Blackhole(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = RotateUV(floor(coords * width) / width, -time, float2(0.5f, 0.5f));
    
    float4 per = tex2D(uImage1, coords + time * 0.12f);
    uv += (per.r - 0.5f) * 0.05f;
    
    float4 color = tex2D(uImage0, uv);  
    color.a = color.r;
    
    return color * sampleColor;
}

technique Technique1
{
    pass Blackhole
    {
        PixelShader = compile ps_2_0 Blackhole();
    }
}