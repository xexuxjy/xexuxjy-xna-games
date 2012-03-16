uniform matrix WorldViewProjMatrix;
uniform matrix ViewMatrix;
uniform matrix ProjMatrix;
uniform matrix WorldMatrix;

uniform float3 LightPosition;
uniform float3 LightDirection;
uniform float3 AmbientLight;
uniform float3 DirectionalLight;


static const float PI = 3.14159265f;

uniform float CarpetMovementOffset;
uniform float Amplitude;
uniform float Frequency;
uniform float CarpetLength;

uniform vector CameraPosition;

texture CarpetTexture;

struct CarpetVertexShaderInput
{
    float3 pos  : POSITION;
    float2 uv	: TEXCOORD0;  // coordinates for normal-map lookup
};


struct CarpetVertexShaderOutput
{
    float4 pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD1;
	float3 normal : TEXCOORD2;
};


uniform sampler CarpetSampler = sampler_state
{
    Texture   = (CarpetTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


// Vertex shader for rendering the geometry clipmap
CarpetVertexShaderOutput CarpetVertexShaderFunction(CarpetVertexShaderInput input)
{
	CarpetVertexShaderOutput output;

	float waveLength = CarpetLength / Frequency;

	float angle = (input.pos.z + CarpetMovementOffset) / waveLength;
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


float4 CarpetPixelShaderFunction(CarpetVertexShaderOutput input) : COLOR0
{
	//float4 result = tex2D(CarpetSampler, input.uv);
	float4 result = float4(1,0,0,1);

	float distanceFromCamera = length(input.pos3d - CameraPosition);


	// lighting
	float3 lightDir = LightDirection;
    
	float4 smoothedNormal = float4(input.normal,0);
    float dotResult = dot(-lightDir, smoothedNormal.xyz);    
	dotResult = saturate(dotResult);

	float3 directionalComponent = DirectionalLight * dotResult;
	float4 light = float4(directionalComponent + AmbientLight,1);

	result *= light;


	//float fogFactor = ComputeFogFactor(distanceFromCamera);
	/*
	// do something funky as well to provide fog near the boundaries of the world.
	if( input.pos3d.x < EdgeFog || input.pos3d.x > WorldWidth - EdgeFog || input.pos3d.z < EdgeFog || input.pos3d.z > WorldWidth - EdgeFog)
	{
		fogFactor = 1.0;
	}
	*/
	//result.rgb = lerp(result.rgb,FogColor,fogFactor);


    return result;
}


technique DrawCarpet
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 CarpetVertexShaderFunction();
        PixelShader = compile ps_3_0 CarpetPixelShaderFunction();
    }
}
