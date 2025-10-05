Shader "Custom/Raymarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CameraFOV ("Camera FOV", Float) = 60
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _CameraFOV;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // [-1,1] screen coords
                return o;
            }

            // ------------------------------
            // Signed Distance Functions (SDF)
            // ------------------------------
            float sdSphere(float3 p, float r)
            {
                return length(p) - r;
            }

            float SceneSDF(float3 p)
            {
                // Sphere centered at (0,0,3), radius 1
                return sdSphere(p - float3(0,0,3), 1.0);
            }

            // Approximate normal via gradient of SDF
            float3 getNormal(float3 p)
            {
                float e = 0.001;
                float3 n = float3(
                    SceneSDF(p + float3(e,0,0)) - SceneSDF(p - float3(e,0,0)),
                    SceneSDF(p + float3(0,e,0)) - SceneSDF(p - float3(0,e,0)),
                    SceneSDF(p + float3(0,0,e)) - SceneSDF(p - float3(0,0,e))
                );
                return normalize(n);
            }

            // ------------------------------
            // Raymarching loop
            // ------------------------------
            float3 raymarch(float3 ro, float3 rd)
            {
                float t = 0.0;
                const int MAX_STEPS = 128;
                const float MAX_DIST = 100.0;
                const float EPSILON = 0.001;

                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = ro + rd * t;
                    float d = SceneSDF(p);
                    if (d < EPSILON)
                    {
                        // Hit -> lighting
                        float3 n = getNormal(p);
                        float3 lightDir = normalize(float3(0.5, 1.0, -0.3));
                        float diff = max(0, dot(n, lightDir));
                        return diff.xxx; // grayscale sphere
                    }
                    t += d;
                    if (t > MAX_DIST) break;
                }
                // Background color
                return float3(1.0, 0.0, 0.0);
            }

            // ------------------------------
            // Fragment Shader
            // ------------------------------
            fixed4 frag (v2f i) : SV_Target
            {
                // Camera setup
                float3 ro = _WorldSpaceCameraPos;

                // Convert screen uv into ray direction
                float3 forward = UNITY_MATRIX_V[2].xyz * -1.0;
                float3 right = UNITY_MATRIX_V[0].xyz;
                float3 up = UNITY_MATRIX_V[1].xyz;

                float aspect = _ScreenParams.x / _ScreenParams.y;
                float fovScale = tan(radians(_CameraFOV * 0.5));

                float3 rd = normalize(forward + i.uv.x * aspect * fovScale * right + i.uv.y * fovScale * up);

                float3 col = raymarch(ro, rd);
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}