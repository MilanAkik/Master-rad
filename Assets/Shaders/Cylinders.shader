Shader "Custom/RaymarchBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"		
		    #include "noiseSimplex.cginc"
            
            sampler2D _MainTex;

            // Camera
            float4x4 _CamToWorld;
            float4x4 _CamInverseProjection;
            float3 _CamPos;
            
            // Light
            float4 _LightColor;
            float3 _LightPosition;
            
            // Cylinders
            float4 _Cylinders[512];
            int _CylinderCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            // // Signed Distance Functions
            // float sdSphere(float3 p, float r) { return length(p) - r; }

            // float SceneSDF(float3 p) { return sdSphere(p - float3(0,0,3), 1.0); }

            // float3 getNormal(float3 p)
            // {
            //     float e = 0.001;
            //     return normalize(float3(
            //         SceneSDF(p + float3(e,0,0)) - SceneSDF(p - float3(e,0,0)),
            //         SceneSDF(p + float3(0,e,0)) - SceneSDF(p - float3(0,e,0)),
            //         SceneSDF(p + float3(0,0,e)) - SceneSDF(p - float3(0,0,e))
            //     ));
            // }

            // float4 raymarch(float3 ro, float3 rd, float3 lightPos)
            // {
            //     float t = 0.0;
            //     const int MAX_STEPS = 128;
            //     const float MAX_DIST = 50.0;
            //     const float EPSILON = 0.001;

            //     for (int i = 0; i < MAX_STEPS; i++)
            //     {
            //         float3 p = ro + rd * t;
            //         float d = SceneSDF(p);
            //         if (d < EPSILON)
            //         {
            //             float3 n = getNormal(p);
            //             float3 lightDir = normalize(lightPos);
            //             float diff = max(0, dot(n, lightDir));
            //             return float4(diff.xxx,1.0);
            //         }
            //         t += d;
            //         if (t > MAX_DIST) break;
            //     }
            //     return float4(0.0, 0.0, 0.0, 0.0); // background color
            // }

            float closestCylinder(float3 ro, float3 rd, float4 cylinder)
            {
                float3 dist = rd;
                float a1 = dist.x;
                float a2 = ro.x;
                float a3 = dist.y;
                float a4 = ro.y;
                float a5 = dist.z;
                float a6 = ro.z;
                float a7 = cylinder.w;
                float a8 = cylinder.x;
                float a9 = cylinder.w;
                float a10 = cylinder.z;
                float sqa7 = a7*a7;
                float sqa5 = a5*a5;
                float sqa9 = a9*a9;
                float sqa1 = a1*a1;
                float a6m10 = (a6-a10);
                float a2m8 = (a2-a8);
                float a = sqa7*sqa5+sqa9*sqa1;
                float b = 2 * (sqa7*a5*a6m10+sqa9*a1*a2m8);
                float c = sqa7*a6m10*a6m10+sqa9*a2m8*a2m8-sqa7*sqa9;
                float disc = b*b-4*a*c;
                if(disc<0) return 0;
                // return 1;
                float t1 = (-b+sqrt(disc))/(2*a);
                float t2 = (-b-sqrt(disc))/(2*a);
                float3 v1 = float3(a1*t1+a2, a3*t1+a4, a5*t1+a6);
                float3 v2 = float3(a1*t2+a2, a3*t2+a4, a5*t2+a6);
                // float d1 = dot(v1-ro, rd);
                // float d2 = dot(v2-ro, rd);
                // if(d1 >= 0) return d1;
                // else if(d2 >= 0) return d2;
                // return 0.1f;
                float d1 = distance(v1, ro);
                float d2 = distance(v2, ro);
                bool v1out = (v1.y<-1 || v1.y>1);
                bool v2out = (v2.y<-1 || v2.y>1);
                if(v1out && v2out) return 0;
                // if(v1out) return 0.7;
                // if(v2out) return 0.2;
                // return 0;
                float m = max(d1,d2);
                return m-(d1+d2)*0.51f;
                // return (d2+d1)/2;
                // return distance(v1,v2);
                float val = distance(ro, v2);
                if(t1<t2) val = distance(ro, v1);
                // if(val<1) return 0.1;
                // if(val<2) return 0.2;
                // if(val<3) return 0.3;
                // if(val<4) return 0.4;
                // if(val<5) return 0.5;
                // if(val<6) return 0.6;
                // if(val<7) return 0.7;
                // if(val<8) return 0.8;
                // if(val<9) return 0.9;
                // return 1;
                return val;
                return abs((ro+t1*rd).z);
                return distance(ro, ro+t1*rd);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert screen UV to NDC (-1..1)
                float2 uv = i.uv * 2.0 - 1.0;
                // Clip-space ray
                float4 rayClip = float4(uv, 1.0, 1.0);
                
                // return rayClip;//float4(uv.x, 0, uv.y, 1);
                // View-space ray
                float4 rayView = mul(_CamInverseProjection, rayClip);
                rayView /= rayView.w;

                float3 worldPos = mul(_CamToWorld, float4(rayView.xyz, 0.0)).xyz;

                // World-space ray

                float3 ro = _CamPos;
                float3 rd = normalize(worldPos - ro);

                // return float4(rd * 0.5f + 0.5f, 1.0f);
                // float dist = closestCylinder(ro, rd, float4(0,0,10,1));
                float dist = 0;
                for(int j=0; j<_CylinderCount; j++)
                {
                    dist += closestCylinder(ro, rd, _Cylinders[j]);
                }
                // for(int i=0; i<_CylinderCount; i++)
                // {
                //     dist += dists[i];
                // }
                dist = dist / _CylinderCount;
                // dist = 5.0f;
                return float4((dist/1.0f).xxx, 1.0);

                // float xtime = sin(_Time.y);
                // float ytime = cos(_Time.y);
                // float3 LightPos = _LightPosition + float3(xtime, 0, ytime);
                // float4 col = raymarch(ro, rd, LightPos)*_LightColor;
                // float4 sceneCol = tex2D(_MainTex, i.uv);
                // float alfa = col.w;
                // float r = (alfa) * col.x + (1-alfa) * sceneCol.x;
                // float g = (alfa) * col.y + (1-alfa) * sceneCol.y;
                // float b = (alfa) * col.z + (1-alfa) * sceneCol.z;
                // fixed4 res = alfa*col+(1-alfa)*sceneCol;
                // return res;
            }
            ENDCG
        }
    }
}