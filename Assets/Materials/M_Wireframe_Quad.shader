/*
  Basalt pressure vessel geometry-based wireframe rendering.

  Looks good and runs fast, but needs a geometry shader, so won't work on WebGL.
  
  Shader from http://www.shaderslab.com/demo-94---wireframe-without-diagonal.html
  by way of https://stackoverflow.com/questions/49350004/wireframe-shader-how-to-display-quads-and-not-triangles
  
  Heavily reworked by Orion Lawlor, lawlor@alaska.edu, 2020-08-03 (Public Domain)
     - Folded the two copies together via CGINCLUDE
     - Use clip instead of discard
*/
Shader "Custom/Wireframe"
{
    Properties
    {
        _WireWidth ("Wire width", Range(0., 0.5)) = 0.05
        _WireColor ("Wire color", color) = (0., 0., 0., 1.)
    }
    
    CGINCLUDE
    /* All code is shared between the front and back passes;
       without lighting we can do Cull Off; with lighting we'd need separate front and back.
    */
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #include "UnityCG.cginc"

            struct v2g {
                float4 worldPos : SV_POSITION;
            };

            struct g2f {
                float4 pos : SV_POSITION;
                float3 bary : TEXCOORD0;
            };

            v2g vert(appdata_base v) {
                v2g o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            g2f project(float4 worldPos,float3 barycentric) {
                g2f o;
                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                o.bary = barycentric;
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) {
                float3 param = float3(0., 0., 0.);

                float EdgeA = length(IN[0].worldPos - IN[1].worldPos);
                float EdgeB = length(IN[1].worldPos - IN[2].worldPos);
                float EdgeC = length(IN[2].worldPos - IN[0].worldPos);

                if(EdgeA > EdgeB && EdgeA > EdgeC)
                    param.y = 1.;
                else if (EdgeB > EdgeC && EdgeB > EdgeA)
                    param.x = 1.;
                else
                    param.z = 1.;
                
                float lo=0.001; //<- nonzero, to filter when far away?
                triStream.Append(project(IN[0].worldPos,float3(1., lo, lo) + param));
                triStream.Append(project(IN[1].worldPos,float3(lo, lo, 1.) + param));
                triStream.Append(project(IN[2].worldPos,float3(lo, 1., lo) + param));
            }

            float _WireWidth;
            fixed4 _WireColor;

            fixed4 frag(g2f i) : SV_Target {
                // Figure out if we're close to any edge
                float edge = min(min(i.bary.x,i.bary.y),i.bary.z);
                clip(_WireWidth-edge);  // kill pixels too far from any edge
                float4 ret=_WireColor;
                // FIXME: alpha fade like mipmaps, alpha blend edges

                return ret;
            }
    
    ENDCG
    
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }

        Pass
        {
            Cull Off
            // Blend SrcAlpha OneMinusSrcAlpha // <- enable alpha blending
            
            CGPROGRAM
            /* Automatically brings in the CGINCLUDE code above */
            ENDCG
        }
    }
}

