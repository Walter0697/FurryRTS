Shader "Custom/OutlineShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Tol ("accumlation angle", Float) = 0.9
		_Thickness ("Thickness", Float) = 0.05
		_Transparent ("Transparent", Range(0, 1)) = 0.5
    } 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
			float3 viewDir;
			float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _Thickness;
		float _Tol;
		half _Transparent;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float4 sd = mul(UNITY_MATRIX_T_MV, o.viewDir);

			sd = normalize(sd);
			if (dot(v.normal, sd.xyz) >= _Tol)
			{
				v.vertex.xyz += sd.xyz * 0.01;
			}
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 normal_nor = normalize(IN.worldNormal);
			float3 view_nor = -normalize(IN.viewDir);
			float dot_prod = dot(normal_nor, view_nor);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			if (dot_prod <= _Thickness && dot_prod >= -_Thickness)
				c = tex2D(_MainTex, IN.uv_MainTex) * _Color * _Transparent;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

			o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
