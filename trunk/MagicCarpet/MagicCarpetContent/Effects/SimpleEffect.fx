#include "Common.fx"



// TODO: add effect parameters here.

struct VertexShaderInput
{
	/*
	float4 pos : POSITION0;
    float3 normal : NORMAL;
    float4 uv : TEXCOORD0;
	*/
	float4 pos : SV_Position;
    float3 normal   : NORMAL;
	float3 tangent	: Tangent0;
    float2 uv : TEXCOORD0;

};

struct VertexShaderOutput
{
    float4 pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD1;
	float3 normal : TEXCOORD2;
	float3x3 tangent: Tangent0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.pos, WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);
	output.pos3d = input.pos;
	output.normal = mul(input.normal,WorldMatrix);
	output.tangent[0] = mul(input.tangent,WorldMatrix);
	output.tangent[1] = mul(cross(input.tangent,input.normal),WorldMatrix);
	output.tangent[2] = output.normal;

	output.uv = input.uv;
    return output;
}
// Helpfull examples at : http://rbwhitaker.wikidot.com/bump-map-shader-2  

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(TextureSampler, input.uv);

	result.rgb = AssignOwnerColour(result.rgb);


	float3 bump = BumpConstant * (tex2D(NormalMapSampler, input.uv) - (0.5, 0.5, 0.5));
    float3 bumpNormal = input.normal + (bump.x * input.tangent[0] + bump.y * input.tangent[1]);

	float diffuseIntensity = dot(normalize(LightDirection), bumpNormal);
    if(diffuseIntensity < 0)
        diffuseIntensity = 0;

	float3 light = normalize(LightDirection);
    float3 r = normalize(2 * dot(light, bumpNormal) * bumpNormal - light);
    float3 v = normalize(mul(normalize(ViewMatrix), WorldMatrix));
    float dotResult = dot(r, v);

	float3 specular = SpecularLightIntensity * SpecularLightColor * max(pow(dotResult, Shininess), 0) * diffuseIntensity;

	result.rgb = saturate(result.rgb * (diffuseIntensity) + AmbientLightColor * AmbientLightIntensity + specular);


	// Get value in the range of -1 to 1
	//float3 normalFromMap = BumpConstant * (tex2D(NormalMapSampler, input.uv) - (0.5, 0.5, 0.5));
	//float3 normalFromMap = normalize(2.0f * tex2D(NormalMapSampler,input.uv) - 1.0f);
	//normalFromMap = mul(normalFromMap,input.tangent);	

	//normalFromMap *= input.normal;
	//normalFromMap.normalize();

	//float dotResult = dot(-LightDirection, normalFromMap);    
	//dotResult = saturate(dotResult);

	//float3 directionalComponent = DirectionalLightColor * DirectionalLightIntensity * dotResult;
	//float4 light = float4(directionalComponent + (AmbientLightColor * AmbientLightIntensity),1);


	//float3 specular = SpecularLightIntensity * SpecularLightColor * max(pow(dotResult, Shininess), 0) * diffuseIntensity;

	//result.rgb = saturate(result.rgb * (diffuseIntensity) + AmbientLightColor * AmbientLightIntensity + specular);
	//result *= light;

	//float fogFactor = ComputeFogFactor(input.pos3d);
	//result.rgb = lerp(result.rgb,FogColor,fogFactor);

	//result = float4(1,1,0,1);


    return result;
}

technique SimpleTechnique
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
