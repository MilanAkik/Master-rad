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
                        return float4(diff.xxx, 1.0);
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
                fixed4 res = alfa*col+(1-alfa)*sceneCol;
                res.w=alfa;
                // float v1 = perlin3D(float3(i.uv.x, i.uv.y, ytime)*32.0);
                // if(v1<-0.8) return float4(0.1,0.1,0.1,1.0);
                // if(v1<-0.6) return float4(0.2,0.2,0.2,1.0);
                // if(v1<-0.4) return float4(0.3,0.3,0.3,1.0);
                // if(v1<-0.2) return float4(0.4,0.4,0.4,1.0);
                // if(v1< 0.0) return float4(0.5,0.5,0.5,1.0);
                // if(v1< 0.2) return float4(0.6,0.6,0.6,1.0);
                // if(v1< 0.4) return float4(0.7,0.7,0.7,1.0);
                // if(v1< 0.6) return float4(0.8,0.8,0.8,1.0);
                // if(v1< 0.8) return float4(0.9,0.9,0.9,1.0);
                // return float4(0.1,0.1,0.1,v1);
                return res;
            }
            ENDCG
        }
    }
}