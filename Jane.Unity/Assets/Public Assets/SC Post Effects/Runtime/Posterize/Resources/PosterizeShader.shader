Shader "Hidden/SC Post Effects/Posterize"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float _Depth;
	float4 _Params;

	float PosterizeChannel(float val, float amount)
	{
		return floor(val * amount) / (amount -1.0);
	}

	float3 PosterizeRGB(float3 val, float amount)
	{
		return floor(val.rgb * amount) / (amount -1.0);
	}

	float4 FragRGB(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float4 screenColor = SCREEN_COLOR(UV_VR);

		#ifndef UNITY_COLORSPACE_GAMMA
		screenColor.rgb = LinearToSRGB(screenColor.rgb);
		#endif

		float3 rgb = PosterizeRGB(screenColor.rgb, _Params.w);

		#ifndef UNITY_COLORSPACE_GAMMA
		rgb = SRGBToLinear(rgb);
		#endif
		
		return float4(rgb, screenColor.a);
	}

	float4 FragHSV(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float4 screenColor = SCREEN_COLOR(UV_VR);

		//Required
		#ifdef URP
		screenColor.rgb = saturate(screenColor.rgb);
		#endif
		
		#ifndef UNITY_COLORSPACE_GAMMA
		screenColor.rgb = LinearToSRGB(screenColor.rgb);
		#endif
		
		float3 hsv = RgbToHsv(screenColor.rgb);
		hsv.x = PosterizeChannel(hsv.x, _Params.x);
		hsv.y = PosterizeChannel(hsv.y, _Params.y);
		hsv.z = PosterizeChannel(hsv.z, _Params.z);

		float3 rgb = HsvToRgb(hsv);

		#ifndef UNITY_COLORSPACE_GAMMA
		rgb = SRGBToLinear(rgb);
		#endif
		
		return float4(rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Posterize RGB"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragRGB

			ENDHLSL
		}
		Pass
		{
			Name "Posterize HSV"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragHSV

			ENDHLSL
		}
	}
}