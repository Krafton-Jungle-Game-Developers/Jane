// SC Post Effects
// Staggart Creations
// http://staggart.xyz
// Copyright protected under Unity Asset Store EULA

//Libraries
#define URP
#ifndef UNITY_GRAPHFUNCTIONS_LW_INCLUDED //Shader Graph
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif
//No longer included, some content is explicitly declare now (URP 10+)
//#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "../Blending.hlsl"

SAMPLER(sampler_LinearClamp);
SAMPLER(sampler_LinearRepeat);
#define Clamp sampler_LinearClamp
#define Repeat sampler_LinearRepeat

#ifndef UNITY_GRAPHFUNCTIONS_LW_INCLUDED //Shader Graph
#define _BlitTex _MainTex //Ensures multipass effects work using built-in blit
TEXTURE2D_X(_MainTex); /* Always included */
SAMPLER(sampler_MainTex);
//#define sampler_MainTex sampler_LinearClamp
float4 _MainTex_TexelSize;
#endif

#if defined(REQUIRE_DEPTH_NORMALS) || _RECONSTRUCT_NORMAL
#define REQUIRE_DEPTH
#endif

#ifdef REQUIRE_DEPTH
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#define SAMPLE_DEPTH(uv) SampleSceneDepth(uv) //Use function from DeclareDepthTexture, uses a float sampler
#define LINEAR_DEPTH(depth) Linear01Depth(depth, _ZBufferParams)
#define LINEAR_EYE_DEPTH(depth) LinearEyeDepth(depth, _ZBufferParams)
#endif

#ifdef REQUIRE_DEPTH_NORMALS
#if (SHADER_LIBRARY_VERSION_MAJOR < 10)
//Depth normals pass not available
#define _RECONSTRUCT_NORMAL 1
#endif

#if _RECONSTRUCT_NORMAL
TEXTURE2D_X(_CameraDepthNormalsTexture);
#define SAMPLE_DEPTH_NORMALS(uv) SAMPLE_RT(_CameraDepthNormalsTexture, Clamp, uv)
#else
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
#define SAMPLE_DEPTH_NORMALS(uv) SampleDepthNormals(uv)

float4 SampleDepthNormals(float2 uv)
{
	//Mirror behavior of Built-in RP, where linear depth is stored in .W component
	float3 normals = SampleSceneNormals(uv);
	normals = float3(1-normals.x * 2 - 1, normals.y * 2 - 1, 0);
	float linearDepth = LINEAR_DEPTH(SAMPLE_DEPTH(uv));

	return float4(normals, linearDepth);
}

#endif
#endif

//Tex sampling
#define DECLARE_RT(textureName) TEXTURE2D_X(textureName);
#define DECLARE_TEX(textureName) TEXTURE2D(textureName);
#define SAMPLE_RT(textureName, samplerName, uv) SAMPLE_TEXTURE2D_X(textureName, samplerName, uv)
#define SAMPLE_RT_LOD(textureName, samplerName, uv, mip) SAMPLE_TEXTURE2D_X_LOD(textureName, samplerName, uv, mip)
#define SAMPLE_TEX(textureName, samplerName, uv) SAMPLE_TEXTURE2D_LOD(textureName, samplerName, uv, 0)

//Param and Arg usage is swapped
#define TEX_PARAM(textureName, samplerName) TEXTURE2D_ARGS(textureName, samplerName)
#define TEX_ARG(textureName, samplerName) TEXTURE2D_PARAM(textureName, samplerName)
#define RT_PARAM(textureName, samplerName) TEXTURE2D_X_ARGS(textureName, samplerName)
#define RT_ARG(textureName, samplerName) TEXTURE2D_X_PARAM(textureName, samplerName)

#ifndef UNITY_GRAPHFUNCTIONS_LW_INCLUDED //Shader Graph
#define SCREEN_COLOR(uv) SAMPLE_RT(_MainTex, Clamp, uv);
//Shorthand for sampling MainTex
float4 ScreenColor(float2 uv) {
	return SAMPLE_RT(_MainTex, sampler_MainTex, uv);
}

float4 ScreenColorTiled(float2 uv) {
	return SAMPLE_RT(_MainTex, Repeat, uv);
}
#endif

