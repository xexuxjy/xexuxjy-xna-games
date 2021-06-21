Shader "Gladius/Cutout"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        [LM_TransparencyCutoff] _AlphaTestRef("Alpha Cutoff", Range(0.0,1.0)) = 0.9
    }
        SubShader
    {
        Tags { "RenderType" = "Cutout" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 vertexColor : COLOR;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 testColor = IN.vertexColor;

            // Albedo comes from a texture tinted by color
            fixed4 original = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 c = original * _Color;
            c = c * testColor;
            o.Albedo = c.rgb;
            o.Alpha = original.a;
            o.Alpha = 0;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
