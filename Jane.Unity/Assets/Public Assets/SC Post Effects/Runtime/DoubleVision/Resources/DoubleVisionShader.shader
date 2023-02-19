Shader "Hidden/SC Post Effects/Double Vision"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float _Amount;

	float4 Frag(Varyings i) : SV_Target
	{
		float4 screenColor = ScreenColor(UV_VR);

		screenColor += SCREEN_COLOR(UV_VR - float2(_Amount, 0));
		screenColor += SCREEN_COLOR(UV_VR + float2(_Amount, 0));

		return screenColor / 3.0;
	}

	float4 FragEdges(Varyings i) : SV_Target
	{
		float2 coords = 2.0 * UV - 1.0;
		float2 end = UV - coords * dot(coords, coords) * _Amount;
		float2 delta = (end - UV) / 3;

		half4 texelA = SCREEN_COLOR(UV_VR);
		half4 texelB = SCREEN_COLOR(delta + UV_VR);
		half4 texelC = SCREEN_COLOR(delta * 2.0 + UV_VR);

		return (texelA + texelB + texelC) / 3.0;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragEdges

			ENDHLSL
		}
	}
}