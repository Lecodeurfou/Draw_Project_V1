Shader "Hand_Make/SobelShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DeltaX ("Delta X", Range(0.,1.))=0.01
		_DeltaY ("Delta Y", Range(0.,1.))=0.01
	}

	SubShader {
		Tags { "RenderType"="Opaque" }

		LOD 200
		Pass{

		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float _DeltaX;
		float _DeltaY;


		float sobel(sampler2D tex, float2 uv){
			float2 delta =float2(_DeltaX,_DeltaY);

			float4 px = float4(0,0,0,0);
			float4 py = float4(0,0,0,0);

			//comparable a mat 3X3

			px += tex2D(tex,(uv + float2(-1.0,-1.0)*delta))*1.0;
			px += tex2D(tex,(uv + float2(0.0,-1.0)*delta))*0.0;
			px += tex2D(tex,(uv + float2(1.0,-1.0)*delta))*-1.0;
			px += tex2D(tex,(uv + float2(-1.0,0.0)*delta))*2.0;
			px += tex2D(tex,(uv + float2(0.0,0.0)*delta))*0.0;
			px += tex2D(tex,(uv + float2(1.0,0.0)*delta))*-2.0;
			px += tex2D(tex,(uv + float2(-1.0,1.0)*delta))*1.0;
			px += tex2D(tex,(uv + float2(0.0,1.0)*delta))*0.0;
			px += tex2D(tex,(uv + float2(1.0,1.0)*delta))*-1.0;


			py += tex2D(tex,(uv + float2(-1.0,-1.0)*delta))*-1.0;
			py += tex2D(tex,(uv + float2(0.0,-1.0)*delta))*-2.0;
			py += tex2D(tex,(uv + float2(1.0,-1.0)*delta))*-1.0;
			py += tex2D(tex,(uv + float2(-1.0,0.0)*delta))*0.0;
			py += tex2D(tex,(uv + float2(0.0,0.0)*delta))*0.0;
			py += tex2D(tex,(uv + float2(1.0,0.0)*delta))*0.0;
			py += tex2D(tex,(uv + float2(-1.0,1.0)*delta))*1.0;
			py += tex2D(tex,(uv + float2(0.0,1.0)*delta))*2.0;
			py += tex2D(tex,(uv + float2(1.0,1.0)*delta))*1.0;

			return sqrt(px*px + py*py);
		}

        fixed4 _Color;
		
		float4 frag (v2f_img IN) : COLOR {
			
			float sobel2 = sobel(_MainTex,IN.uv)*_Color;
			return float4(sobel2,sobel2,sobel2,1);
		}
		ENDCG
		}
	}
		FallBack "Diffuse"
}
