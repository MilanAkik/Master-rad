Shader "Custom/Cylinders"
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

            intersection closestCylinder(float3 ro, float3 rd, float4x4 cylinder)
            {
                float a1 = rd.x;
                float a2 = ro.x;
                float a3 = rd.y;
                float a4 = ro.y;
                float a5 = rd.z;
                float a6 = ro.z;
                float a7 = cylinder[0][3];
                float a8 = cylinder[0][0];
                float a9 = cylinder[0][3];
                float a10 = cylinder[0][2];
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
                float ymin = cylinder[0][1];
                float ymax = ymin + cylinder[1][0];
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

            float mapToZeroOne(float n, float mn, float mx){
                return (n-mn)/(mx-mn);
            }

            float3 map3ToZeroOne(float3 a){
                return float3(
                    mapToZeroOne(a.x, _areaMin.x, _areaMax.x),
                    mapToZeroOne(a.y, _areaMin.y, _areaMax.y),
                    mapToZeroOne(a.z, _areaMin.z, _areaMax.z)
                );
            }

            float denistyAtPoint(float3 p){
                float3 coords = map3ToZeroOne(p);
                float resol = 1 / (float)_DensityNoiseSize;
                float dx1 = fmod(coords.x, resol);
                float dx = dx1 / resol;
                float dy1 = fmod(coords.y, resol);
                float dy = dy1 / resol;
                float dz1 = fmod(coords.z, resol);
                float dz = dz1 / resol;
                float c000 = tex3D(_DensityNoise, coords);
                float c100 = tex3D(_DensityNoise, coords+float3(resol,0,0));
                float c001 = tex3D(_DensityNoise, coords+float3(0,0,resol));
                float c101 = tex3D(_DensityNoise, coords+float3(resol,0,resol));
                float c010 = tex3D(_DensityNoise, coords+float3(0,resol,0));
                float c110 = tex3D(_DensityNoise, coords+float3(resol,resol,0));
                float c011 = tex3D(_DensityNoise, coords+float3(0,resol,resol));
                float c111 = tex3D(_DensityNoise, coords+float3(resol,resol,resol));
                float c00 = c000 * (1-dx) + c100 * dx;
                float c01 = c001 * (1-dx) + c101 * dx;
                float c10 = c010 * (1-dx) + c110 * dx;
                float c11 = c011 * (1-dx) + c111 * dx;
                float c0 = c00 * (1-dy) + c10 * dy;
                float c1 = c01 * (1-dy) + c11 * dy;
                float val = tex3D(_DensityNoise, coords);
                val = c0 * (1-dz) + c1 * dz;
                return val;
                return val;
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
                    // intersection res = closestCylinder(ro, rd, _Cylinders[j]);
                    intersection res = closestCylinder(ro, rd, _CylinderMatrices[j]);
                    if(res.count>0){
                        hits++;
                        float4 p = res.first;
                        if(p.x == ro.x && p.y == ro.y && p.z == ro.z) p=res.second;
                        if(distance(ro, p)<closestDistance){
                            closest = res;
                            closestDistance = distance(ro, p);
                            closestIndex = j;
                        }
                    }
                }
                float4 skyColor = tex2D(_MainTex, i.uv);
                if(hits==0)return skyColor;
                float val = 0;
                float dv = rd*0.1f;
                for(int i=0; i<steps; i++){
                    float3 currPoint = closest.first.xyz + i * dv;
                    float den = denistyAtPoint(currPoint);
                    float4x4 cyl = _CylinderMatrices[closestIndex];
                    // float4 cyl = _Cylinders[closestIndex];
                    float halfheight = cyl[1][0]/2.0f;
                    float middle = cyl[0][1] + halfheight;
                    float dist = abs(currPoint.y - middle)/halfheight;
                    val += den;
                }
                val = val/steps;
                return float4(random3(closestIndex), 1.0f);
                return (1,1,1,1)*val+skyColor*(1-val);
                return float4(val, val, val, 1.0f);
                // float lengthInside = distance(closest.first.xyz,closest.second.xyz);
                // return float4(1-exp(-0.1*closestDistance), 1, lengthInside, 1.0);
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