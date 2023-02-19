Shader "Hidden/SC Post Effects/Colorize"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_ColorRamp);

	float _Intensity;
	half _BlendMode;

	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);

		half luminance = Luminance(screenColor.rgb);

		float4 colors = SAMPLE_TEX(_ColorRamp, sampler_LinearClamp, float2(luminance, 0));
	
		float3 color = 0;

		if (_BlendMode == 0) color = lerp(screenColor.rgb, colors.rgb, colors.a * _Intensity);
		if (_BlendMode == 1) color = lerp(screenColor.rgb, BlendAdditive(colors.rgb, screenColor.rgb), colors.a * _Intensity);
		if (_BlendMode == 2) color = lerp(screenColor.rgb, colors.rgb * screenColor.rgb, _Intensity);
		if (_BlendMode == 3) color = lerp(screenColor.rgb, BlendScreen(colors.rgb, screenColor.rgb), colors.a * _Intensity);

		return float4(color.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Colorize"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}