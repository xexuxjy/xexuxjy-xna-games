#include "../Common.fx"

/*
 * This has been built partly from the Tree/Leave effect samples of LTrees, credit below

 * Copyright (c) 2007-2009 Asger Feldthaus
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

// Should be:  InverseReferenceFrame * AbsoluteBoneTransform
#define MAXBONES 20
float4x4 Bones[MAXBONES];

float HeightMapTexelWidth;

texture HeightMapTexture;
uniform sampler ElevationSampler = sampler_state
{
    Texture   = (HeightMapTexture);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


Texture2D LeafTexture;
float LeafScale = 0.1f;

float3 BillboardRight = float3(1,0,0);	// The billboard's right direction in view space
float3 BillboardUp = float3(0,1,0);		// The billboard's up direction in view space

sampler LeafTextureSampler = sampler_state
{
	Texture = (LeafTexture);
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};

struct LeafVertexShaderInput
{
    float4 pos : POSITION0;
    float2 uv : TEXCOORD0;
    float2 Offset : TEXCOORD1;
    float4 Color : COLOR0;
    int2 BoneIndex : TEXCOORD2;
    float3 normal : NORMAL;
};

struct LeafVertexShaderOutput
{
    float4 pos : POSITION0;
    float2 uv : TEXCOORD0;
	float3 pos3d : TEXCOORD1;
	float3 normal : TEXCOORD2;
    float4 Color : COLOR0;
};


struct TrunkVertexShaderInput
{
	float4 pos : POSITION0;
    float3 normal   : NORMAL;
    float2 uv : TEXCOORD0;
    int2 BoneIndex : TEXCOORD1;
};

struct TrunkVertexShaderOutput
{
    float4 pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD1;
	float3 normal : TEXCOORD2;
};


TrunkVertexShaderOutput TrunkVertexShaderFunction(TrunkVertexShaderInput input, float4x4 instanceTransform)
{
    TrunkVertexShaderOutput output;
    float2 uv = float2(input.pos.x + WorldWidth/2,input.pos.z+WorldWidth/2) * HeightMapTexelWidth;

	float4 posCopy = input.pos;
	float height = tex2Dlod(ElevationSampler, float4(uv, 0, 1));
	height = 0;

	posCopy.y += height;

	float4 localPosition = mul(posCopy, Bones[input.BoneIndex.x]);
    float4 worldPosition = mul(localPosition, instanceTransform);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);
	output.uv = input.uv;
	output.pos3d = input.pos;
	output.normal = mul(input.normal,instanceTransform);

    return output;
}

float4 TrunkPixelShaderFunction(TrunkVertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 result = tex2D(TextureSampler, input.uv);

	// lighting
	float3 lightDir = LightDirection;
    
    float dotResult = dot(-lightDir, input.normal);    
	dotResult = saturate(dotResult);

	float3 directionalComponent = DirectionalLightColor * DirectionalLightIntensity * dotResult;
	float4 light = float4(directionalComponent + (AmbientLightColor * AmbientLightIntensity),1);

	result *= light;
	
	// Fog stuff.
	float fogFactor = ComputeFogFactor(input.pos3d);

	// do something funky as well to provide fog near the boundaries of the world.
	result.rgb = lerp(result.rgb,FogColor,fogFactor);

	result.a = 1;



	return result;
}

// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
TrunkVertexShaderOutput TrunkHardwareInstancingVertexShader(TrunkVertexShaderInput input,
                                                  float4x4 instanceTransform : BLENDWEIGHT)
{
    return TrunkVertexShaderFunction(input, mul(WorldMatrix, transpose(instanceTransform)));
}



LeafVertexShaderOutput LeafVertexShaderFunction(LeafVertexShaderInput input,
                                                  float4x4 instanceTransform : BLENDWEIGHT)
{
    LeafVertexShaderOutput output;
	
	float4 localPosition = mul(input.pos, Bones[input.BoneIndex.x]);
    float4 worldPosition = mul(localPosition, instanceTransform);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    viewPosition.xyz += (input.Offset.x * BillboardRight + input.Offset.y * BillboardUp) * LeafScale;

    output.pos = mul(viewPosition, ProjMatrix);
	output.uv = input.uv;
	output.pos3d = input.pos;
	float3 normal = mul(input.normal, Bones[input.BoneIndex.x]);
	normal = mul(normal, instanceTransform);
	output.normal = normal;
	output.Color = float4(1,1,1,1);
    return output;
}

LeafVertexShaderOutput LeafHardwareInstancingVertexShader(LeafVertexShaderInput input,
                                                  float4x4 instanceTransform : BLENDWEIGHT)
{
    return LeafVertexShaderFunction(input, mul(WorldMatrix, transpose(instanceTransform)));
}


float4 LeafPixelShaderFunction(LeafVertexShaderOutput input) : COLOR0
{
	// we use a larger mipmap for the alpha channel so the leaves don't look transparent
    return float4(input.Color * tex2D(LeafTextureSampler, input.uv).rgb, tex2Dbias(LeafTextureSampler, float4(input.uv.xy, 1, -1)).a);
}

float4 LeafPixelShaderFunctionOpaque(LeafVertexShaderOutput input) : COLOR0
{
	float4 result = LeafPixelShaderFunction(input);

	// XNA 4.0 doesn't support AlphaTestEnable state, so need to replicate
	// that functionality with this.
	clip((result.a < 230.0 / 255.0) ? -1 : 1);

	return result;
}

float4 LeafPixelShaderFunctionBlendedEdges(LeafVertexShaderOutput input) : COLOR0
{
	float4 result = LeafPixelShaderFunction(input);

	// XNA 4.0 doesn't support AlphaTestEnable state, so need to replicate
	// that functionality with this.
	clip((result.a > 230.0 / 255.0) ? -1 : 1);

	return result;
}

technique LeafHardwareInstancing
{
    pass Opaque
    {
        VertexShader = compile vs_2_0 LeafHardwareInstancingVertexShader();
        PixelShader = compile ps_2_0 LeafPixelShaderFunctionOpaque();
        
        AlphaBlendEnable = false;
        
        ZEnable = true;
        ZWriteEnable = true;
        
        CullMode = None;
    }
    pass BlendedEdges
    {
        VertexShader = compile vs_2_0 LeafHardwareInstancingVertexShader();
        PixelShader = compile ps_2_0 LeafPixelShaderFunctionBlendedEdges();
        
        AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;

        ZEnable = true;
        ZWriteEnable = false;

        CullMode = None;
    }
}
technique LeafSetNoRenderStates
{
	pass Pass1
	{
        VertexShader = compile vs_2_0 LeafHardwareInstancingVertexShader();
        PixelShader = compile ps_2_0 LeafPixelShaderFunction();
	}
}


// Hardware instancing technique.
technique TrunkHardwareInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 TrunkHardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 TrunkPixelShaderFunction();
    }
}
