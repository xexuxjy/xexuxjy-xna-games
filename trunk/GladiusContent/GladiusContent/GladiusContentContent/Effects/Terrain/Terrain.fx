float4x4 World;
float4x4 View;
float4x4 Projection;

texture BaseTexture;
texture NoiseTexture;
texture NormalMapTexture;

uniform float3 CameraPosition;

uniform float3 LightPosition;
uniform float3 LightDirection;
uniform float3 AmbientLightColor;
uniform float AmbientLightIntensity;
uniform float3 PointLightColor;
uniform float PointLightIntensity;
uniform float3 PointLightPosition;
uniform float PointLightRadius;

uniform float3 SpecularLightColor;
uniform float SpecularLightIntensity;

uniform float Shininess = 200;


struct VertexShaderInput
{
	  vector pos      : POSITION;   
	  vector normal   : POSITION1;   
	  float2 uv      : TEXCOORD0;   
};

struct VertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float2 noiseuv : TEXCOORD1;      // coordinates for noise lookup
	float3 normal : TEXCOORD2;
	float3 pos3d : TEXCOORD3;

};

uniform sampler BaseSampler = sampler_state
{
    Texture   = <BaseTexture>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

uniform sampler NoiseSampler = sampler_state
{
    Texture   = <NoiseTexture>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};


uniform sampler NormalMapSamplerPS = sampler_state
{
    Texture   = <NormalMapTexture>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.pos, World);
    float4 viewPosition = mul(worldPosition, View);
    output.pos = mul(viewPosition, Projection);

	output.normal = float3(0,1,0);
    output.uv = input.uv;
	output.noiseuv = float2(worldPosition.x/10.0,worldPosition.y/10.0);
	output.pos3d = worldPosition.xyz;

    return output;
}



float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// base texture and noise
	float4 result = tex2D(BaseSampler, float4(input.uv, 0, 1));
	//float4 noise = tex2D(NoiseSampler, float4(input.noiseuv,0,1));
	//result = result +(noise * 0.3);

	// lighting
	float3 lightDir = normalize(input.pos3d - PointLightPosition);
    

	//float4 smoothedNormal = normalize( 2.0f * (tex2D(NormalMapSamplerPS,input.uv) - 0.5f));
	float4 smoothedNormal = tex2D(NormalMapSamplerPS,input.uv);

    float dotResult = dot(-lightDir, smoothedNormal.xyz);    
	dotResult = saturate(dotResult);

	float pointDist = length(input.pos3d -PointLightPosition);
	float pointContribution = saturate(1 - (pointDist/PointLightRadius));
	pointContribution *= dotResult;
	float3 pointComponent = PointLightColor * pointContribution;

	float4 light = float4(pointComponent + (AmbientLightColor * AmbientLightIntensity),1);

	result *= light;
	

	result.a = 1;

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
