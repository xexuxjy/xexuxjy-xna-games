uniform matrix WorldViewProjMatrix;
uniform matrix ViewMatrix;
uniform matrix ProjMatrix;
uniform matrix WorldMatrix;

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
	float3 normal : NORMAL;   
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

	float height = sin(angle) * sin(angle) * Amplitude;
	//float height = sin(temp) * Amplitude;

	float4 worldPosition = mul(float4(input.pos.x, height,input.pos.z, 1), WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);

    output.uv = input.uv;
	output.pos3d = output.pos;

	 //(1,cos(x))/sqrt(1+(cos(x))^2) = (tx,ty). A unit-length normal is (ty,-tx). 



	output.normal = normal

    return output;

}


float4 CarpetPixelShaderFunction(CarpetVertexShaderOutput input) : COLOR0
{
	//float4 result = tex2D(CarpetSampler, input.uv);
	float4 result = float4(1,0,0,1);

	float distanceFromCamera = length(input.pos3d - CameraPosition);
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
