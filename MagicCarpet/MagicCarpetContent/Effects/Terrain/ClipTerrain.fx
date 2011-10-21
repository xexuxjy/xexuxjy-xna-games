texture HeightMapTexture;
texture BaseTexture;
texture NoiseTexture;

uniform matrix WorldViewProjMatrix;
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



struct VertexShaderInput
{
	float2 gridPos: TEXCOORD0;
};


struct VertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
    float2 zalpha : TEXCOORD1;      // coordinates for elevation-map lookup
	float2 noiseuv : TEXCOORD2;      // coordinates for noise lookup
	float3 normal : TEXCOORD3;
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
    //float2 worldPos = input.gridPos * ScaleFactor.xy + ScaleFactor.zw;
	float2 worldPos = input.gridPos+FineTextureBlockOrigin.zw;
                     
    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture
    //  FineBlockOrig.zw: origin of block in texture           
    float2 uv = float2((worldPos)*FineTextureBlockOrigin.xy);
    float height = tex2Dlod(ElevationSampler, float4(uv, 0, 1));


    float2 uv1 = float2((worldPos.x+1)*FineTextureBlockOrigin.x,worldPos.y * FineTextureBlockOrigin.y);
    float2 uv2 = float2((worldPos.x)*FineTextureBlockOrigin.x,(worldPos.y+1) * FineTextureBlockOrigin.y);

    float heightxplus1 = tex2Dlod(ElevationSampler, float4(uv1, 0, 1));
	float heightyplus1 = tex2Dlod(ElevationSampler, float4(uv2, 0, 1));

	float3 c0 = float3(worldPos.x,height,worldPos.y);
	float3 c1 = float3(worldPos.x+ScaleFactor.x,heightxplus1,worldPos.y);
	float3 c2 = float3(worldPos.x,heightyplus1,worldPos.y+ScaleFactor.y);

	output.normal = normalize(cross((c2-c0), (c1-c0)));
    
/*
	float2 uv1 = float2(((worldPos.x+ScaleFactor.x) * OneOverMaxExtents)+0.5,(worldPos.y * OneOverMaxExtents)+0.5);
	float2 uv2 = float2((worldPos.x * OneOverMaxExtents)+0.5,((worldPos.y+ScaleFactor.y) * OneOverMaxExtents)+0.5);

    float xplus1 = tex2Dlod(ElevationSampler, float4(uv1, 0, 1));
	float yplus1 = tex2Dlod(ElevationSampler, float4(uv2, 0, 1));

	float3 c0 = float3(worldPos.x,xy,worldPos.y);
	float3 c1 = float3(worldPos.x+ScaleFactor.x,xplus1,worldPos.y);
	float3 c2 = float3(worldPos.x,yplus1,worldPos.y+ScaleFactor.y);

	output.normal = normalize(cross((c2-c0), (c1-c0)));
	*/
//	output.normal = float3(0,1,0);

    
    output.pos = mul(float4(worldPos.x, height,worldPos.y, 1), WorldViewProjMatrix);
    output.uv = uv;
	output.noiseuv = float2(worldPos.x/10.0,worldPos.y/10.0);


    //output.zalpha = float2(0.5 + z/1600, alpha.x);
    output.zalpha = 1.0;
    
    return output;

}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // do a texture lookup to get the normal in current level
    //float4 normalfc = tex2D(NormalMapSampler, input.uv);
    // normal_fc.xy contains normal at current (fine) level
    // normal_fc.zw contains normal at coarser level
    // blend normals using alpha computed in vertex shader  
    //float3 normal = float3((1 - input.zalpha.y) * normalfc.xy + input.zalpha.y * (normalfc.zw), 1.0);
    
    // unpack coordinates from [0, 1] to [-1, +1] range, and renormalize.
    //normal = normalize(normal * 2 - 1);

    //float s = clamp(dot(normal, LightDirection), 0, 1); 
    //return s * tex1D(ZBasedColorSampler, input.zalpha.x);

	float4 result = BlockColor;
	result = tex2Dlod(BaseSampler, float4(input.uv, 0, 1));
	float4 noise = tex2Dlod(NoiseSampler, float4(input.noiseuv,0,1));

	result = result +(noise * 0.3);

	// adjust for lighting.

	float projection = saturate(DotProduct(LightPosition,input.pos,input.normal));
	//projection *= 0.02;
	float3 directionalComponent = DirectionalLight * projection;
	float4 light = (AmbientLight + directionalComponent,1);
	//float4 light = (directionalComponent,1);
	//result *= light;
	
	//light = float4(0.1,0.1,0.1,1);
	
	
	result *= light;

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
