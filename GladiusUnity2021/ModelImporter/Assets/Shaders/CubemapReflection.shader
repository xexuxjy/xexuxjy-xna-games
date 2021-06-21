Shader "Gladius/CubemapReflection" 
{
	Properties{
	  _MainTex("Texture", 2D) = "white" {}
	  _Cube("Cubemap", CUBE) = "" {}
	}
		SubShader{
		 Tags { "RenderType" = "Opaque" }
		  CGPROGRAM
		 #pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
	
		struct Input 
		{
			  float2 uv_MainTex;
			  float3 worldRefl;
			  float4 vertexColor : COLOR;
		  };
		  sampler2D _MainTex;
		  samplerCUBE _Cube;
		  void surf(Input IN, inout SurfaceOutputStandard o) 
		  {
			  //o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgba * 0.5;
			  float4 mainTexSample = tex2D(_MainTex, IN.uv_MainTex);
			  o.Albedo = mainTexSample.rgb;

			  float4 cubeMapSample = texCUBE(_Cube, IN.worldRefl).rgba;
			  o.Emission = (1 - mainTexSample.a) * (cubeMapSample * 0.5);
		  
		  }
		  ENDCG
	}
		Fallback "Diffuse"
}