float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha;
uniform float3 BaseColour;
uniform float3 TeamColour;
texture Texture;

uniform sampler TextureSampler = sampler_state
{
    Texture   = (Texture);
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
};

float4 AssignOwnerColour(float4 input)
{
	float4 result = input;
	result.a = min(result.a,Alpha);
	if(input.r == BaseColour.r && input.g == BaseColour.g && input.b == BaseColour.b)
	{
		result.rgb = TeamColour.rgb;
	}

	return result;
	//return  TeamColour.rgb;
}


struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.UV = input.UV;
    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(TextureSampler, input.UV);
	result = AssignOwnerColour(result);
	//result.a = Alpha;
    return result;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
