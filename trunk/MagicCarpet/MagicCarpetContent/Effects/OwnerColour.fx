// Simple shader file for converting pink (unassigned) to player colour

	float4x4 worldViewProjection;
	float4 playerColour;
	float4 unassignedColour;
	
	texture2D baseTexture;
	sampler baseSampler = sampler_state{Texture = < baseTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	
	struct VS_INPUT
	{
		float4 ObjectPos: POSITION;
		float2 TextureCoords: TEXCOORD0;
		float3 Normal : NORMAL;
	};

	struct VS_OUTPUT 
	{
	   float4 ScreenPos:   POSITION;
	   float2 TextureCoords: TEXCOORD0;
	};

	struct PS_OUTPUT 
	{
	   float4 Colour:   COLOR;
	};

	VS_OUTPUT OwnerColourVS(VS_INPUT In)
	{
	   VS_OUTPUT Out;
		// we get the input vertex but we need to use that to adjust the height
		float4 position = In.ObjectPos;
		Out.ScreenPos = mul(position, worldViewProjection);
		Out.TextureCoords = In.TextureCoords;
		return Out;
	}

	PS_OUTPUT OwnerColourPS(VS_OUTPUT In)
	{
		PS_OUTPUT Out;
		float4 outputColour = tex2D(baseSampler,In.TextureCoords);
		// Replace the un-assigned color with the new one.
		// must be a tidier version
		//if(outputColour == unassignedColour)
		if(outputColour.r == unassignedColour.r &&
			outputColour.g == unassignedColour.g &&
			outputColour.b == unassignedColour.b)
		{
			outputColour = playerColour;
		}
		// TODO - Lighting.
		
		
		Out.Colour = outputColour;
		return Out;
	}
	
	
	//--------------------------------------------------------------//
	// Technique Section for OwnerColourVS screen transform
	//--------------------------------------------------------------//
	technique OwnerColourVS
	{
	   pass Single_Pass
	   {
			LIGHTING = TRUE;
			ZENABLE = TRUE;
			ZWRITEENABLE = TRUE;
			ALPHATESTENABLE = TRUE;
			ALPHABLENDENABLE = FALSE;

			CULLMODE = CCW;

			VertexShader = compile vs_3_0 OwnerColourVS();
			PixelShader = compile ps_3_0 OwnerColourPS();
	   }
	}