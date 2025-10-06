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
        // Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // Incoming from script
            float4x4 _CamToWorld;
            float4x4 _CamInverseProjection;
            float3 _CamPos;
            static const int myArray[256] = {33, 99, 223, 71, 15, 82, 59, 137, 37, 192, 139, 44, 232, 189, 14, 103, 190, 227, 105, 35, 66, 185, 224, 80, 56, 95, 72, 86, 203, 236, 62, 151, 67, 114, 230, 197, 209, 84, 81, 73, 175, 202, 79, 93, 115, 52, 61, 91, 110, 2, 255, 94, 133, 48, 5, 125, 222, 145, 161, 0, 176, 20, 141, 97, 191, 243, 109, 205, 214, 183, 239, 140, 199, 122, 200, 162, 237, 228, 83, 143, 211, 53, 210, 134, 226, 194, 104, 154, 24, 136, 12, 242, 54, 129, 113, 32, 152, 246, 58, 158, 179, 213, 117, 147, 64, 245, 193, 102, 169, 173, 221, 50, 156, 9, 180, 57, 100, 101, 178, 92, 107, 108, 244, 34, 146, 198, 74, 40, 27, 212, 49, 65, 184, 220, 26, 30, 111, 204, 47, 31, 164, 75, 155, 132, 112, 43, 120, 208, 186, 215, 247, 87, 126, 89, 98, 144, 163, 229, 160, 76, 39, 207, 45, 166, 70, 106, 10, 250, 78, 174, 195, 28, 234, 6, 188, 55, 248, 187, 41, 127, 150, 19, 13, 42, 96, 168, 121, 181, 36, 85, 253, 18, 130, 235, 217, 135, 116, 51, 167, 219, 68, 249, 231, 77, 251, 218, 142, 69, 11, 148, 131, 22, 172, 153, 17, 119, 138, 170, 16, 63, 124, 128, 3, 216, 254, 182, 46, 252, 38, 118, 157, 123, 159, 165, 88, 238, 201, 7, 4, 60, 90, 196, 25, 21, 177, 171, 8, 149, 206, 240, 241, 233, 23, 1, 29, 225};

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

            // Signed Distance Functions
            float sdSphere(float3 p, float r) { return length(p) - r; }

            float SceneSDF(float3 p) { return sdSphere(p - float3(0,0,3), 1.0); }

            float3 getNormal(float3 p)
            {
                float e = 0.001;
                return normalize(float3(
                    SceneSDF(p + float3(e,0,0)) - SceneSDF(p - float3(e,0,0)),
                    SceneSDF(p + float3(0,e,0)) - SceneSDF(p - float3(0,e,0)),
                    SceneSDF(p + float3(0,0,e)) - SceneSDF(p - float3(0,0,e))
                ));
            }

            float fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }

            float3 fade3(float3 t) { return float3(fade(t.x), fade(t.y), fade(t.z)); }

            float grad(int hash, float3 p)
            {
                // Pick gradient direction from hash
                int h = hash & 15;
                float3 grads[16] = {
                    float3(1,1,0), float3(-1,1,0), float3(1,-1,0), float3(-1,-1,0),
                    float3(1,0,1), float3(-1,0,1), float3(1,0,-1), float3(-1,0,-1),
                    float3(0,1,1), float3(0,-1,1), float3(0,1,-1), float3(0,-1,-1),
                    float3(1,1,0), float3(-1,1,0), float3(0,-1,1), float3(0,-1,-1)
                };
                return dot(grads[h], p);
            }

            float lerp(float a, float b, float t) { return a + t * (b - a); }

            float permute(float x) { return fmod((34.0 * x + 1.0) * x, 289.0); }

            float perlin3D(float3 p)
            {
                // Floor and fractional part
                float3 Pi = floor(p);
                float3 Pf = p - Pi;

                // Fade curves
                float3 f = fade3(Pf);

                // Permutation hashing
                float A = permute(Pi.x) + Pi.y;
                float AA = permute(A) + Pi.z;
                float AB = permute(A + 1.0) + Pi.z;
                float B = permute(Pi.x + 1.0) + Pi.y;
                float BA = permute(B) + Pi.z;
                float BB = permute(B + 1.0) + Pi.z;

                // 8 corner gradients
                float v000 = grad((int)permute(AA), Pf);
                float v100 = grad((int)permute(BA), Pf - float3(1, 0, 0));
                float v010 = grad((int)permute(AB), Pf - float3(0, 1, 0));
                float v110 = grad((int)permute(BB), Pf - float3(1, 1, 0));
                float v001 = grad((int)permute(AA + 1.0), Pf - float3(0, 0, 1));
                float v101 = grad((int)permute(BA + 1.0), Pf - float3(1, 0, 1));
                float v011 = grad((int)permute(AB + 1.0), Pf - float3(0, 1, 1));
                float v111 = grad((int)permute(BB + 1.0), Pf - float3(1, 1, 1));

                // Trilinear interpolation
                float x1 = lerp(v000, v100, f.x);
                float x2 = lerp(v010, v110, f.x);
                float y1 = lerp(x1, x2, f.y);

                float x3 = lerp(v001, v101, f.x);
                float x4 = lerp(v011, v111, f.x);
                float y2 = lerp(x3, x4, f.y);

                return lerp(y1, y2, f.z);
            }

            // float perlin(float3 p){
            //     int X = int(floor(p.x)) & 255;
            //     int Y = int(floor(p.y)) & 255;
            //     int Z = int(floor(p.z)) & 255;
            //     float x = p.x - floor(p.x);
            //     float y = p.y - floor(p.y);
            //     float z = p.z - floor(p.z);
            //     float u = fade(x);
            //     float v = fade(y);
            //     float w = fade(z);
            //     int A  = myArray[X] + Y;
            //     int AA = myArray[A] + Z;
            //     int AB = myArray[A + 1] + Z;
            //     int B  = myArray[X + 1] + Y;
            //     int BA = myArray[B] + Z;
            //     int BB = myArray[B + 1] + Z;
            //     return lerp(
            //         lerp(
            //             lerp(grad(myArray[AA], x, y, z),
            //                 grad(myArray[BA], x - 1, y, z), u),
            //             lerp(grad(myArray[AB], x, y - 1, z),
            //                 grad(myArray[BB], x - 1, y - 1, z), u),
            //             v
            //         ),
            //         lerp(
            //             lerp(grad(myArray[AA + 1], x, y, z - 1),
            //                 grad(myArray[BA + 1], x - 1, y, z - 1), u),
            //             lerp(grad(myArray[AB + 1], x, y - 1, z - 1),
            //                 grad(myArray[BB + 1], x - 1, y - 1, z - 1), u),
            //             v
            //         ),
            //         w
            //     );
            // }

            float4 raymarch(float3 ro, float3 rd, float3 lightPos)
            {
                float t = 0.0;
                const int MAX_STEPS = 128;
                const float MAX_DIST = 50.0;
                const float EPSILON = 0.001;

                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = ro + rd * t;
                    float d = SceneSDF(p);
                    if (d < EPSILON)
                    {
                        float3 n = getNormal(p);
                        float3 lightDir = normalize(lightPos);
                        float diff = max(0, dot(n, lightDir));
                        const float INTERNAL_STEP = 0.1;
                        float3 ir = p;
                        float res = 0;
                        float count = 0;
                        for(int j = 0; j<MAX_STEPS; j++){    
                            float v1 = perlin3D(ir);
                            float v2 = perlin3D(ir*2.0);
                            float v3 = perlin3D(ir*4.0);
                            float v4 = perlin3D(ir*8.0);
                            float v5 = perlin3D(ir*16.0);
                            float v = min(v1/16.0+v2/16.0+v3/8.0+v4/4.0+v5/2.0,1.0);
                            res += v;
                            count = count + 1.0;
                            ir = ir + rd*INTERNAL_STEP; 
                            float id = SceneSDF(ir);
                            if(id>EPSILON) break;
                        }
                        return float4(diff.xxx ,res/count);
                    }
                    t += d;
                    if (t > MAX_DIST) break;
                }
                return float4(0.0, 0.0, 0.0, 0.0); // background color
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert screen UV to NDC (-1..1)
                float2 uv = i.uv * 2.0 - 1.0;

                // Clip-space ray
                float4 rayClip = float4(uv, -1.0, 1.0);

                // View-space ray
                float4 rayView = mul(_CamInverseProjection, rayClip);
                rayView /= rayView.w;

                // World-space ray
                float3 rd = normalize(mul(_CamToWorld, float4(rayView.xyz, 0.0)).xyz);
                float3 ro = _CamPos;
                float xtime = sin(_Time.y);
                float ytime = cos(_Time.y);
                float4 col = raymarch(ro, rd, float3(xtime, ytime, -0.8));
                float4 sceneCol = tex2D(_MainTex, i.uv);
                float alfa = col.w;
                // return fixed4(1.0,1.0,1.0,v);
                // if(alfa==0) return sceneCol;
                // return col;
                float r = (alfa) * col.x + (1-alfa) * sceneCol.x;
                float g = (alfa) * col.y + (1-alfa) * sceneCol.y;
                float b = (alfa) * col.z + (1-alfa) * sceneCol.z;
                fixed4 res = alfa*col+(1-alfa)*sceneCol;
                res.w=alfa;
                return fixed4(r,g,b,1.0);
            }
            ENDCG
        }
    }
}