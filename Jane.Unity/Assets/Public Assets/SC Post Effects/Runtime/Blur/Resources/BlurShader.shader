Shader "Hidden/SC Post Effects/Blur"
{
	HLSLINCLUDE
	#define MULTI_PASS
	#define REQUIRE_DEPTH

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"
	#include "../../../Shaders/Blurring.hlsl"

	DECLARE_RT(_BlurredTex);
	SAMPLER(sampler_BlurredTex);
	float4 _FadeParams;
	
	//Separate pass, because this shouldn't be looped
	float4 FragBlend(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		return SAMPLE_RT(_BlurredTex, sampler_BlurredTex, UV_VR);
	}

	float4 FragDepthFade(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);
		float3 blurredColor = SAMPLE_RT(_BlurredTex, sampler_BlurredTex, UV_VR).rgb;

		float depth = SAMPLE_DEPTH(UV_VR);

		float fadeDist = LinearDepthFade(LINEAR_DEPTH(depth), _FadeParams.x, _FadeParams.y, 0.0, 1.0);

		return float4(lerp(blurredColor.rgb, screenColor.rgb, fadeDist), screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass //0
		{
			Name "Blur Blend"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragBlend

			ENDHLSL
		}
		Pass //1
		{
			Name "Blur Depth Fade"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragDepthFade

			ENDHLSL
		}
		Pass //2
		{
			Name "Gaussian Blur"
			HLSLPROGRAM

			#pragma vertex VertGaussian
			#pragma fragment FragBlurGaussian

			ENDHLSL
		}
		Pass //3
		{
			Name "Box Blur"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragBlurBox

			ENDHLSL
		}

	}
}