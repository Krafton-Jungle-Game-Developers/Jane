Shader "Hidden/SC Post Effects/Dithering"
{
	HLSLINCLUDE

	//#define REQUIRE_DEPTH
	//#define REQUIRE_DEPTH_NORMALS
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"
	//#include "../../../Shaders/SCPE.hlsl"

	DECLARE_TEX(_LUT);
	SAMPLER(sampler_LUT);

	float4 _Dithering_Coords;
	//X: Size
	//Y: Tiling
	//Z: Luminance influence
	//W: Intensity

	uniform float4x4 clipToWorld;
	float4x4 cameraToWorld;
	struct v2f {
		float4 positionCS : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 texcoordStereo : TEXCOORD1;
		float3 worldDirection : TEXCOORD2;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f VertWSRecontruction(Attributes v) {
		v2f o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);
		o.uv.xy = GET_TRIANGLE_UV(v);

		o.uv = FlipUV(o.uv);

		float4 clip = float4(o.uv.xy * 2 - 1, 0.0, 1.0);
		o.worldDirection.rgb = (mul((float4x4)clipToWorld, clip.rgba).xyz - _WorldSpaceCameraPos.rgb).xyz;

		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(o.uv, v.vertexID);

		return o;
	}

	float3 ApplyDithering(float3 color, float2 uv)
	{
		float luminance = Luminance(LinearToSRGB(color.rgb));

		float lut = SAMPLE_TEX(_LUT, Repeat, uv).r;

		float dither = step(lut, luminance / _Dithering_Coords.z);

		return lerp(color, color * saturate(dither), _Dithering_Coords.w);
	}

	float4 Frag(Varyings i) : SV_Target
	{
		float4 screenColor = SCREEN_COLOR(UV_VR);

		float2 lutUV = float2(UV.x *= _ScreenParams.x / _ScreenParams.y, UV.y * _ScreenParams.w) *  _Dithering_Coords.y * 32;
		float3 ditheredColor = ApplyDithering(screenColor.rgb, lutUV);
		
		return float4(lerp(screenColor.rgb, ditheredColor.rgb, _Dithering_Coords.w), screenColor.a);
	}

	/*
	//Scrapped, moire artifacts make this pretty unusable
	float4 FragProjected(v2f i) : SV_Target
	{
		float4 screenColor = SCREEN_COLOR(UV_VR);

		//World-space UV
		float depth = SAMPLE_DEPTH(UV_VR);
		float3 worldPos = i.worldDirection.xyz * LINEAR_EYE_DEPTH(depth) + _WorldSpaceCameraPos.xyz;
		worldPos *= _Dithering_Coords.y * 4;

		//XYZ projection masks
		float3 normals = SAMPLE_DEPTH_NORMALS(UV_VR);
		float viewDepth = 0;
		DecodeDepthNormal(SAMPLE_DEPTH_NORMALS(UV_VR), viewDepth, normals);
		normals = mul(cameraToWorld, normals) ;

		//XYZ sampling
		float lutX = lerp(SAMPLE_TEX(_LUT, sampler_LUT, worldPos.yz).r, 1, 0);
		float lutY = lerp(lutX, SAMPLE_TEX(_LUT, sampler_LUT, worldPos.xz).r, normals.y);	
		float lutZ = lerp(lutY, SAMPLE_TEX(_LUT, sampler_LUT, worldPos.xy).r, normals.z);

		//Screen-space for skybox
		if(LINEAR_DEPTH(depth) > 0.99)
		{
			float2 lutUV = float2(UV.x *= _ScreenParams.x / _ScreenParams.y, UV.y * _ScreenParams.w) *  _Dithering_Coords.y * 32;
			lutUV += (normals.xy * 1);
		}
		
		float luminance = Luminance(LinearToSRGB(screenColor.rgb));
		float dither = step(saturate(lutZ), luminance / _Dithering_Coords.z);

		float3 ditheredColor = lerp(screenColor, screenColor * saturate(dither), _Dithering_Coords.w);
		
		return float4(lerp(screenColor, ditheredColor, _Dithering_Coords.w), screenColor.a);
	}
	*/

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Dithering"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
		/*
		Pass
		{
			Name "Dithering (World projected)"
			HLSLPROGRAM

			#pragma vertex VertWSRecontruction
			#pragma fragment FragProjected

			ENDHLSL
		}
		*/
	}
}