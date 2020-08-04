/*
  Basalt pressure vessel geometry-based wireframe rendering
  
  Original geometry shader path (not WebGL compatible) based one:
  Shader from http://www.shaderslab.com/demo-94---wireframe-without-diagonal.html
  by way of https://stackoverflow.com/questions/49350004/wireframe-shader-how-to-display-quads-and-not-triangles
  
  Final antialiased barycentric coords from:
  https://forum.unity.com/threads/perspective-wireframe-using-baked-barycentric-coordinates.817179/
  
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 barry0 : TEXCOORD0;
                float4 barry1 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float _WireWidth;
            float4 _WireColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                /* Compute "barry"centric coordinates: (not barycentric, this is weirder!)
                    each vertex sets exactly one element of our vector.
                    Each fragment sees how many elements are nonzero:
                        Only 1: at a vertex
                        Only 2: on a line
                        3: in a triangles
                    The clever part: we just randomly generate the elements.
                */
                float3 skew=float3(0.789,0.234,-0.567); // mix all three axes
                uint indexN=(uint)(64.*frac(13.0*dot(v.normal,skew)));
                uint indexP=(uint)(32.*frac(21.0*dot(v.vertex.xyz,skew)));
                uint indexU=(uint)(8.*frac(17.0*dot(v.uv,skew.xy)));
                uint index=indexN^indexP^indexU; // mix normal, position, and UV entropy
                index = index^(index>>3); //<- pull more entropy into low bits
                uint cycle=index%8; // pick which barry coordinate to make nonzero
                o.barry0 = float4(cycle==0,cycle==1,cycle==2,cycle==3);
                o.barry1 = float4(cycle==4,cycle==5,cycle==6,cycle==7);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                /* A normal vertex will have three nonzero barry coordinates,
                   and all the rest zero.  We want the smallest nonzero. */
                float4 B0NZ=i.barry0+(i.barry0<=0.); // if <=0, add one (out of the running)
                float4 B1NZ=i.barry1+(i.barry1<=0.);
                float4 Bmin=min(B0NZ,B1NZ);
                float minB=min(
                    min(Bmin.x,Bmin.y),
                    min(Bmin.z,Bmin.w)
                );
                
                minB*=1.0/_WireWidth; // set line width
                
                // Discard if in the middle of a polygon
                if (minB>1.0f) clip(-1);
            
                // grab our color
                fixed4 col = _WireColor;
                
                //col.rgb=i.barry0*bright; // debug
                
                //col=clamp(col,0.0,1.0);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
    
    ENDCG
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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

