Shader "Hidden/SC Post Effects/Gradient"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_Gradient);
	float _Intensity;
	float _Rotation;
	float4 _Color1;
	float4 _Color2;
	half _BlendMode;

	inline float3 BlendColors(float3 colors, float3 screenColor, float alpha) 
	{
		float3 color = 0;

		if (_BlendMode == 0) color = lerp(screenColor, colors, alpha * _Intensity);
		if (_BlendMode == 1) color = lerp(screenColor, BlendAdditive(colors, screenColor), alpha * _Intensity);
		if (_BlendMode == 2) color = lerp(screenColor, colors * screenColor, alpha * _Intensity);
		if (_BlendMode == 3) color = lerp(screenColor, BlendScreen(colors, screenColor), alpha * _Intensity);

		return color;
	}

	float4 FragColors(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);	
		float2 gradientUV = RotateUV(UV_VR, float2(0.5, 0.5), _Rotation);

		float4 colors = lerp(_Color2, _Color1, gradientUV.y);

		#ifndef UNITY_COLORSPACE_GAMMA
		colors.rgb = SRGBToLinear(colors.rgb);
		#endif
		
		float3 color = BlendColors(colors.rgb, screenColor.rgb, lerp(_Color2.a, _Color1.a, gradientUV.y));

		return float4(color.rgb, screenColor.a);
	}

	float4 FragTexture(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);
		float2 gradientUV = RotateUV(UV_VR, float2(0.5, 0.5), _Rotation);

		float4 gradient = SAMPLE_TEX(_Gradient, Clamp, gradientUV);
		
		float3 color = BlendColors(gradient.rgb, screenColor.rgb, gradient.a);

		return float4(color.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		//2-colors
		Pass
		{
			Name "Gradient by colors"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragColors
			ENDHLSL
		}
		//Gradient texture
		Pass
		{
			Name "Gradient by texture"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragTexture
			ENDHLSL
		}
	}
}