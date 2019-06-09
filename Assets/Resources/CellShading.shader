// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hand_Make/CellShading"
{
	Properties
	{	//_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Seuil("Seuil",Range(1.0,20.0))=5.0
		_Ambiant("Ambiant",Range(0.0,0.5))=0.1

	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always
		//tags ....
		Tags {"RenderType"="Opaque" "LightMode"="ForwardBase"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			
			//struct appdata
			//{
			//	float4 vertex : POSITION;
			//	float2 uv : TEXCOORD0;
			//	float3 worldNormal : NORMAL;
			//};
			
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : NORMAL;
			};

			
			float _Seuil;
			
			float ToonShad(float3 normal , float3 lightDir){
				float NdotL = max(0.0, dot(normalize(normal),normalize(lightDir)));
				return floor(NdotL* _Seuil)/ (_Seuil -0.5);
			}
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);// UnityObjectToClipPos(v.vertex);
				o.uv =TRANSFORM_TEX(v.texcoord, _MainTex);// v.uv;
				o.worldNormal = mul(v.normal.xyz, (float3x3) unity_WorldToObject);
				return o;
			}
			
			half _Ambiant;
			//fixed4 _Color2;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);//*_Color;
				// just invert the colors
				//col.rgb = 1 - col.rgb;
				col.rgb *= saturate(ToonShad(i.worldNormal, _WorldSpaceLightPos0.xyz)+_Ambiant);//*_Color2.rgb;
				return col;
			}
			ENDCG
		}
	}
}
