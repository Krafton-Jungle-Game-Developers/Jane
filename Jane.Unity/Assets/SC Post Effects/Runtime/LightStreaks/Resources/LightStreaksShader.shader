Shader "Hidden/SC Post Effects/Light Streaks"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"
	#include "../../../Shaders/Blurring.hlsl"

	DECLARE_RT(_BloomTex);

	float4 _Params;
	//X: Luminance threshold
	//Y: Intensity
	//Z: ...
	//W: ...

	float4 FragLuminanceDiff(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);
		
		float3 luminance = LuminanceThreshold(screenColor.rgb, _Params.x);
		luminance *= _Params.y;

		return float4(luminance.rgb, screenColor.a);
	}

	float4 FragBlend(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 original = SAMPLE_RT(_MainTex, Clamp, UV_VR);
		float3 bloom = SAMPLE_RT(_BloomTex, Clamp, UV_VR).rgb;

		return float4(original.rgb + bloom, original.a);
	}

	float4 FragDebug(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		return SAMPLE_RT(_BloomTex, Clamp, UV_VR);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass //0
		{
			Name "Light Streaks: Luminance filter"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragLuminanceDiff

			ENDHLSL
		}
		Pass //1
		{
			Name "Light Streaks: Streak (Performance mode)"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragBlurBox

			ENDHLSL
		}
		Pass //2
		{
			Name "Light Streaks: Streak (Appearance mode)"
			HLSLPROGRAM
			#pragma vertex VertGaussian
			#pragma fragment FragBlurGaussian

			ENDHLSL
		}
		Pass //3
		{
			Name "Light Streaks: Composite"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragBlend

			ENDHLSL
		}
		Pass //4
		{
			Name "Light Streaks: Debug"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragDebug

			ENDHLSL
		}
	}
}