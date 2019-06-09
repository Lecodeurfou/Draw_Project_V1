Shader "Hand_Make/BlurShader" {
    
		Properties 
		{
        _Color ("Main Color", Color) = (1,1,1,1)
        
        _MainTex ("Tint Color (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        
        _Size ("Size", Range(0, 20)) = 1
    	_BumpAmt  ("Distortion", Range (0,128)) = 10
    }
   
    Category {

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
    }
   
        SubShader {
        //horizontal BLUR
            GrabPass {                     
                Tags { "LightMode" = "Always" }
            }

        Pass
        {
            Tags{"LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //pragma
            #pragma fragmentoption ARB_precision_hint_fastest 
            
            #include "UnityCG.cginc"


                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv: TEXCOORD0;
                };
               
                struct v2f {
                    float4 vertex : SV_POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
               
                v2f vert (appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Size;
               
                half4 frag( v2f i ) : COLOR {
//                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
//                  return col;
                   
                    half4 som = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
 
 					som+=GRABPIXEL(0.05, -4.0);
              	    som+=GRABPIXEL(0.09, -3.0);
                 	som+=GRABPIXEL(0.12, -2.0);
              	    som+=GRABPIXEL(0.15, -1.0);
              	  	som+=GRABPIXEL(0.18,  0.0);
             	    som+=GRABPIXEL(0.15, +1.0);
              	  	som+=GRABPIXEL(0.12, +2.0);
             	    som+=GRABPIXEL(0.09, +3.0);
              	  	som+=GRABPIXEL(0.05, +4.0);

               		 return som;
                }
                ENDCG
            }
 
 		//vertical BLUR
        GrabPass{
            Tags{"LightMode"="Always"}
        }
        Pass{
            Tags{"LightMode"= "Always"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
               
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv: TEXCOORD0;
                };
               
                struct v2f {
                    float4 vertex : SV_POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
               
                v2f vert (appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Size;
               
                half4 frag( v2f i ) : COLOR {
              
                //fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                //return col;

                    half4 som = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight
 
                   
           	       som+=GRABPIXEL(0.05, -4.0);
            	   som+=GRABPIXEL(0.09, -3.0);
           	       som+=GRABPIXEL(0.12, -2.0);
             	   som+=GRABPIXEL(0.15, -1.0);
             	   som+=GRABPIXEL(0.18,  0.0);
             	   som+=GRABPIXEL(0.15, +1.0);
             	   som+=GRABPIXEL(0.12, +2.0);
            	   som+=GRABPIXEL(0.09, +3.0);
             	   som+=GRABPIXEL(0.05, +4.0);

                return som;

                }
                ENDCG
            }
           
            // Distortion
            GrabPass {                         
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
               
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv: TEXCOORD0;
                };
               
                struct v2f {
                    float4 uvgrab : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float2 uvbump : TEXCOORD1;
                    float2 uvmain : TEXCOORD2;
                };
               
                float _BumpAmt;
                float4 _BumpMap_ST;
                float4 _MainTex_ST;
               
                v2f vert (appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.uvbump = TRANSFORM_TEX( v.uv, _BumpMap );
                    o.uvmain = TRANSFORM_TEX( v.uv, _MainTex );
                    return o;
                }
               
                fixed4 _Color;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                sampler2D _BumpMap;
                sampler2D _MainTex;
               
                half4 frag( v2f i ) : COLOR {
                    // calculate perturbed coordinates
                    half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg;
                    float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
                    i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
                   
                    half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                    half4 tint = tex2D( _MainTex, i.uvmain ) * _Color;
                   
                    return col * tint;
                }
                ENDCG
            }
   }
}
 