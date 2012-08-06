#include "../Common.fx"

/*
 * This has been built partly from the Tree/Leave effect samples of LTrees, credit below

 * Copyright (c) 2007-2009 Asger Feldthaus
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

// 1 means we should only accept opaque pixels.
// -1 means only accept transparent pixels.
float AlphaTestDirection = 1;
float AlphaTestThreshold = 0.95;


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

texture BillboardTreeTexture;
uniform sampler BillboardTreeSampler = sampler_state
{
    Texture   = (BillboardTreeTexture);
    MipFilter = None;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

sampler TrunkTextureSampler = sampler_state
{
	Texture = (Texture);
	AddressU = Wrap;
	AddressV = Wrap;
	AddressW = Wrap;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};


struct BillboardTreeVertexShaderInput
{
    float4 pos  : POSITION0;
    float2 uv	: TEXCOORD0;  // coordinates for normal-map lookup
};

struct BillboardTreeVertexShaderOutput
{
    vector pos        : POSITION;   
    float2 uv         : TEXCOORD0;  // coordinates for normal-map lookup
	float3 pos3d : TEXCOORD3;
};


float ComputeHeight(float2 uv:TEXCOORD0)
{
	//float c = abs(tex2D (ElevationSampler, uv));   // center
	float c = tex2Dlod(ElevationSampler, float4(uv, 0, 1));

    float tl = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2(-1, -1),0,1));   // top left
    float  l = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2(-1,  0),0,1));   // left
    float bl = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2(-1,  1),0,1));   // bottom left
    float  t = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2( 0, -1),0,1));   // top

	float  b = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2( 0,  1),0,1));   // bottom
    float tr = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2( 1, -1),0,1));   // top right
    float  r = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2( 1,  0),0,1));   // right
    float br = tex2Dlod (ElevationSampler, float4(uv + HeightMapTexelWidth * float2( 1,  1),0,1));   // bottom right
	
	float numSamples = 9;
	float sum1 = c+tl+l+bl+t;
	float sum2 = b+tr+r+br;
	float result = c;//(float)((sum1+sum2) / numSamples);
	//float result = c;
	
	return result;
}




Texture2D LeafTexture;
float LeafScale = 1.0f;

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

	float4 posCopy = input.pos;

	float4 localPosition = mul(posCopy, Bones[input.BoneIndex.x]);
    float4 worldPosition = mul(localPosition, instanceTransform);


	// height needs to be off the instance transform center, not the vertex otherwise branches follow terrain. oops.


	float3 center = instanceTransform[3].xyz;
    float2 uv = float2(center.x + WorldWidth/2,center.z+WorldWidth/2) * HeightMapTexelWidth;
	float height = ComputeHeight(uv);
	center.y += height;
	worldPosition.y += center.y;

    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);
	output.uv = input.uv;
	output.pos3d = worldPosition;//input.pos;
	
	float3 normal = mul(input.normal, Bones[input.BoneIndex.x]);
	output.normal = normalize(mul(normal,instanceTransform));

    return output;
}

float4 TrunkPixelShaderFunction(TrunkVertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 result = tex2D(TrunkTextureSampler, input.uv);

	// lighting
	float3 lightDir = LightDirection;
    
    float dotResult = dot(-lightDir, input.normal);    
	dotResult = saturate(dotResult);

	float3 directionalComponent = DirectionalLightColor * DirectionalLightIntensity * dotResult;
	float4 light = float4(directionalComponent + (AmbientLightColor * AmbientLightIntensity),1);

	//light = float4(1,1,1,1);

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

    float2 uv = float2(worldPosition.x + WorldWidth/2,worldPosition.z+WorldWidth/2) * HeightMapTexelWidth;
	float height = ComputeHeight(uv);
	worldPosition.y += height;

    float4 viewPosition = mul(worldPosition, ViewMatrix);

	float leafLen = length(instanceTransform[0].xyz);

    viewPosition.xyz += (input.Offset.x * BillboardRight + input.Offset.y * BillboardUp) * LeafScale * leafLen;

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


BillboardTreeVertexShaderOutput BillboardTreeVertexShaderFunction(BillboardTreeVertexShaderInput input,float4x4 instanceTransform)
{
	BillboardTreeVertexShaderOutput output;

	float scale = 1.0;

    // compute coordinates for vertex texture
    //  FineBlockOrig.xy: 1/(w, h) of texture (texelwidth)



	/*
	float3 adjustedPos = float3(input.pos.x,input.pos.y + height,input.pos.z);

	float4 worldPosition = mul(float4(adjustedPos, 1), instanceTransform);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);
	*/


	float3 center = mul(input.pos, instanceTransform);

    float2 uv = float2(center.x + WorldWidth/2,center.z+WorldWidth/2) * HeightMapTexelWidth;
	float height = ComputeHeight(uv);
	center.y += height;

    float3 eyeVector = center - CameraPosition;

    float3 upVector = float3(0,1,0);
    upVector = normalize(upVector);
    float3 sideVector = cross(eyeVector,upVector);
    sideVector = normalize(sideVector);

    float3 finalPosition = center;
    finalPosition += (input.uv.x-0.5f)*sideVector;
    finalPosition += (1.5f-input.uv.y*1.5f)*upVector;

    float4 finalPosition4 = float4(finalPosition, 1);
    float4 viewPosition = mul(finalPosition4, ViewMatrix);
    output.pos = mul(viewPosition, ProjMatrix);


	output.pos3d = output.pos;
    output.uv= input.uv;

    return output;
}


float4 BillboardTreePixelShaderFunction(BillboardTreeVertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(BillboardTreeSampler, input.uv);
    // Apply the alpha test.
    clip((color.a - AlphaTestThreshold) * AlphaTestDirection);

	// Make sure tree's vanish in the mist as well.
/*
	float fogFactor = ComputeFogFactor(input.pos3d);
	float4 result2;
	result2.rgb = lerp(result.rgb,FogColor,fogFactor);
	result2.a = result.a;

	clip(result.w - 0.7843f);
*/
	//color = float4(1,0,0,0);

    return color;
}


BillboardTreeVertexShaderOutput BillboardHardwareInstancingTreeVertexShaderFunction(BillboardTreeVertexShaderInput input,
                                                  float4x4 instanceTransform : BLENDWEIGHT)
{
    return BillboardTreeVertexShaderFunction(input, mul(WorldMatrix, transpose(instanceTransform)));
}




technique LeafHardwareInstancing
{
    pass Opaque
    {
        VertexShader = compile vs_3_0 LeafHardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 LeafPixelShaderFunctionOpaque();
        
        AlphaBlendEnable = false;
        
        ZEnable = true;
        ZWriteEnable = true;
        
        CullMode = None;
    }
    pass BlendedEdges
    {
        VertexShader = compile vs_3_0 LeafHardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 LeafPixelShaderFunctionBlendedEdges();
        
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
        VertexShader = compile vs_3_0 LeafHardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 LeafPixelShaderFunction();
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


technique BillboardTrees
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 BillboardHardwareInstancingTreeVertexShaderFunction();
        PixelShader = compile ps_3_0 BillboardTreePixelShaderFunction();
    }

}
