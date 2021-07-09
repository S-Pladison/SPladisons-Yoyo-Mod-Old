matrix transformMatrix;

texture texture0 : register(s0);
sampler textureSampler0 = sampler_state
{
    texture = <texture0>;
};

struct VertexShaderInput
{
    float2 coord : TEXCOORD0;
    float4 color : COLOR0;
    float4 position : POSITION0;
};

struct VertexShaderOutput
{
    float2 coord : TEXCOORD0;
    float4 color : COLOR0;
    float4 position : SV_POSITION;
};

VertexShaderOutput MainVertexShader(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.coord = input.coord;
    output.color = input.color;
    output.position = mul(input.position, transformMatrix);
    return output;
}

float4 Primitive(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(textureSampler0, input.coord);
    color.a = (color.r + color.g + color.b) / 3.0;
    return color * input.color;
}

technique Technique1
{
    pass PrimitiveTrail
    {
        PixelShader = compile ps_2_0 Primitive();
        VertexShader = compile vs_2_0 MainVertexShader();
    }
}