uniform matrix ViewMatrix;
uniform matrix ProjMatrix;
uniform matrix WorldMatrix;


uniform float3 CameraPosition;

uniform float3 LightPosition;
uniform float3 LightDirection;
uniform float3 AmbientLight;
uniform float3 DirectionalLight;

uniform bool FogEnabled;
uniform float FogStart;
uniform float FogEnd;
uniform float3 FogColor = float3(0,0.7,0.4);
uniform float EdgeFog;

uniform float WorldWidth;

static const float PI = 3.14159265f;

texture Texture;


uniform float3 UnassignedPlayerColor;
uniform float3 OwnerColor;

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}

/*
where d is the length of the vector going from the camera to the vertex or pixel.

Then the computed fog factor is used to lerp between the normal color and the fog color:

color.rgb = lerp(color.rgb, FogColor, fogFactor);
*/


float ComputeFogFactor(float d,float3 worldpos)
{
    float result = clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
	float minFog = (-WorldWidth / 2) + EdgeFog;
	float maxFog = (WorldWidth / 2) - EdgeFog;
	// provide a foggy band around the edge of the world.
	if( worldpos.x < minFog || worldpos.x > maxFog || worldpos.z < minFog|| worldpos.z > maxFog)
	{
		result = 1.0f;
	}
	return result;

}

float3 AssignOwnerColour(float3 input)
{
	if(input.r == UnassignedPlayerColor.r && input.g == UnassignedPlayerColor.g && input.b == UnassignedPlayerColor.b)
	{
		return  OwnerColor.rgb;
	}
	return input;
}


technique CommonTechnique
{
/*
    pass
    {
        VertexShader = compile vs_1_0 CommonVertexShader();
        PixelShader = compile ps_1_0 CommonPixelShader();
    }
*/
}