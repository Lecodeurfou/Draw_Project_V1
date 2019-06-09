Shader "Hand_Make/GeowireShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _EpaisseurVal ("Epaisseur" , Range (0., 0.34))= 0.05
        _ArriereColor("Couleur Avant",color) = (1,1,1,1)  //les valeurs sont inversées
        _AvantColor("Couleur Arriere", color) = (1,1,1,1) // BIS

    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" }

        Pass{
            Cull Front //face arriere
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #include "UnityCG.cginc"

            struct v2g {
                float4 geo : SV_POSITION;
            };

            struct g2f{
                float4 geo : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            v2g vert(appdata_base v){
                v2g o;
                o.geo = UnityObjectToClipPos(v.vertex);
                return o;
            }
            [maxvertexcount(3)] //pour pallier a la place limité du geo shader
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream){
                g2f o;
                o.geo= IN[0].geo;
                o.uv = float3(1.,0.,0.);
                triStream.Append(o);
                
                o.geo= IN[1].geo;
                o.uv = float3(0.,0.,1.);
                triStream.Append(o);
                
                o.geo= IN[2].geo;
                o.uv = float3(0.,1.,0.);
                triStream.Append(o);
            }

            float _EpaisseurVal;
            fixed4 _AvantColor;

            fixed4 frag(g2f i) : SV_Target {
                if (!any(bool3(i.uv.x < _EpaisseurVal, i.uv.y < _EpaisseurVal, i.uv.z < _EpaisseurVal)))
                     discard;
                

                return _AvantColor;
            }
            ENDCG

        }

        Pass{
            Cull Back //face arriere
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #include "UnityCG.cginc"

            struct v2g {
                float4 geo : SV_POSITION;
            };

            struct g2f{
                float4 geo : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            v2g vert(appdata_base v){
                v2g o;
                o.geo = UnityObjectToClipPos(v.vertex);
                return o;
            }
            [maxvertexcount(3)] //pour pallier a la place limité du geo shader
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream){
                g2f o;
                o.geo= IN[0].geo;
                o.uv = float3(1.,0.,0.);
                triStream.Append(o);
                
                o.geo= IN[1].geo;
                o.uv = float3(0.,0.,1.);
                triStream.Append(o);
                
                o.geo= IN[2].geo;
                o.uv = float3(0.,1.,0.);
                triStream.Append(o);
            }

            float _EpaisseurVal;
            fixed4 _ArriereColor;

            fixed4 frag(g2f i) : SV_Target {
                if (!any(bool3(i.uv.x < _EpaisseurVal, i.uv.y < _EpaisseurVal, i.uv.z < _EpaisseurVal)))
                     discard;
                

                return _ArriereColor;
            }
            ENDCG

        }






    }
    FallBack "Diffuse"
}