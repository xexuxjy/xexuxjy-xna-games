Shader "Gladius/DetailMap" 
{
    Properties{
      _MainTex("Texture", 2D) = "white" {}
      _Detail("Detail", 2D) = "white" {}
    }
        SubShader{
          Tags { "RenderType" = "Opaque" }
          CGPROGRAM
     #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
    {
              float2 uv_MainTex ;
              float2 uv_Detail ;
              fixed4 vertexColor : COLOR;
          };
          sampler2D _MainTex;
          sampler2D _Detail;
          
          void surf(Input IN, inout SurfaceOutputStandard o) 
          {
              fixed4 testColor = IN.vertexColor;

              o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
              o.Albedo *= tex2D(_Detail, IN.uv_Detail).rgb * 2;

              o.Albedo *= testColor;

              //o.alb
              

              //o.Alpha = c.a;


          }
          ENDCG
    }
        Fallback "Diffuse"
}