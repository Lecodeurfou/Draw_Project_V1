Shader "Hand_Make/HoloSurfaceShader"
{
    Properties {
    	_Color ("Color", Color)=(1, 1, 1, 1)
    	//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
		//_BumpMap("Normal Map", 2D)="bump"{}
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _HoloValue ("Holo value", Range(0, 1)) = 0.5
        
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
    FallBack "Diffuse"
}