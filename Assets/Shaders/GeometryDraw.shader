Shader "Custom/GeometryDraw"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DensityNoise ("Density noise 3d texture", 3D) = "white" {}
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

            // Camera parameters
            float4x4 _CamToWorld;
            float4x4 _CamInverseProjection;
            float3 _CamPos;
            
            // Light parameters
            float4 _LightColor;
            float3 _LightPosition;
            
            // Cylinders parameters
            float4 _Cylinders[512];
            float4x4 _CylinderMatrices[512];
            int _CylinderCount;
            
            //Cylinder shape parameters
            float _RadiusMultiplier;
            float _HeightMultiplier;
            float _RadiusThreshold;

            //Area parameters
            float3 _areaMin;
            float3 _areaMax;

            //Density noise parameters
            sampler3D _DensityNoise;
            int _DensityNoiseSize;

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
            
            float getHeight(float radius){
                float r1 = radius;
                if (r1 < _RadiusThreshold) return 0;
                float a1 = 2 / (_RadiusThreshold - 1);
                float a = a1 * a1;
                float b = -a * (_RadiusThreshold + 1);
                float c = 1 - a - b;
                float h = 2.0f - a * r1 * r1 - b * r1 - c;
                return _HeightMultiplier * h;
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
                float a7 = cylinder.w*_RadiusMultiplier;
                float a8 = cylinder.x;
                float a9 = cylinder.w*_RadiusMultiplier;
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
                float pos1 = dot(v1-ro, rd);
                float pos2 = dot(v2-ro, rd);
                // If-elseif-else that checks which of the intersections are behind camera
                if(pos1 < 0 && pos2 < 0){
                    return noIntersection();
                }
                else if(pos1 < 0) {
                    t1 = 0;
                }
                else if(pos2 < 0) {
                    t2 = t1;
                    t1 = 0;
                }
                else {
                    float d1 = distance(ro, v1);
                    float d2 = distance(ro, v2);
                    if(d1>d2){
                        float tmp = t1;
                        t1 = t2;
                        t2 = tmp;
                    }
                }
                v1 = float3(a1*t1+a2, a3*t1+a4, a5*t1+a6);
                v2 = float3(a1*t2+a2, a3*t2+a4, a5*t2+a6);
                float ymin = cylinder.y;
                float ymax = ymin + getHeight(cylinder.w);
                //3x3 of combinations of the positions
                if(v1.y < ymin){
                    if(v2.y < ymin){
                        return noIntersection();
                    }
                    else if(v2.y > ymax) {
                        t2=(ymax-a4)/a3;
                    }
                    t1=(ymin-a4)/a3;
                }
                else if(v1.y > ymax) {
                    if(v2.y > ymax){
                        return noIntersection();
                    }
                    else if(v2.y < ymin) {
                        t2=(ymin-a4)/a3;
                    }
                    t1=(ymax-a4)/a3;
                }
                else {
                    if(v2.y < ymin){
                        t2=(ymin-a4)/a3;
                    }
                    else if(v2.y > ymax) {
                        t2=(ymax-a4)/a3;
                    }
                }
                v1 = float3(a1*t1+a2, a3*t1+a4, a5*t1+a6);
                v2 = float3(a1*t2+a2, a3*t2+a4, a5*t2+a6);
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

            uint pcg(uint v)
            {
                uint state = v * 747796405u + 2891336453u;
                uint word  = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
                return (word >> 22u) ^ word;
            }

            float3 random3(uint seed)
            {
                uint3 h = uint3( pcg(seed), pcg(seed + 1u), pcg(seed + 2u) );
                return float3(h) * (1.0 / float(0xffffffffu));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert screen UV to NDC (-1..1)
                float2 uv = i.uv * 2.0 - 1.0;
                // Clip-space ray
                float4 rayClip = float4(uv, 1.0, 1.0);
                // View-space ray
                float4 rayView = mul(_CamInverseProjection, rayClip);
                rayView /= rayView.w;
                float3 worldPos = mul(_CamToWorld, float4(rayView.xyz, 0.0)).xyz;
                // World-space ray
                float3 ro = _CamPos;
                float3 rd = normalize(worldPos - ro);
                float dist = 0;
                int hits = 0;
                float steps = 20;
                intersection closest;
                float closestDistance = 1000000;
                int closestIndex = 0;
                for(int j=0; j<_CylinderCount; j++)
                {
                    // float x = _Cylinders[j].x;
                    // float x = _CylinderMatrices[j][0][0];
                    // float y = _Cylinders[j].y;
                    // float y = _CylinderMatrices[j][0][1];
                    // float z = _Cylinders[j].z;
                    // float z = _CylinderMatrices[j][0][2];
                    // float w = _Cylinders[j].w;
                    // float w = _CylinderMatrices[j][0][3];
                    // intersection res = closestCylinder(ro, rd, float4(x,y,z,w));
                    intersection res = closestCylinder(ro, rd, _CylinderMatrices[j][0]);
                    // intersection res = closestCylinder(ro, rd, _Cylinders[j]);
                    if(res.count>0){
                        hits++;
                        float4 p = res.first;
                        if(p.x == ro.x && p.y == ro.y && p.z == ro.z) p=res.second;
                        float currDist = distance(ro, p);
                        if(currDist<closestDistance){
                            closest = res;
                            closestDistance = currDist;
                            closestIndex = j;
                        }
                    }
                }
                float4 skyColor = tex2D(_MainTex, i.uv);
                if(hits==0)return skyColor;
                return float4(random3(closestIndex), 1.0f);
            }
            ENDCG
        }
    }
}