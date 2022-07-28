Shader "Doctrina/wetFloor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicTex ("Metallic Texture", 2D) = "white" {}
        _NormalMap1 ("NormalTexture1", 2D) = "white" {}
        _NormalMap2 ("NormalTexture2", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _ReflectionTex ("Internal Reflection", 2D) = "" {}  
        _Wet ("WetParam", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _MetallicTex;
        sampler2D _NormalMap1;
        sampler2D _NormalMap2; 
        sampler2D _ReflectionTex;

		struct Input {
			float2 uv_MainTex;
            float2 uv_MetallicTex;
            float2 uv_NormalMap1;
            float2 uv_NormalMap2;
            float4 screenPos;
		};

		half _Glossiness;
		half _Metallic;
        half _Wet;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
            fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(IN.screenPos));
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MetallicTex, IN.uv_MetallicTex);
            m = m*_Wet;
          float3 nmp1 = UnpackNormal( tex2D(_NormalMap1, IN.uv_NormalMap1));
           float3 nmp2 = UnpackNormal( tex2D(_NormalMap2, IN.uv_NormalMap2));
			o.Albedo = c.rgb + refl*pow(m.r, 3)*10;
            
            o.Normal = (nmp1+nmp2)/2;
			// Metallic and smoothness come from slider variables
			o.Metallic  = m.r;
			o.Smoothness = m.a;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
