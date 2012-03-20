uniform matrix ViewMatrix;
uniform matrix ProjMatrix;
uniform matrix WorldMatrix;
uniform matrix WorldViewProjMatrix;


uniform float3 CameraPosition;

uniform float3 LightPosition;
uniform float3 LightDirection;
uniform float3 AmbientLight;
uniform float3 DirectionalLight;

bool FogEnabled;
float FogStart;
float FogEnd;
float3 FogColor = float3(0,0.7,0.4);


static const float PI = 3.14159265f;

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

float ComputeFogFactor(float d)
{
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
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