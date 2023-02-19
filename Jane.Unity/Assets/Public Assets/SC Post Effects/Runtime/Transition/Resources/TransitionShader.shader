Shader "Hidden/SC Post Effects/Transition"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_Gradient);
	SamplerState sampler_Gradient;

	float _Progress;

	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = SCREEN_COLOR(UV_VR);

		float gradientTex = SAMPLE_TEX(_Gradient, sampler_Gradient, UV_VR).r;

		float alpha = smoothstep(gradientTex, _Progress, 1.01);

		return float4(lerp(screenColor.rgb, 0, alpha), screenColor.a);
	}

	ENDHLSL

	SubShader
	{
	Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Transition"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}