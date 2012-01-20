float texelWidth;
texture HeightMapTexture;

float normalStrength = 8.0f;


uniform sampler NormalMapSamplerPS = sampler_state
{
    Texture   = (HeightMapTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


matrix MatrixTransform : register(vs, c0);

struct VertexShaderInput
{
    float3 Position : POSITION0;
	float2 TexCoord : TexCoord0;	
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TexCoord0;    
};


VertexShaderOutput ComputeNormalsVS(VertexShaderInput input) 
{ 
	VertexShaderOutput output = (VertexShaderOutput)0;
    // using this multiply messes things up somhow?
	//output.Position = mul(float4(input.Position, 0), MatrixTransform); 
	output.Position = float4(input.Position, 1);
	output.TexCoord = input.TexCoord;
	return output;
} 

float4 ComputeNormalsPS(in VertexShaderOutput input) : COLOR0
{
/*
    float tl = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2(-1, -1)).x);   // top left
    float  l = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2(-1,  0)).x);   // left
    float bl = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2(-1,  1)).x);   // bottom left
    float  t = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 0, -1)).x);   // top
    float  b = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 0,  1)).x);   // bottom
    float tr = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 1, -1)).x);   // top right
    float  r = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 1,  0)).x);   // right
    float br = abs(tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 1,  1)).x);   // bottom right
	*/
    float tl = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2(-1, -1)).x;   // top left
    float  l = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2(-1,  0)).x;   // left
    float bl = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2(-1,  1)).x;   // bottom left
    float  t = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 0, -1)).x;   // top
    float  b = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 0,  1)).x;   // bottom
    float tr = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 1, -1)).x;   // top right
    float  r = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 1,  0)).x;   // right
    float br = tex2D (NormalMapSamplerPS, input.TexCoord + texelWidth * float2( 1,  1)).x;   // bottom right


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
	//return float4(0,1,0,0);
}

technique ComputeNormals
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 ComputeNormalsVS();
        PixelShader = compile ps_3_0 ComputeNormalsPS();
    }
}
