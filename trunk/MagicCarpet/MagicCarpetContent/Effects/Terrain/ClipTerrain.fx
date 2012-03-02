texture HeightMapTexture;
texture BaseTexture;
texture NoiseTexture;
texture NormalMapTexture;
texture TreeTexture;

uniform matrix WorldViewProjMatrix;
uniform matrix ViewMatrix;
uniform matrix ProjMatrix;
uniform matrix WorldMatrix;

uniform float3 CameraPosition;

uniform float  ZScaleFactor;
uniform float WorldWidth;
uniform float EdgeFog;
uniform float4 ScaleFactor;
uniform float4 FineTextureBlockOrigin;
uniform float2 AlphaOffset;
uniform float2 ViewerPos;
uniform float  OneOverWidth;
uniform float3 LightPosition;
uniform float3 LightDirection;
uniform float3 AmbientLight;
uniform float3 DirectionalLight;
uniform float3 AllowedRotDir;

float4 BlockColor;
float OneOverMaxExtents;
float2 TerrainTextureWindow;

bool FogEnabled;
float FogStart;
float FogEnd;
float3 FogColor = float3(0,0.7,0.4);


// Normal calc stuff based on example at
// http://www.catalinzima.com/tutorials/4-uses-of-vtf/terrain-morphing/

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


struct TreeVertexShaderInput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
};

struct TreeVertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
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
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

uniform sampler NoiseSampler = sampler_state
{
    Texture   = (NoiseTexture);
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};


uniform sampler NormalMapSamplerPS = sampler_state
{
    Texture   = (NormalMapTexture);
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

uniform sampler TreeSampler = sampler_state
{
    Texture   = (TreeTexture);
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

float ComputeHeight(float2 uv:TEXCOORD0)
{
	//float c = abs(tex2D (ElevationSampler, uv));   // center
	float c = tex2Dlod(ElevationSampler, float4(uv, 0, 1));

    float tl = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2(-1, -1),0,1));   // top left
    float  l = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2(-1,  0),0,1));   // left
    float bl = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2(-1,  1),0,1));   // bottom left
    float  t = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2( 0, -1),0,1));   // top

	float  b = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2( 0,  1),0,1));   // bottom
    float tr = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2( 1, -1),0,1));   // top right
    float  r = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2( 1,  0),0,1));   // right
    float br = tex2Dlod (ElevationSampler, float4(uv + FineTextureBlockOrigin.x * float2( 1,  1),0,1));   // bottom right
	
	float numSamples = 9;
	float sum1 = c+tl+l+bl+t;
	float sum2 = b+tr+r+br;
	//float result = (float)((sum1+sum2) / numSamples);
	float result = c;
	
	return result;
}


// Vertex shader for rendering the geometry clipmap
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
    // convert from grid xy to world xy coordinates
    //  ScaleFactor.xy: grid spacing of current level
    //  ScaleFactor.zw: origin of current block within world
	float2 worldPos = input.gridPos+FineTextureBlockOrigin.zw;
                     
    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture (texelwidth)
    //  FineBlockOrig.zw: origin of block in texture           
    float2 uv = float2((worldPos)*FineTextureBlockOrigin.xy);
    float height = ComputeHeight(uv);

    output.pos = mul(float4(worldPos.x, height,worldPos.y, 1), WorldViewProjMatrix);
	output.normal = float3(0,1,0);
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


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// base texture and noise
	float4 result = tex2Dlod(BaseSampler, float4(input.uv, 0, 1));
	float4 noise = tex2Dlod(NoiseSampler, float4(input.noiseuv,0,1));
	result = result +(noise * 0.3);

	// lighting
	float3 lightDir = LightDirection;
    
	float4 smoothedNormal = normalize( 2.0f * (tex2D(NormalMapSamplerPS,input.uv) - 0.5f));
	//float4 smoothedNormal = ComputeNormalsPS(input.uv);
    float dotResult = dot(-lightDir, smoothedNormal.xyz);    
	dotResult = saturate(dotResult);

	float3 directionalComponent = DirectionalLight * dotResult;
	float4 light = float4(directionalComponent + AmbientLight,1);

	result *= light;
	
	// Fog stuff.
	float distanceFromCamera = length(input.pos3d - CameraPosition);
	float fogFactor = ComputeFogFactor(distanceFromCamera);

	// do something funky as well to provide fog near the boundaries of the world.
	if( input.pos3d.x < EdgeFog || input.pos3d.x > WorldWidth - EdgeFog || input.pos3d.z < EdgeFog || input.pos3d.z > WorldWidth - EdgeFog)
	{
		fogFactor = 1.0;
	}

	result.rgb = lerp(result.rgb,FogColor,fogFactor);


	return result;

}


// Vertex shader for rendering the geometry clipmap
TreeVertexShaderOutput TreeVertexShaderFunction(TreeVertexShaderInput input)
{
	TreeVertexShaderOutput output;
	float3 worldPos = input.pos;
                     
    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture (texelwidth)
    float2 uv = float2((worldPos)*FineTextureBlockOrigin.xy);
    float height = ComputeHeight(uv);

	worldPos = float3(worldPos.x, height,worldPos.z);

	float3 center = mul(worldPos, WorldMatrix);
    float3 eyeVector = center - CameraPosition;
    float3 upVector = AllowedRotDir;
    upVector = normalize(upVector);
    float3 sideVector = cross(eyeVector,upVector);
    sideVector = normalize(sideVector);
    float3 finalPosition = center;
    finalPosition += (input.uv.x-0.5f)*sideVector;
    finalPosition += (1.5f-input.uv.y*1.5f)*upVector;

    float4 finalPosition4 = float4(finalPosition, 1);
    float4x4 preViewProjection = mul (ViewMatrix, ProjMatrix);

//    output.pos = mul(float4(worldPos.x, height,worldPos.y, 1), WorldViewProjMatrix);
	output.pos = mul(finalPosition4, preViewProjection);
	output.pos3d = output.pos;
    output.uv= input.uv;

    return output;
}




float4 TreePixelShaderFunction(TreeVertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(TreeSampler, input.uv);
	// Make sure tree's vanish in the mist as well.

	float distanceFromCamera = length(input.pos3d - CameraPosition);
	float fogFactor = ComputeFogFactor(distanceFromCamera);

	// do something funky as well to provide fog near the boundaries of the world.
	if( input.pos3d.x < EdgeFog || input.pos3d.x > WorldWidth - EdgeFog || input.pos3d.z < EdgeFog || input.pos3d.z > WorldWidth - EdgeFog)
	{
		fogFactor = 1.0;
	}

	result.rgb = lerp(result.rgb,FogColor,fogFactor);

    return result;
}



technique TileTerrain
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}


technique BillboardTrees
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 TreeVertexShaderFunction();
        PixelShader = compile ps_3_0 TreePixelShaderFunction();
    }

}
