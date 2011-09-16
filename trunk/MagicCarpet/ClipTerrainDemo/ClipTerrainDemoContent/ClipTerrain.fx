texture fineLevelTexture;
texture normalsTexture;
texture rampTexture;
texture HeightMapTexture;

uniform matrix WorldViewProjMatrix;
uniform float  ZScaleFactor;
uniform float4 ScaleFactor;
uniform float4 FineTextureBlockOrigin;
uniform float2 AlphaOffset;
uniform float2 ViewerPos;
uniform float  OneOverWidth;
uniform float3 LightDirection;
float4 BlockColor;
float OneOverMaxExtents;
float2 TerrainTextureWindow;


struct VertexShaderInput
{
	float2 gridPos: TEXCOORD0;
};


struct VertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
    float2 zalpha : TEXCOORD1;      // coordinates for elevation-map lookup
};



uniform sampler ElevationSampler = sampler_state            // fine level height sampler
{
    Texture   = (HeightMapTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

uniform sampler NormalMapSampler = sampler_state            
{
    Texture   = (normalsTexture);
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

uniform sampler ZBasedColorSampler = sampler_state
{
    Texture   = (rampTexture);
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


// Vertex shader for rendering the geometry clipmap
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
    // convert from grid xy to world xy coordinates
    //  ScaleFactor.xy: grid spacing of current level
    //  ScaleFactor.zw: origin of current block within world
    float2 worldPos = input.gridPos * ScaleFactor.xy + ScaleFactor.zw;
                     
    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture
    //  FineBlockOrig.zw: origin of block in texture           
    //float2 uv = float2(input.gridPos*FineTextureBlockOrigin.zw+FineTextureBlockOrigin.xy);
	// calc texture coordinate (assuming positions are centered)
	float2 uv = float2((worldPos.x * OneOverMaxExtents)+0.5,(worldPos.y * OneOverMaxExtents)+0.5);
    uv += TerrainTextureWindow;
    // sample the vertex texture
    float zf_zd = tex2Dlod(ElevationSampler, float4(uv, 0, 1));

    // unpack to obtain zf and zd = (zc - zf)
    //  zf is elevation value in current (fine) level
    //  zc is elevation value in coarser level
//    float zf   = floor(zf_zd);
  //  float zd   = frac(zf_zd) * 512 - 256;       // zd = zc - z

    // compute alpha (transition parameter), and blend elevation.
    //float2 alpha = clamp((abs(worldPos-ViewerPos) - AlphaOffset) * OneOverWidth, 0, 1);
    //alpha.x  = max(alpha.x, alpha.y);   
    
    //float z = zf + alpha.x * zd;
	float z = zf_zd;
    z = z * ZScaleFactor;
    
    output.pos = mul(float4(worldPos.x, z,worldPos.y, 1), WorldViewProjMatrix);
	//output.pos = mul(float4(worldPos.x, 0,worldPos.y, 1), WorldViewProjMatrix);
    
    output.uv = uv;
    //output.zalpha = float2(0.5 + z/1600, alpha.x);
    output.zalpha = 1.0;
    
    return output;

}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // do a texture lookup to get the normal in current level
    float4 normalfc = tex2D(NormalMapSampler, input.uv);
    // normal_fc.xy contains normal at current (fine) level
    // normal_fc.zw contains normal at coarser level
    // blend normals using alpha computed in vertex shader  
    float3 normal = float3((1 - input.zalpha.y) * normalfc.xy + input.zalpha.y * (normalfc.zw), 1.0);
    
    // unpack coordinates from [0, 1] to [-1, +1] range, and renormalize.
    normal = normalize(normal * 2 - 1);

    float s = clamp(dot(normal, LightDirection), 0, 1); 
    //return s * tex1D(ZBasedColorSampler, input.zalpha.x);
	return BlockColor;
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