//Generic functions
#include "../SCPE.hlsl"
#define LightProjectionMultiplier 1
#define WorldViewDirection -GetWorldToViewMatrix()[2].xyz

#ifndef UNITY_GRAPHFUNCTIONS_LW_INCLUDED //Shader Graph
//Stereo rendering
//#define UNITY_SETUP_INSTANCE_ID(v) UNITY_SETUP_INSTANCE_ID(v)
#define STEREO_EYE_INDEX_POST_VERTEX(input) UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
//#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o) UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)

//Vertex
#if VERSION_GREATER_EQUAL(10,0) && _USE_DRAW_PROCEDURAL
#define OBJECT_TO_CLIP(input) TransformverticesProcedural(input.vertexID);
void TransformverticesProcedural(Attributes input, inout float2 positionCS)
{
	output.positionCS = GetQuadVertexPosition(input.vertexID);
	output.positionCS.xy = output.positionCS.xy * float2(2.0f, -2.0f) + float2(-1.0f, 1.0f); //convert to -1..1
}
#else
#define OBJECT_TO_CLIP(input) TransformObjectToHClip(input.positionOS.xyz)
#endif

#if VERSION_GREATER_EQUAL(10,0) && _USE_DRAW_PROCEDURAL
#define GET_TRIANGLE_UV(input) GetQuadTexCoord(input.vertexID) * _ScaleBias.xy + _ScaleBias.zw;
#else
#define GET_TRIANGLE_UV(input) input.uv
#endif
#define GET_TRIANGLE_UV_VR(uv, id) UnityStereoTransformScreenSpaceTex(uv)
float2 FlipUV(float2 uv) {
	return uv;
}
//Fragment
#define UV i.uv
#define UV_VR UnityStereoTransformScreenSpaceTex(i.uv)


//Varyings/Attributes were removed in URP 10.0.0 so include an abstraction here for backwards compatibility
struct Attributes
{
	#if _USE_DRAW_PROCEDURAL && VERSION_GREATER_EQUAL(10,0)
	uint vertexID     : SV_VertexID;
	#else
	float4 positionOS 	: POSITION;
	float2 uv 			: TEXCOORD0;
	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings {
	float4 positionCS 		: POSITION;
	float2 uv 				: TEXCOORD0;
	float2 texcoordStereo 	: TEXCOORD2;
	UNITY_VERTEX_OUTPUT_STEREO
};


Varyings Vert(Attributes input) {
	Varyings output;

	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

	output.positionCS = OBJECT_TO_CLIP(input);
	output.uv.xy = GET_TRIANGLE_UV(input);

	output.uv = FlipUV(output.uv);

	//UNITY_SINGLE_PASS_STEREO
	output.texcoordStereo = GET_TRIANGLE_UV_VR(output.uv, input.vertexID);

	return output;
}

struct Varyings2 {
	float4 positionCS : POSITION;
	float4 texcoord[2] : TEXCOORD0;
	float2 texcoordStereo : TEXCOORD2;
	UNITY_VERTEX_OUTPUT_STEREO
};

struct Varyings3 {
	float4 positionCS : POSITION;
	float4 texcoord[3] : TEXCOORD0;
	float2 texcoordStereo : TEXCOORD3;
	UNITY_VERTEX_OUTPUT_STEREO
};

#define EPSILON         1.0e-4

// Temporarly added so Fog shader compiles without error
// Quadratic color thresholding
// curve = (threshold - knee, knee * 2, 0.25 / knee)
//
half4 QuadraticThreshold(half4 color, half threshold, half3 curve)
{
	// Pixel brightness
	half br = Max3(color.r, color.g, color.b);

	// Under-threshold part: quadratic curve
	half rq = clamp(br - curve.x, 0.0, curve.y);
	rq = curve.z * rq * rq;

	// Combine and apply the brightness response curve.
	color *= max(rq, br - threshold) / max(br, EPSILON);

	return color;
}

#define HALF_MAX        65504.0 // (2 - 2^-10) * 2^15

// Clamp HDR value within a safe range
half3 SafeHDR(half3 c)
{
	return min(c, HALF_MAX);
}

half4 SafeHDR(half4 c)
{
	return min(c, HALF_MAX);
}
#endif