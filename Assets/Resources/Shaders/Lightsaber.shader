Shader "Hand_Make/LightSaberShad"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		_FadePower("Fade Power", Range(1, 10)) = 5.0
		//_HoloValue ("Holo value", Range(0, 1)) = 0.5
	}
		SubShader
	{
		Tags{ "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}
		Blend SrcAlpha DstAlpha


		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float dotProduct : VIEWDIRECTION;
	};

	float4 _Color;
	float _FadePower;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
		viewDir.y = 0;
		viewDir = normalize(viewDir);

		o.dotProduct = dot(v.normal, viewDir);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		if (i.dotProduct <= 0.1) {
			clip(-1);
		}
		else if (i.dotProduct > 0.1) {
			_Color.w = _Color.w*pow(i.dotProduct, _FadePower);

		}

		return _Color;
	}
		ENDCG
	}
	}

	    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
 	
 		LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha


        // Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
 
        sampler2D _MainTex;
        fixed _HoloValue;
 
        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };
 
        fixed4 _Color;
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex + _Time.y * 0.1);
            o.Albedo = c.rgb * _Color;
 
            float3 normal = normalize(IN.worldNormal);
            float3 dir = normalize(IN.viewDir);
            float val = 1 - (abs(dot(dir, normal)));
            float rim = val * val *  _HoloValue;
            o.Alpha = c.a * rim;
        }
        ENDCG
    }
}
