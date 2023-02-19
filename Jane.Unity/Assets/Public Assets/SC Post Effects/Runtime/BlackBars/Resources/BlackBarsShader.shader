Shader "Hidden/SC Post Effects/Black Bars"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float2 _Size;

	float4 FragHorizontal(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV;
		float4 screenColor = ScreenColor(UV_VR);

		half bars = min(uv.y, (1-uv.y));
		bars = step(_Size.x * _Size.y, bars);

		return float4(screenColor.rgb * bars, screenColor.a);
	}

	float4 FragVertical(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV;
		float4 screenColor = ScreenColor(UV_VR);

		half bars = (uv.x * (1-uv.x));
		bars = step(_Size.x * (_Size.y /2), bars);

		return float4(screenColor.rgb * bars, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragHorizontal

			ENDHLSL
		}
		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragVertical

			ENDHLSL
		}
	}
}