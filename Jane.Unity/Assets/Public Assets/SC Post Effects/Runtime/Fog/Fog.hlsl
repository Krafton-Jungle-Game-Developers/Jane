#if !defined(SHADERGRAPH_PREVIEW)
//#define REQUIRE_DEPTH

#if !defined(PIPELINE_INCLUDED)
#include "../../Shaders/Pipeline/Pipeline.hlsl"
#endif

//Uncomment to disable feature that definitely aren't used
#define ENABLE_GRADIENT
#define ENABLE_SKYBOXCOLOR
#define ENABLE_HEIGHTNOISE
#define ENABLE_DIRECTIONAL_COLOR

DECLARE_TEX(_NoiseTex);
DECLARE_TEX(_ColorGradient);
DECLARE_RT(_SkyboxTex);
#endif

int4 _SceneFogMode;
float4 _HeightParams;
float4 _DistanceParams;
float4 _SceneFogParams;
float4 _DensityParams;
float4 _NoiseParams;
float4 _FogColor;
float4 _SkyboxParams;
//X: Influence
//Y: Mip level
float4 _DirLightParams;
//XYZ: Direction
//W: Intensity
float4 _DirLightColor; //(a=free)
uniform half _FarClippingPlane;	//Used for gradient distance

half ComputeFactor(float coord)
{
    float fogFac = 0.0;
    if (_SceneFogMode.x == 1) // linear
        {
        // factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
        fogFac = coord * _SceneFogParams.z + _SceneFogParams.w;
        }
    if (_SceneFogMode.x == 2) // exp
        {
        // factor = exp(-density*z)
        fogFac = _SceneFogParams.y * coord; fogFac = exp2(-fogFac);
        }
    if (_SceneFogMode.x == 3) // exp2
        {
        // factor = exp(-(density*z)^2)
        fogFac = _SceneFogParams.x * coord; fogFac = exp2(-fogFac * fogFac);
        }
    return saturate(fogFac);
}

float ComputeDistance(float3 wpos, float depth)
{
    float3 wsDir = _WorldSpaceCameraPos.xyz - wpos;
    float dist;
    //Radial distance
    if (_SceneFogMode.y == 1)
        dist = length(wsDir);
    else
        dist = depth * _ProjectionParams.z;
    //Start distance
    dist -= _ProjectionParams.y;
    return dist;
}

//Use unique name, may clash with other fog assets
float ComputeHeightFogSCPE(float3 wpos)
{
    float3 wsDir = _WorldSpaceCameraPos.xyz - wpos;
    float FH = _HeightParams.x;
    float3 C = _WorldSpaceCameraPos;
    float3 V = wsDir;
    float3 P = wpos;
    float3 aV = _HeightParams.w * V;
    float FdotC = _HeightParams.y;
    float k = _HeightParams.z;
    float FdotP = P.y - FH;
    float FdotV = wsDir.y;
    float c1 = k * (FdotP + FdotC);
    float c2 = (1 - 2 * k) * FdotP;
    float g = min(c2, 0.0);
    g = -length(aV) * (c1 - g * g / abs(FdotV + 1.0e-5f));
    return g;
}

float GetFogDistance(float3 worldPos, float clipZ)
{
    //Distance fog
    float distanceFog = 0;
    float distanceWeight = 0;
    if (_DistanceParams.z == 1) {
        distanceFog = ComputeDistance(worldPos, clipZ);

        //Density (separated so it doesn't affect the UV of a gradient texture)
        distanceWeight = distanceFog * _DensityParams.x;
    }

    return distanceWeight;
}

float4 GetFogColor(float2 screenPos, float3 worldPos, float distanceFactor)
{
    #if !defined(SHADERGRAPH_PREVIEW)
    float4 fogColor = _FogColor.rgba;
#ifdef ENABLE_GRADIENT
    if (_SceneFogMode.z == 1) //Gradient
    {
        fogColor = SAMPLE_TEX(_ColorGradient, Clamp, float2(distanceFactor / _FarClippingPlane, 0));
    }
#endif
#ifdef ENABLE_SKYBOXCOLOR
    if (_SceneFogMode.z == 2) //Skybox
    {
        fogColor = SAMPLE_RT_LOD(_SkyboxTex, Clamp, screenPos, 0.0); //Mip not used
    }
#endif

#ifdef ENABLE_DIRECTIONAL_COLOR
    float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
    float NdotL = saturate(dot(-viewDir, _DirLightParams.xyz));
    fogColor.rgb = lerp(fogColor.rgb, _DirLightColor.rgb * _DirLightParams.w, saturate(NdotL * _DirLightParams.w));
#endif

    return fogColor;
    #else
    return 1;
    #endif
}

float GetFogHeight(float3 worldPos, float time, float skyboxMask)
{
    #if !defined(SHADERGRAPH_PREVIEW)
    //Height fog
    float heightFog = 0;
    float heightWeight = 0;
    if (_DistanceParams.w == 1) { //Heightfog enabled
        float noise = 1;
#ifdef ENABLE_HEIGHTNOISE
        if (_SceneFogMode.w == 1) //Noise enabled
        {
            float noise1 = SAMPLE_TEX(_NoiseTex, Repeat, worldPos.xz * _NoiseParams.x + (time * _NoiseParams.y * float2(0, 1))).r;
            float noise2 = SAMPLE_TEX(_NoiseTex, Repeat, worldPos.xz * _NoiseParams.x * 0.5 + (time * _NoiseParams.y * 0.8 * float2(0, 1))).r;
            
            noise = lerp(1, max(noise1, noise2), _DensityParams.y * skyboxMask);
        }
#endif
        heightFog = ComputeHeightFogSCPE(worldPos);
        heightWeight += heightFog * noise;
    }

    return heightWeight;
    #else
    return 1.0;
    #endif
}

//Note: clipZ should be linear depth when used in screen-space
float GetFogDensity(float3 worldPos, float linearDepth, float time, float skyboxMask)
{
    #if !defined(SHADERGRAPH_PREVIEW)
    //Fog start distance
    float g = _DistanceParams.x;

    g += GetFogDistance(worldPos, linearDepth);
    g += GetFogHeight(worldPos, time, skyboxMask);

    //Fog density (Linear/Exp/ExpSqr)
    half fogFac = ComputeFactor(max(0.0, g));

    return fogFac;
    #else
    return 1.0;
    #endif
}

//Shader Graph subgraph functions

void ApplyFog_float(in float3 worldPos, in float3 worldNormal, in float4 screenPos, in float time, in float4 color, in float3 emission, out float4 outColor, out float3 outEmission)
{
    float g = _DistanceParams.x;
    float distanceFog = GetFogDistance(worldPos, 1.0);
    g += distanceFog;
    float heightFog = GetFogHeight(worldPos, time, 1.0);
    g += heightFog;
    float fogFactor = ComputeFactor(max(0.0, g));
    
    float4 fogColor = GetFogColor(screenPos.xy, worldPos, distanceFog);
    
    fogColor.a = saturate(fogFactor);
    
    outEmission = lerp(fogColor.rgb, emission, fogFactor);
    //Blend to black color so lighting gets nullified
    outColor = lerp(0, color, fogFactor);
}

//Shader graph, multiply result with final alpha to blend transparent materials into the fog
void GetFogAlpha_float(in float3 worldPos, out float factor)
{
    #ifdef URP
    #if !defined(SHADERGRAPH_PREVIEW)
    factor = saturate(GetFogDensity(worldPos, 1.0, _TimeParameters.x, 1.0));
    #else
    factor = 1.0;
    #endif
    #endif
}
