#ifndef UNITY_GRAPHFUNCTIONS_LW_INCLUDED //Shader Graph
#endif
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#if !defined(SHADERGRAPH_PREVIEW)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

void GetSourceImage_float(float2 uv, out float4 color)
{
    color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
}

void ViewSpacePosition_float(float4 uv, float rawDepth, out float3 wPos)
{
    float4 viewPos = float4((uv.xy/uv.w) * 2.0 - 1.0, rawDepth, 1.0);
    float4 viewWorld = mul(UNITY_MATRIX_I_VP, viewPos);
    wPos = viewWorld.xyz / viewWorld.w;
}

void GetScreenLuminance_float(float4 color, out float luminance)
{
    luminance = (color.r * 0.3 + color.g * 0.59 + color.b * 0.11);
}

void DistanceFactor_float(float3 wPos, float start, float end, out float factor)
{
    float pixelDist = length(_WorldSpaceCameraPos.xyz - wPos.xyz);

    //Distance based scalar
    factor = saturate((end - pixelDist ) / (end-start));
}

void LinearDepthFade_float(float linearDepth, float start, float end, out float fadeFactor)
{
    float rawDepth = (linearDepth * _ProjectionParams.z) - _ProjectionParams.y;;
    float orthoDist = _ProjectionParams.z - ((_ZBufferParams.z * (1.0 - rawDepth) + _ZBufferParams.w) * _ProjectionParams.w);

    //Non-linear depth values
    float dist = lerp(rawDepth, orthoDist, unity_OrthoParams.w);
	
    fadeFactor = 1-saturate((end - dist) / (end-start));
}