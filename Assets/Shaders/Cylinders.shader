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

            struct intersection
            {
                int count;
                float4 first;
                float4 second;
            };
            
            float getHeight(float radius, float f){
                float r1 = radius;
                if (r1 < f) return 0;
                float a1 = 2 / (f - 1);
                float a = a1 * a1;
                float b = -a * (f + 1);
                float c = 1 - a - b;
                float h = 2.0f - a * r1 * r1 - b * r1 - c;
                return h;
            }

            intersection noIntersection(){
                intersection res;
                res.count = 0;
                res.first = float4(0,0,0,0);
                res.second = float4(0,0,0,0);
                return res;
            }

            intersection oneIntersection(float3 point1){
                intersection res;
                res.count = 1;
                res.first = float4(point1,0);
                res.second = float4(0,0,0,0);
                return res;
            }

            intersection twoIntersections(float3 point1, float3 point2){
                intersection res;
                res.count = 2;
                res.first = float4(point1,0);
                res.second = float4(point2,0);
                return res;
            }

            intersection closestCylinder(float3 ro, float3 rd, float4 cylinder)
            {
                float a1 = rd.x;
                float a2 = ro.x;
                float a3 = rd.y;
                float a4 = ro.y;
                float a5 = rd.z;
                float a6 = ro.z;
                float a7 = cylinder.w*2;
                float a8 = cylinder.x;
                float a9 = cylinder.w*2;
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
                if(disc<0) return noIntersection();
                float t1 = (-b+sqrt(disc))/(2*a);
                float t2 = (-b-sqrt(disc))/(2*a);
                float3 v1 = float3(a1*t1+a2, a3*t1+a4, a5*t1+a6);
                float3 v2 = float3(a1*t2+a2, a3*t2+a4, a5*t2+a6);
                float ymin = cylinder.y;
                float ymax = ymin + getHeight(cylinder.w, 0.1);
                bool v1out = (v1.y < ymin && v2.y < ymin);
                bool v2out = (v1.y > ymax && v2.y > ymax);
                float pos1 = dot(v1-ro, rd);
                float pos2 = dot(v2-ro, rd);
                if(pos1 < 0 && pos2 < 0){
                    return noIntersection();
                }
                else if(pos1 < 0) {
                    v1 = ro;
                }
                else if(pos2 < 0) {
                    v2 = v1;
                    v1 = ro;
                }
                else {
                    float d1 = distance(ro, v1);
                    float d2 = distance(ro, v2);
                    if(d1>d2){
                        float3 tmp = v1;
                        v1 = v2;
                        v2 = tmp;
                    }
                }
                if(v1out || v2out) return noIntersection();
                if(t1==t2) return oneIntersection(v1);
                return twoIntersections(v1,v2);
            }            

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
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
                int hits = 0;
                for(int j=0; j<_CylinderCount; j++)
                {
                    intersection res = closestCylinder(ro, rd, _Cylinders[j]);
                    if(res.count>0) hits++;
                    float d1 = distance(res.first.xyz, ro);
                    float d2 = distance(res.second.xyz, ro);
                    float m = max(d1,d2);
                    // dist += distance(res.first.xyz,res.second.xyz);
                    dist += m-(d1+d2)*0.51f;
                }
                // for(int i=0; i<_CylinderCount; i++)
                // {
                //     dist += dists[i];
                // }
                if(hits==0){
                    float4 sceneCol = tex2D(_MainTex, i.uv);
                    return sceneCol;
                }
                dist = dist / _CylinderCount;
                // dist = 5.0f;
                return float4((dist/1.0f).xxx, 1.0);

                // float xtime = sin(_Time.y);
                // float ytime = cos(_Time.y);
                // float3 LightPos = _LightPosition + float3(xtime, 0, ytime);
                // float4 col = raymarch(ro, rd, LightPos)*_LightColor;
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