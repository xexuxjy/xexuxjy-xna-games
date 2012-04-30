#include "../Common.fx"

texture HeightMapTexture;
texture BaseTexture;
texture NoiseTexture;
texture NormalMapTexture;
texture TreeTexture;

uniform float  ZScaleFactor;
uniform float4 ScaleFactor;
uniform float4 FineTextureBlockOrigin;
uniform float  OneOverWidth;


float4 BlockColor;
float2 TerrainTextureWindow;


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
    float3 pos  : POSITION;
	float scale : TEXCOORD0;   
    float2 uv	: TEXCOORD1;  // coordinates for normal-map lookup
};

struct TreeVertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD3;
};

/*
struct WallVertexShaderInput
{
    float3 pos  : POSITION;
	float4 color : COLOR;   
};
*/
struct WallVertexShaderInput
{
	float4 pos : POSITION0;
    float3 normal : NORMAL;
    float4 uv : TEXCOORD0;
};



struct WallVertexShaderOutput
{
    vector pos        : POSITION;   
	float3 pos3d : TEXCOORD0;
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
	float result = (float)((sum1+sum2) / numSamples);
	//float result = c;
	
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

    //output.pos = mul(float4(worldPos.x, height,worldPos.y, 1), WorldViewProjMatrix);

	float4 worldPosition = mul(float4(worldPos.x, height,worldPos.y, 1), WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);

	output.normal = float3(0,1,0);
    output.uv = uv;
	output.noiseuv = float2(worldPos.x/10.0,worldPos.y/10.0);
	output.pos3d = output.pos;
    return output;

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
	float fogFactor = ComputeFogFactor(distanceFromCamera,input.pos3d);

	// do something funky as well to provide fog near the boundaries of the world.
	result.rgb = lerp(result.rgb,FogColor,fogFactor);


	return result;

}


// Vertex shader for rendering the geometry clipmap
TreeVertexShaderOutput TreeVertexShaderFunction(TreeVertexShaderInput input)
{
	TreeVertexShaderOutput output;
	float3 worldPos = input.pos;
	float2 worldPos2 = float2(input.pos.x,input.pos.z);
	float scale = input.scale;

    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture (texelwidth)
    float2 uv = float2(worldPos.x + WorldWidth/2,worldPos.z+WorldWidth/2) * FineTextureBlockOrigin.x;

	float height = ComputeHeight(uv);

	float3 adjustedPos = float3(worldPos.x,worldPos.y + height,worldPos.z);

	float4 worldPosition = mul(float4(adjustedPos, 1), WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);

	output.pos3d = output.pos;
    output.uv= input.uv;

    return output;
}




float4 TreePixelShaderFunction(TreeVertexShaderOutput input) : COLOR0
{
	float4 result = tex2D(TreeSampler, input.uv);
	// Make sure tree's vanish in the mist as well.

	float distanceFromCamera = length(input.pos3d - CameraPosition);
	float fogFactor = ComputeFogFactor(distanceFromCamera,input.pos3d);
	float4 result2;
	result2.rgb = lerp(result.rgb,FogColor,fogFactor);
	result2.a = result.a;

    return result;
}

WallVertexShaderOutput WallVertexShaderFunction(WallVertexShaderInput input)
{
    WallVertexShaderOutput output;
    float4 worldPosition = mul(input.pos, WorldMatrix);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);
	output.pos3d = input.pos;
    return output;
}


float4 WallPixelShaderFunction(WallVertexShaderOutput input) : COLOR0
{
	float4 result = float4(FogColor,1);
	//float distanceFromCamera = length(input.pos3d - CameraPosition);
	//float fogFactor = ComputeFogFactor(distanceFromCamera,input.pos3d);
	//result.rgb = lerp(result.rgb,FogColor,fogFactor);
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
        VertexShader = compile vs_3_0 TreeVertexShaderFunction();
        PixelShader = compile ps_3_0 TreePixelShaderFunction();
    }

}

technique TerrainWall
{
	pass Pass1
	{
        VertexShader = compile vs_3_0 WallVertexShaderFunction();
        PixelShader = compile ps_3_0 WallPixelShaderFunction();

	}
}




