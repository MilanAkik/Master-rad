Shader "Custom/Cylinders"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DensityNoise ("Density noise 3d texture", 3D) = "white" {}
        _CloudColor ("Base cloud color (c_ss)", Color) = (1, 1, 1, 1)
        _PrimaryStepLength ("Primary step length (l_g)", Float) = 0.1
        _ShadowStepLength ("Shadow step length (l_p)", Float) = 0.1
        _IsotropicCoefficient ("Isotropic coefficient (s)", Float) = 1.0
        [Toggle(_MULTI_CYLINDER)] _MultiCylinder ("March multiple cylinders", Float) = 0
        _MaxCylinderSegments ("Maximum cylinder segments", Range(1, 32)) = 8
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
            #pragma target 3.5
            #pragma shader_feature_local_fragment _MULTI_CYLINDER
            #include "UnityCG.cginc"		
            #include "noiseSimplex.cginc"

            #define MAX_PRIMARY_MARCH_STEPS 128
            #define MAX_SHADOW_MARCH_STEPS 128
            #define MAX_CYLINDER_SEGMENTS 32
            #define CYLINDER_MASK_WORD_COUNT 16
            
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

            //Area parameters
            float3 _AreaMin;
            float3 _AreaMax;

            //Density noise parameters
            sampler3D _DensityNoise;
            int _DensityNoiseSize;

            // Single-scattering parameters
            float4 _CloudColor;
            float _PrimaryStepLength;
            float _ShadowStepLength;
            float _IsotropicCoefficient;
            float _MaxCylinderSegments;

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
            
            float densityAtPoint(float3 p)
            {
                float3 extent = _AreaMax - _AreaMin;
                float3 normalizedPosition = saturate((p - _AreaMin) / extent);
                float size = (float)_DensityNoiseSize;
                // Map areaMin/areaMax to the centers of the first/last texels.
                float3 uvw = (normalizedPosition * (size - 1.0f) + 0.5f) / size;
                return tex3D(_DensityNoise, uvw).r;
            }

            // Beer-Lambert transmittance for one primary-ray step: e^(-d_x * l_g).
            float primaryStepTransmittance(float density, float primaryStepLength)
            {
                return exp(-density * primaryStepLength);
            }

            // Alpha from equation (1), represented incrementally instead of as a product loop.
            float backgroundOcclusionFactor(float previousOcclusionFactor, float density, float primaryStepLength)
            {
                return previousOcclusionFactor * primaryStepTransmittance(density, primaryStepLength);
            }

            // The final factor in equation (2): 1 - e^(-d_x * l_g).
            float primaryStepAbsorption(float density, float primaryStepLength)
            {
                return 1.0 - primaryStepTransmittance(density, primaryStepLength);
            }

            // One term of the shadow-ray optical-depth sum: d_j * l_p.
            float shadowOpticalDepthContribution(float density, float shadowStepLength)
            {
                return density * shadowStepLength;
            }

            // Equation (4), accumulated one shadow-ray sample at a time.
            float accumulateShadowOpticalDepth(float currentOpticalDepth, float density, float shadowStepLength)
            {
                return currentOpticalDepth + shadowOpticalDepthContribution(density, shadowStepLength);
            }

            // Distance from a point inside a finite vertical cylinder to its boundary.
            // This handles vertical rays explicitly, unlike the general camera-ray intersection.
            float distanceToCylinderExitFromInside(float3 rayOrigin, float3 rayDirection, float4x4 cylinder)
            {
                const float directionEpsilon = 0.000001;
                const float maximumDistance = 1e20;

                float centerX = cylinder[0][0];
                float minimumY = cylinder[0][1];
                float centerZ = cylinder[0][2];
                float radius = cylinder[0][3];
                float maximumY = minimumY + cylinder[1][0];

                float sideExitDistance = maximumDistance;
                float radialDirectionLengthSquared =
                    rayDirection.x * rayDirection.x + rayDirection.z * rayDirection.z;

                if (radialDirectionLengthSquared > directionEpsilon)
                {
                    float relativeX = rayOrigin.x - centerX;
                    float relativeZ = rayOrigin.z - centerZ;
                    float halfB = relativeX * rayDirection.x + relativeZ * rayDirection.z;
                    float c = relativeX * relativeX + relativeZ * relativeZ - radius * radius;
                    float discriminant = max(
                        halfB * halfB - radialDirectionLengthSquared * c,
                        0.0);

                    // For an origin inside the cylinder, the larger root is the forward exit.
                    sideExitDistance =
                        (-halfB + sqrt(discriminant)) / radialDirectionLengthSquared;
                }

                float capExitDistance = maximumDistance;
                if (rayDirection.y > directionEpsilon)
                {
                    capExitDistance = (maximumY - rayOrigin.y) / rayDirection.y;
                }
                else if (rayDirection.y < -directionEpsilon)
                {
                    capExitDistance = (minimumY - rayOrigin.y) / rayDirection.y;
                }

                return max(0.0, min(sideExitDistance, capExitDistance));
            }

            // Marches the shadow ray only through the cylinder containing the primary sample.
            float shadowRayOpticalDepth(
                float3 primarySamplePoint,
                float3 lightPosition,
                float4x4 cylinder,
                float requestedShadowStepLength)
            {
                const float minimumLength = 0.0001;

                float3 pointToLight = lightPosition - primarySamplePoint;
                float lightDistance = length(pointToLight);
                if (lightDistance <= minimumLength)
                {
                    return 0.0;
                }

                float3 shadowRayDirection = pointToLight / lightDistance;
                float cylinderExitDistance = distanceToCylinderExitFromInside(
                    primarySamplePoint,
                    shadowRayDirection,
                    cylinder);
                float marchDistance = min(lightDistance, cylinderExitDistance);
                if (marchDistance <= minimumLength)
                {
                    return 0.0;
                }

                float targetStepLength = max(requestedShadowStepLength, minimumLength);
                int shadowStepCount = (int)min(
                    ceil(marchDistance / targetStepLength),
                    (float)MAX_SHADOW_MARCH_STEPS);
                float shadowStepLength = marchDistance / (float)shadowStepCount;
                float opticalDepth = 0.0;

                [loop]
                for (int shadowStepIndex = 0;
                    shadowStepIndex < MAX_SHADOW_MARCH_STEPS;
                    shadowStepIndex++)
                {
                    if (shadowStepIndex >= shadowStepCount)
                    {
                        break;
                    }

                    float distanceAlongShadowRay =
                        ((float)shadowStepIndex + 0.5) * shadowStepLength;
                    float3 shadowSamplePoint =
                        primarySamplePoint + shadowRayDirection * distanceAlongShadowRay;
                    float shadowDensity = densityAtPoint(shadowSamplePoint);
                    opticalDepth = accumulateShadowOpticalDepth(
                        opticalDepth,
                        shadowDensity,
                        shadowStepLength);
                }

                return opticalDepth;
            }

            // The phase parameter supplied by equation (3): d_p = 10 / t_xss.
            float phaseParameterFromOpticalDepth(float shadowOpticalDepth)
            {
                const float minimumOpticalDepth = 0.0001;
                return 10.0 / max(shadowOpticalDepth, minimumOpticalDepth);
            }

            // Equation (5): modified approximate Lorenz-Mie phase function.
            float approximateLorenzMiePhase(float viewLightDot, float phaseParameter)
            {
                float angularFactor = saturate((1.0 + viewLightDot) * 0.5);
                return (phaseParameter / (4.0 * UNITY_PI)) * pow(angularFactor, phaseParameter);
            }

            // Equation (6), preserving the requested positive exponent in its second term.
            float isotropicLightFactor(float shadowOpticalDepth, float isotropicCoefficient)
            {
                float firstTerm = exp(-shadowOpticalDepth);
                float secondTerm = isotropicCoefficient * exp(shadowOpticalDepth / 10.0);
                float thirdTerm = (2.0 * isotropicCoefficient / 5.0) * exp(-shadowOpticalDepth / 50.0);
                return firstTerm + secondTerm + thirdTerm;
            }

            // Equation (3): light intensity arriving along the shadow ray.
            float incomingSingleScatteredLight(float viewLightDot, float shadowOpticalDepth, float isotropicCoefficient)
            {
                float phaseParameter = phaseParameterFromOpticalDepth(shadowOpticalDepth);
                float directionalLight = approximateLorenzMiePhase(viewLightDot, phaseParameter);
                float isotropicLight = isotropicLightFactor(shadowOpticalDepth, isotropicCoefficient);
                return directionalLight + isotropicLight;
            }

            // One term of the color sum in equation (2).
            float3 singleScatteringColorContribution(float backgroundOcclusion, float3 baseCloudColor, float incomingLight, float density, float primaryStepLength)
            {
                float absorbedLight = primaryStepAbsorption(density, primaryStepLength);
                return backgroundOcclusion * baseCloudColor * incomingLight * absorbedLight;
            }

            bool isUsableCylinderIntersection(intersection cylinderIntersection)
            {
                return cylinderIntersection.count == 2 &&
                    distance(cylinderIntersection.first.xyz, cylinderIntersection.second.xyz) > 0.0001;
            }

            // Returns accumulated cloud light in RGB and remaining background visibility in A.
            float4 marchSingleScatteringThroughCylinder(
                float3 primaryRayDirection,
                intersection cylinderIntersection,
                float4x4 cylinder,
                float initialBackgroundVisibility)
            {
                const float minimumLength = 0.0001;
                const float minimumDensity = 0.0001;
                const float minimumBackgroundVisibility = 0.05;

                if (cylinderIntersection.count < 2)
                {
                    return float4(0.0, 0.0, 0.0, initialBackgroundVisibility);
                }

                float3 marchStart = cylinderIntersection.first.xyz;
                float3 marchEnd = cylinderIntersection.second.xyz;
                float marchDistance = distance(marchStart, marchEnd);
                if (marchDistance <= minimumLength)
                {
                    return float4(0.0, 0.0, 0.0, initialBackgroundVisibility);
                }

                float targetStepLength = max(_PrimaryStepLength, minimumLength);
                int primaryStepCount = (int)min(
                    ceil(marchDistance / targetStepLength),
                    (float)MAX_PRIMARY_MARCH_STEPS);
                float primaryStepLength = marchDistance / (float)primaryStepCount;
                float backgroundVisibility = initialBackgroundVisibility;
                float3 accumulatedCloudColor = float3(0.0, 0.0, 0.0);

                [loop]
                for (int primaryStepIndex = 0;
                    primaryStepIndex < MAX_PRIMARY_MARCH_STEPS;
                    primaryStepIndex++)
                {
                    if (primaryStepIndex >= primaryStepCount)
                    {
                        break;
                    }

                    float distanceAlongPrimaryRay =
                        ((float)primaryStepIndex + 0.5) * primaryStepLength;
                    float3 primarySamplePoint =
                        marchStart + primaryRayDirection * distanceAlongPrimaryRay;
                    float density = densityAtPoint(primarySamplePoint);

                    if (density <= minimumDensity)
                    {
                        continue;
                    }

                    float shadowOpticalDepth = shadowRayOpticalDepth(
                        primarySamplePoint,
                        _LightPosition,
                        cylinder,
                        _ShadowStepLength);

                    float3 sampleToLight = _LightPosition - primarySamplePoint;
                    float sampleToLightDistance = length(sampleToLight);
                    float3 shadowRayDirection = sampleToLightDistance > minimumLength
                        ? sampleToLight / sampleToLightDistance
                        : primaryRayDirection;
                    float viewLightDot = dot(primaryRayDirection, shadowRayDirection);
                    float incomingLight = incomingSingleScatteredLight(
                        viewLightDot,
                        shadowOpticalDepth,
                        _IsotropicCoefficient);

                    // Equation (1) defines alpha_x through the current sample, so update it
                    // before evaluating the corresponding term of equation (2).
                    backgroundVisibility = backgroundOcclusionFactor(
                        backgroundVisibility,
                        density,
                        primaryStepLength);
                    accumulatedCloudColor += singleScatteringColorContribution(
                        backgroundVisibility,
                        _CloudColor.rgb * _LightColor.rgb,
                        incomingLight,
                        density,
                        primaryStepLength);

                    // Once at least 95% of the background is occluded, later samples have
                    // little visible influence and can be skipped.
                    if (backgroundVisibility <= minimumBackgroundVisibility)
                    {
                        break;
                    }
                }

                return float4(accumulatedCloudColor, backgroundVisibility);
            }

            #if !defined(_MULTI_CYLINDER)
            // Finds and marches only the closest cylinder intersected by this camera ray.
            float4 marchSingleScatteringThroughClosestCylinder(
                float3 primaryRayOrigin,
                float3 primaryRayDirection)
            {
                int closestCylinderIndex = -1;
                float closestDistance = 1e20;
                intersection closestIntersection;

                [loop]
                for (int cylinderIndex = 0;
                    cylinderIndex < _CylinderCount;
                    cylinderIndex++)
                {
                    intersection candidateIntersection = closestCylinder(
                        primaryRayOrigin,
                        primaryRayDirection,
                        _CylinderMatrices[cylinderIndex]);
                    if (!isUsableCylinderIntersection(candidateIntersection))
                    {
                        continue;
                    }

                    float candidateDistance = distance(
                        primaryRayOrigin,
                        candidateIntersection.first.xyz);
                    if (candidateDistance < closestDistance)
                    {
                        closestDistance = candidateDistance;
                        closestCylinderIndex = cylinderIndex;
                        closestIntersection = candidateIntersection;
                    }
                }

                if (closestCylinderIndex < 0)
                {
                    return float4(0.0, 0.0, 0.0, 1.0);
                }

                return marchSingleScatteringThroughCylinder(
                    primaryRayDirection,
                    closestIntersection,
                    _CylinderMatrices[closestCylinderIndex],
                    1.0);
            }
            #endif

            #if defined(_MULTI_CYLINDER)
            // Builds a compact 512-bit set of cylinders intersecting this camera ray, then
            // repeatedly selects the closest remaining segment. Intersections are recalculated
            // only for cylinders whose candidate bit is still set.
            float4 marchSingleScatteringThroughMultipleCylinders(
                float3 primaryRayOrigin,
                float3 primaryRayDirection)
            {
                const float cursorOffset = 0.0001;
                const float minimumBackgroundVisibility = 0.05;

                uint candidateMask[CYLINDER_MASK_WORD_COUNT];
                [unroll]
                for (int maskWordIndex = 0;
                    maskWordIndex < CYLINDER_MASK_WORD_COUNT;
                    maskWordIndex++)
                {
                    candidateMask[maskWordIndex] = 0u;
                }

                [loop]
                for (int cylinderIndex = 0; cylinderIndex < _CylinderCount; cylinderIndex++)
                {
                    intersection candidateIntersection = closestCylinder(
                        primaryRayOrigin,
                        primaryRayDirection,
                        _CylinderMatrices[cylinderIndex]);
                    if (isUsableCylinderIntersection(candidateIntersection))
                    {
                        int maskWordIndex = cylinderIndex >> 5;
                        uint maskBit = 1u << (cylinderIndex & 31);
                        candidateMask[maskWordIndex] |= maskBit;
                    }
                }

                int maximumSegments = (int)clamp(
                    _MaxCylinderSegments,
                    1.0,
                    (float)MAX_CYLINDER_SEGMENTS);
                float3 rayCursor = primaryRayOrigin;
                float backgroundVisibility = 1.0;
                float3 accumulatedCloudColor = float3(0.0, 0.0, 0.0);

                [loop]
                for (int segmentIndex = 0;
                    segmentIndex < MAX_CYLINDER_SEGMENTS;
                    segmentIndex++)
                {
                    if (segmentIndex >= maximumSegments ||
                        backgroundVisibility <= minimumBackgroundVisibility)
                    {
                        break;
                    }

                    int closestCylinderIndex = -1;
                    float closestDistance = 1e20;
                    intersection closestIntersection;

                    [loop]
                    for (int cylinderIndex = 0;
                        cylinderIndex < _CylinderCount;
                        cylinderIndex++)
                    {
                        int maskWordIndex = cylinderIndex >> 5;
                        uint maskBit = 1u << (cylinderIndex & 31);
                        if ((candidateMask[maskWordIndex] & maskBit) == 0u)
                        {
                            continue;
                        }

                        intersection candidateIntersection = closestCylinder(
                            rayCursor,
                            primaryRayDirection,
                            _CylinderMatrices[cylinderIndex]);
                        if (!isUsableCylinderIntersection(candidateIntersection))
                        {
                            candidateMask[maskWordIndex] &= ~maskBit;
                            continue;
                        }

                        float candidateDistance = distance(
                            rayCursor,
                            candidateIntersection.first.xyz);
                        if (candidateDistance < closestDistance)
                        {
                            closestDistance = candidateDistance;
                            closestCylinderIndex = cylinderIndex;
                            closestIntersection = candidateIntersection;
                        }
                    }

                    if (closestCylinderIndex < 0)
                    {
                        break;
                    }

                    float4 segmentScattering = marchSingleScatteringThroughCylinder(
                        primaryRayDirection,
                        closestIntersection,
                        _CylinderMatrices[closestCylinderIndex],
                        backgroundVisibility);
                    accumulatedCloudColor += segmentScattering.rgb;
                    backgroundVisibility = segmentScattering.a;

                    int closestMaskWordIndex = closestCylinderIndex >> 5;
                    uint closestMaskBit = 1u << (closestCylinderIndex & 31);
                    candidateMask[closestMaskWordIndex] &= ~closestMaskBit;
                    rayCursor = closestIntersection.second.xyz +
                        primaryRayDirection * cursorOffset;
                }

                return float4(accumulatedCloudColor, backgroundVisibility);
            }
            #endif

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

            float4 frag(v2f i) : SV_Target
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

                float4 skyColor = tex2D(_MainTex, i.uv);
                float4 scattering;

                #if defined(_MULTI_CYLINDER)
                scattering = marchSingleScatteringThroughMultipleCylinders(ro, rd);
                #else
                scattering = marchSingleScatteringThroughClosestCylinder(ro, rd);
                #endif

                float3 finalColor = scattering.rgb + skyColor.rgb * scattering.a;
                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
