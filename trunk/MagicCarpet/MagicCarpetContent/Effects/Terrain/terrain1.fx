		//Input variables
	float4x4 worldViewProjection;
	float4 Ambient = float4(0.25, 0.25, 0.25, 1.0); 

	//float4 terrainTextureWeights1;
	//float4 terrainTextureWeights2;

	float timeStep;

	float deepWaterHeightValue;
	float shallowWaterHeightValue;
	float sandHeightValue;
	float grassHeightValue;
	float screeHeightValue;
	float iceHeightValue;

	texture baseTexture;

	texture2D currentHeightMap;
	texture targetHeightMap;

	texture2D deepWaterTexture;
	texture2D shallowWaterTexture;
	texture2D sandTexture;
	texture2D grassTexture;
	texture2D screeTexture;
	texture2D iceTexture;

	sampler2D heightMapSampler = 
	sampler_state
	{
		Texture = < currentHeightMap >;
		MipFilter = POINT;
		MinFilter = POINT;
		MagFilter = POINT;
		ADDRESSU = CLAMP;
		ADDRESSV = CLAMP;
	};




	sampler baseSampler = sampler_state{Texture = < baseTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	sampler deepWaterSampler = sampler_state{Texture = < deepWaterTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	sampler shallowWaterSampler = sampler_state{Texture = < shallowWaterTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	sampler sandSampler = sampler_state{Texture = < sandTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	sampler grassSampler = sampler_state{Texture = < grassTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	sampler screeSampler = sampler_state{Texture = < screeTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};
	sampler iceSampler = sampler_state{Texture = < iceTexture >; MipFilter = LINEAR; MinFilter = LINEAR; MagFilter = LINEAR; ADDRESSU = MIRROR; ADDRESSV = MIRROR;};

	struct VS_INPUT
	{
		float4 ObjectPos: POSITION;
		float2 TextureCoords: TEXCOORD0;
		float3 Normal : NORMAL;
		float TargetPos : TEXCOORD1;
	};

	struct VS_OUTPUT 
	{
	   float4 ScreenPos:   POSITION;
	   float2 TextureCoords: TEXCOORD0;
		float Height : TEXCOORD1;
	};

	struct PS_INPUT 
	{ 
		float2 Texcoord : TEXCOORD0; 
	}; 

	struct PS_OUTPUT 
	{
	   float4 Color:   COLOR;
	};

	float4 Phong(PS_INPUT Input) : COLOR0
	{ 
		return saturate(Ambient); 
	}; 

	VS_OUTPUT SimpleVS(VS_INPUT In)
	{
	   VS_OUTPUT Out;
		// we get the input vertex but we need to use that to adjust the height
		float4 position = In.ObjectPos;
		float targetPosition = In.TargetPos;
		
		float interpHeight = lerp(position.y,targetPosition,timeStep);
		position.y = interpHeight;
		Out.ScreenPos = mul(position, worldViewProjection);
		Out.TextureCoords = In.TextureCoords;
		Out.Height = interpHeight;
		return Out;
	}

	PS_OUTPUT SimplePS(VS_OUTPUT In)
	{
		PS_OUTPUT Out;
		float4 outputColor = float4(0,0,0,1);
		if(In.Height < deepWaterHeightValue)
		{
			outputColor = tex2D(deepWaterSampler,In.TextureCoords);
		}
		else
		if(In.Height >= deepWaterHeightValue && In.Height < shallowWaterHeightValue)
		{
			float weight = smoothstep(deepWaterHeightValue,shallowWaterHeightValue,In.Height);
			weight = 1.0-weight;
			outputColor = tex2D(deepWaterSampler,In.TextureCoords) * weight;
			outputColor += tex2D(shallowWaterSampler,In.TextureCoords) * (1.0 - weight);
		}
		else
		if(In.Height >= shallowWaterHeightValue && In.Height < sandHeightValue)
		{
			float weight = smoothstep(shallowWaterHeightValue,sandHeightValue,In.Height);
			weight = 1.0-weight;
			outputColor = tex2D(shallowWaterSampler,In.TextureCoords) * weight;
			outputColor += tex2D(sandSampler,In.TextureCoords) * (1.0 - weight);
		}
		else
		if(In.Height >= sandHeightValue && In.Height < grassHeightValue)
		{
			float weight = smoothstep(sandHeightValue,grassHeightValue,In.Height);
			weight = 1.0-weight;
			outputColor = tex2D(sandSampler,In.TextureCoords) * weight;
			outputColor += tex2D(grassSampler,In.TextureCoords) * (1.0 - weight);
		}
		else
		if(In.Height >= grassHeightValue && In.Height < screeHeightValue)
		{
			float weight = smoothstep(grassHeightValue,screeHeightValue,In.Height);
			weight = 1.0-weight;
			outputColor = tex2D(grassSampler,In.TextureCoords) * weight;
			outputColor += tex2D(screeSampler,In.TextureCoords) * (1.0 - weight);
		}
		else
		if(In.Height >=screeHeightValue && In.Height < iceHeightValue)
		{
			float weight = smoothstep(screeHeightValue,iceHeightValue,In.Height);
			weight = 1.0-weight;
			outputColor = tex2D(screeSampler,In.TextureCoords) * weight;
			outputColor += tex2D(iceSampler,In.TextureCoords) * (1.0 - weight);
		}
		else
		{
			outputColor += tex2D(iceSampler,In.TextureCoords);
		}
		Out.Color = outputColor;
		return Out;
	}

	//--------------------------------------------------------------//
	// Technique Section for Simple screen transform
	//--------------------------------------------------------------//
	technique Simple
	{
	   pass Single_Pass
	   {
			LIGHTING = TRUE;
			ZENABLE = TRUE;
			ZWRITEENABLE = TRUE;
			ALPHATESTENABLE = TRUE;
			ALPHABLENDENABLE = FALSE;

			CULLMODE = CCW;

			VertexShader = compile vs_3_0 SimpleVS();
			PixelShader = compile ps_3_0 SimplePS();
	   }
	}