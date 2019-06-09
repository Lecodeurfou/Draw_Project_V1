Shader "Hand_Make/NeigeShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Bump("BumpTex", 2D) = "bump" {}
		_Direction("Direction", Vector) = (0,1,0)
		_Intensite ("Intensite", Range(-1,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
			float3 worldNormal;
			INTERNAL_DATA
		};
		half4 _Color;
		sampler2D _MainTex;
		sampler2D _Bump;
		half3 _Direction;
		fixed _Intensite;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));

			if(dot (WorldNormalVector(IN, o.Normal), _Direction) >= _Intensite){
				o.Albedo = _Color.rgb;
			}

			else{
			o.Albedo = c.rgb;
			}

			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
