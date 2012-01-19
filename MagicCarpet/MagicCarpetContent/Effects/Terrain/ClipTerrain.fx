texture HeightMapTexture;
texture BaseTexture;
texture NoiseTexture;

uniform matrix WorldViewProjMatrix;
uniform float3 CameraPosition;

uniform float  ZScaleFactor;
uniform float4 ScaleFactor;
uniform float4 FineTextureBlockOrigin;
uniform float2 AlphaOffset;
uniform float2 ViewerPos;
uniform float  OneOverWidth;
uniform float3 LightPosition;
uniform float3 AmbientLight;
uniform float3 DirectionalLight;



float4 BlockColor;
float OneOverMaxExtents;
float2 TerrainTextureWindow;

bool FogEnabled;
float FogStart;
float FogEnd;
float3 FogColor = float3(0,0.7,0.4);


// Normal calc stuff based on example at
// http://www.catalinzima.com/tutorials/4-uses-of-vtf/terrain-morphing/


float normalStrength = 8.0f;

struct VertexShaderInput
{
	float2 gridPos: TEXCOORD0;
};


struct VertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float2 noiseuv : TEXCOORD1;      // coordinates for noise lookup
	float3 normal : TEXCOORD2;
	float3 pos3d : TEXCOORD3;

};

uniform sampler ElevationSampler = sampler_state
{
    Texture   = (HeightMapTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

uniform sampler BaseSampler = sampler_state
{
    Texture   = (BaseTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

uniform sampler NoiseSampler = sampler_state
{
    Texture   = (NoiseTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Wrap;
    AddressV  = Wrap;
};


// Vertex shader for rendering the geometry clipmap
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
    // convert from grid xy to world xy coordinates
    //  ScaleFactor.xy: grid spacing of current level
    //  ScaleFactor.zw: origin of current block within world
	float2 worldPos = input.gridPos+FineTextureBlockOrigin.zw;
                     
    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture
    //  FineBlockOrig.zw: origin of block in texture           
    float2 uv = float2((worldPos)*FineTextureBlockOrigin.xy);
    float height = tex2Dlod(ElevationSampler, float4(uv, 0, 1));

	// need to sample heights at integer values of worldpos x& y then lerp to find average height?

    float2 uv1 = float2((worldPos.x+ScaleFactor.x)*FineTextureBlockOrigin.x,worldPos.y * FineTextureBlockOrigin.y);
    float2 uv2 = float2((worldPos.x)*FineTextureBlockOrigin.x,(worldPos.y+ScaleFactor.y) * FineTextureBlockOrigin.y);

    float heightxplus1 = tex2Dlod(ElevationSampler, float4(uv1, 0, 1));
	float heightyplus1 = tex2Dlod(ElevationSampler, float4(uv2, 0, 1));

//	float3 c0 = float3(worldPos.x,height,worldPos.y);
//	float3 c1 = float3(worldPos.x+ScaleFactor.x,heightxplus1,worldPos.y);
//	float3 c2 = float3(worldPos.x,heightyplus1,worldPos.y+ScaleFactor.x);

//	output.normal = normalize(cross((c2-c0), (c1-c0))).xyz;
	
	//output.normal = float3(0,1,0);
    output.pos = mul(float4(worldPos.x, height,worldPos.y, 1), WorldViewProjMatrix);
	//output.normal = normalize(mul(cross((c2-c0), (c1-c0)),(float3x3)WorldViewProjMatrix)).xyz;
    output.uv = uv;
	output.noiseuv = float2(worldPos.x/10.0,worldPos.y/10.0);
	output.pos3d = output.pos;
    return output;

}

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


float4 ComputeNormalsPS(in float2 uv:TEXCOORD0)
{
    float tl = abs(tex2D (ElevationSampler, uv + texelSize * float2(-1, -1)).x);   // top left
    float  l = abs(tex2D (ElevationSampler, uv + texelSize * float2(-1,  0)).x);   // left
    float bl = abs(tex2D (ElevationSampler, uv + texelSize * float2(-1,  1)).x);   // bottom left
    float  t = abs(tex2D (ElevationSampler, uv + texelSize * float2( 0, -1)).x);   // top
    float  b = abs(tex2D (ElevationSampler, uv + texelSize * float2( 0,  1)).x);   // bottom
    float tr = abs(tex2D (ElevationSampler, uv + texelSize * float2( 1, -1)).x);   // top right
    float  r = abs(tex2D (ElevationSampler, uv + texelSize * float2( 1,  0)).x);   // right
    float br = abs(tex2D (ElevationSampler, uv + texelSize * float2( 1,  1)).x);   // bottom right

    // Compute dx using Sobel:
    //           -1 0 1 
    //           -2 0 2
    //           -1 0 1
    float dX = tr + 2*r + br -tl - 2*l - bl;

    // Compute dy using Sobel:
    //           -1 -2 -1 
    //            0  0  0
    //            1  2  1
    float dY = bl + 2*b + br -tl - 2*t - tr;

    // Compute cross-product and renormalize
    float4 N = float4(normalize(float3(dX, 1.0f / normalStrength, dY)), 1.0f);
    //convert (-1.0 , 1.0) to (0.0 , 1.0);
    return N * 0.5f + 0.5f;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

    float3 lightDir = normalize(input.pos3d - float4(LightPosition,0)).xyz;
    float4 smoothedNormal = ComputeNormalPS(input.uv);
    
    
    float dotResult = dot(-lightDir, smoothedNormal.xyz);    
	float projection = saturate(dotResult);
	float3 directionalComponent = DirectionalLight * projection;
	//float4 light = (AmbientLight + directionalComponent,1);
	float4 light = float4(directionalComponent.x,directionalComponent.y,directionalComponent.z,1);
	//light *= 0.2;
	float4 result = tex2Dlod(BaseSampler, float4(input.uv, 0, 1));
	float4 noise = tex2Dlod(NoiseSampler, float4(input.noiseuv,0,1));

	result = result +(noise * 0.3);


	result *= light;
	
	// Fog stuff.
	float distanceFromCamera = length(input.pos3d - CameraPosition);
	float fogFactor = ComputeFogFactor(distanceFromCamera);
	result.rgb = lerp(result.rgb,FogColor,fogFactor);

	return result;

}





technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
