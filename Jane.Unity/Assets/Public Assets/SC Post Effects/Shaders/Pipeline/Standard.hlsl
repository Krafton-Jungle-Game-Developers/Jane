//Libraries
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl" //PPSv2
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Sampling.hlsl"
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"
#include "../Blending.hlsl"

//Use shared texture samplers, which is in line with SRP methods
SamplerState sampler_LinearClamp;
SamplerState sampler_LinearRepeat;
#define Clamp sampler_LinearClamp
#define Repeat sampler_LinearRepeat

#define SAMPLER(textureName) SAMPLER2D(textureName)

TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex); /* Always included */
float4 _MainTex_TexelSize;
#ifdef REQUIRE_DEPTH
TEXTURE2D(_CameraDepthTexture);
#endif
#ifdef REQUIRE_DEPTH_NORMALS
TEXTURE2D(_CameraDepthNormalsTexture);
#endif

//Tex sampling (TEX and RT expand to the same function, since no Tex2DArray is used)
#define DECLARE_TEX(textureName) TEXTURE2D(textureName);
#define DECLARE_RT(textureName) DECLARE_TEX(textureName);

#define SAMPLE_TEX(textureName, samplerName, uv) SAMPLE_TEXTURE2D(textureName, samplerName, uv)  
#define SAMPLE_RT(textureName, samplerName, uv) SAMPLE_TEX(textureName, samplerName, uv)  
#define SAMPLE_TEX_LOD(textureName, samplerName, uv, mip) SAMPLE_TEXTURE2D_LOD(textureName, samplerName, uv, mip)  
#define SAMPLE_RT_LOD(textureName, samplerName, uv, mip) SAMPLE_TEX_LOD(textureName, samplerName, uv, mip)  

#define SAMPLE_DEPTH(uv) SAMPLE_TEX(_CameraDepthTexture, Clamp, uv).r  
#define LINEAR_DEPTH(depth) Linear01Depth(depth)
#define LINEAR_EYE_DEPTH(depth) LinearEyeDepth(depth)
#define SAMPLE_DEPTH_NORMALS(uv) SAMPLE_TEX(_CameraDepthNormalsTexture, Clamp, uv)

/*
float4 SampleDepthNormals(float2 uv)
{
	#ifdef REQUIRE_DEPTH_NORMALS
	float4 normals = SAMPLE_TEX(_CameraDepthNormalsTexture, Clamp, uv);
	float depth = LINEAR_DEPTH(SAMPLE_DEPTH(uv));
	
	return float4(normals.rgb, depth);
	#else
	return 0;
	#endif
}
*/

#define TEX_PARAM(textureName, samplerName) TEXTURE2D_PARAM(textureName, samplerName)
#define TEX_ARG(textureName, samplerName) TEXTURE2D_ARGS(textureName, samplerName)
#define RT_PARAM(textureName, samplerName) TEX_PARAM(textureName, samplerName)
#define RT_ARG(textureName, samplerName) TEX_ARG(textureName, samplerName)

//Shorthand for sampling MainTex
#define SCREEN_COLOR(uv) SAMPLE_RT(_MainTex, Clamp, uv);
float4 ScreenColor(float2 uv) {
	return SAMPLE_RT(_MainTex, sampler_MainTex, uv);
}

float4 ScreenColorTiled(float2 uv) {
	return SAMPLE_RT(_MainTex, Repeat, uv);
}

//Generic functions
#include "../SCPE.hlsl"
#define LightProjectionMultiplier 1 //Magic scalar to match the transform's actual worldToLocalMatrix
float4x4 UNITY_MATRIX_V;
#define WorldViewDirection -UNITY_MATRIX_V[2].xyz

/* Doesn't do anything, no instancing library available */
#define STEREO_EYE_INDEX_POST_VERTEX(input) 0  
#define UNITY_SETUP_INSTANCE_ID(v) 0
#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o) 0
#define UNITY_VERTEX_OUTPUT_STEREO // So that templates compile

//Vertex
#define OBJECT_TO_CLIP(v) float4(v.positionOS.xy, 0.0, 1.0)
#define GET_TRIANGLE_UV(v) TransformTriangleVertexToUV(v.positionOS.xy).xy 
#define GET_TRIANGLE_UV_VR(uv, vertexID) TransformStereoScreenSpaceTex(uv, 1.0)

float2 FlipUV(float2 uv) {
#if UNITY_UV_STARTS_AT_TOP
	return uv * float2(1.0, -1.0) + float2(0.0, 1.0);
#else
	return uv;
#endif
}

//Fragment
#define UV i.uv
#define UV_VR i.texcoordStereo

//Structs (same names as URP/HDRP)
struct Attributes
{
	float3 positionOS : POSITION;
	float2 uv : TEXCOORD0;
	//Safe to remove, macros end up not using the attribute anyway
	//uint vertexID : SV_VertexID;
};

struct Varyings
{
	float4 positionCS : SV_POSITION;
	float2 uv : TEXCOORD0;
	float2 texcoordStereo : TEXCOORD1;

#if STEREO_INSTANCING_ENABLED
	uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
#endif
};

struct Varyings2 {
	float4 positionCS : POSITION;
	float4 texcoord[2] : TEXCOORD0;
	float2 texcoordStereo : TEXCOORD2;

#if STEREO_INSTANCING_ENABLED
	uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
#endif
};

struct Varyings3 {
	float4 positionCS : POSITION;
	float4 texcoord[3] : TEXCOORD0;
	float2 texcoordStereo : TEXCOORD3;

#if STEREO_INSTANCING_ENABLED
	uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
#endif
};

Varyings Vert(Attributes v)
{
	Varyings o;
#if STEREO_DOUBLEWIDE_TARGET
	o.positionCS = OBJECT_TO_CLIP(float4(v.positionOS.xy * _PosScaleOffset.xy + _PosScaleOffset.zw, 0, 1), v.vertexID);
#else
	o.positionCS = OBJECT_TO_CLIP(v);
#endif
	o.uv = GET_TRIANGLE_UV(v);

	o.uv = FlipUV(o.uv);

	o.texcoordStereo = GET_TRIANGLE_UV_VR(o.uv, v.vertexID);
#if STEREO_INSTANCING_ENABLED
	o.stereoTargetEyeIndex = (uint)_DepthSlice;
#endif

	return o;
}
