#include "Common.fx"

uniform float ClothMovementOffset;
uniform float Amplitude;
uniform float Frequency;
uniform float ClothLength;
uniform float3 OwnerColor;

texture ClothTexture;

struct ClothVertexShaderInput
{
    float3 pos  : POSITION;
    float2 uv	: TEXCOORD0;  // coordinates for normal-map lookup
};


struct ClothVertexShaderOutput
{
    float4 pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD1;
	float3 normal : TEXCOORD2;
};


uniform sampler ClothSampler = sampler_state
{
    Texture   = (ClothTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


// Vertex shader for rendering the geometry clipmap
ClothVertexShaderOutput ClothVertexShaderFunction(ClothVertexShaderInput input)
{
	ClothVertexShaderOutput output;

	float waveLength = ClothLength / Frequency;

	float angle = (input.pos.z + ClothMovementOffset) / waveLength;
	angle = angle * 2 * PI;

	float cosAngle = cos(angle);
	float sinAngle = sin(angle);
	
	float3 normal = float3( sinAngle, 0,cosAngle);

	normal = float3(0,sinAngle,cosAngle);
	normal = normalize(normal);

	float height = sinAngle * Amplitude;
	//float height = sin(temp) * Amplitude;

	float4 worldPosition = mul(float4(input.pos.x, height,input.pos.z, 1), WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);

	output.normal = mul(normal,WorldMatrix);
    output.uv = input.uv;
	output.pos3d = output.pos;

    return output;

}


float4 ClothPixelShaderFunction(ClothVertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(ClothSampler, input.uv);
	
	// translate color to owners color.

	if(result.r == UnassignedPlayerColor.r && result.g == UnassignedPlayerColor.g && result.b == UnassignedPlayerColor.b)
	{
		result.rgb = OwnerColor.rgb;
	}

	/*
	if(result.rgb == float3(0,0,0))
	{
		result.rgb = OwnerColor;

	}
	*/

	float distanceFromCamera = length(input.pos3d - CameraPosition);


	// lighting
	float3 lightDir = LightDirection;
    
	float4 smoothedNormal = float4(input.normal,0);
    float dotResult = dot(-lightDir, smoothedNormal.xyz);    
	dotResult = saturate(dotResult);

	float3 directionalComponent = DirectionalLight * dotResult;
	float4 light = float4(directionalComponent + AmbientLight,1);

	//result *= light;

	// alpha blend Cloth when it's close to camera?
	//result.a = lerp(result.a,2,5);

	//result.rgb = float3(1,0,0);
	//result.a = 0.0;
    return result;
}


technique DrawCloth
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 ClothVertexShaderFunction();
        PixelShader = compile ps_3_0 ClothPixelShaderFunction();
    }
}
