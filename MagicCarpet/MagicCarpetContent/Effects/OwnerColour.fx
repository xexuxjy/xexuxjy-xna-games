uniform matrix WorldViewProjMatrix;
uniform matrix ViewMatrix;
uniform matrix ProjMatrix;
uniform matrix WorldMatrix;

uniform float TimeStep;
uniform float Amplitude;
uniform float Frequency;

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
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD1;
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

	// take length 

	//float height = Amplitude * sin((input.pos.z + TimeStep) / Frequency);
	float height = input.pos.y;

    output.pos = mul(float4(input.pos.x, height,input.pos.z, 1), WorldViewProjMatrix);
    output.uv = input.uv;
	output.pos3d = output.pos;
    return output;

}


float4 CarpetPixelShaderFunction(CarpetVertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(CarpetSampler, input.uv);

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
