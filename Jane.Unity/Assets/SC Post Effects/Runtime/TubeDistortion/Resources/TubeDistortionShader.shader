Shader "Hidden/SC Post Effects/Tube Distortion"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float _Amount;

	float2 BuldgedUV(half2 uv, half amount, half zoom)
	{
		half2 center = uv.xy - half2(0.5, 0.5);
		half CdotC = dot(center, center);
		half f = 1.0 + CdotC * (amount * sqrt(CdotC));
		return f * zoom * center + 0.5;
	}

	float2 PinchUV(float2 uv)
	{
		uv = uv * 2.0 - 1.0;
		float2 offset = abs(uv.yx) * float2(_Amount , _Amount);
		uv = uv + uv * offset * offset;
		uv = uv * 0.5 + 0.5;
		return uv;
	}

	float EdgesUV(float2 uv)
	{
		half2 d = abs(uv - float2(0.5, 0.5)) * 2;
		d = pow(saturate(d), 8);

		float vignette = saturate(1-dot(d, d));

		return vignette;
	}

	float4 FragBuldge(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float2 uv = BuldgedUV(UV_VR, _Amount, lerp(1, 0.75, _Amount));
		
		float4 screenColor = ScreenColor(uv);

		return float4(screenColor.rgb, screenColor.a);
	}

	float4 FragPinch(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float2 uv = PinchUV(UV_VR);

		float2 blackEdge = 1-ceil((uv.xy -1) * (uv.xy / UV_VR * 0.001)).rg;
		float crop = (blackEdge.r * blackEdge.g);

		//return float4(crop, crop, crop, 0);

		float4 screenColor = ScreenColor(uv);

		return float4(screenColor.rgb * crop, screenColor.a);
	}

	float4 FragBevel(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float2 uv = lerp(UV_VR, UV_VR * EdgesUV(UV_VR), _Amount);

		float4 screenColor = ScreenColor(uv);

		return float4(screenColor.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Tube Distortion: Buldge"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragBuldge

			ENDHLSL
		}
		Pass
		{
			Name "Tube Distortion: Pinch"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragPinch

			ENDHLSL
		}
		Pass
		{
			Name "Tube Distortion: Bevel"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragBevel

			ENDHLSL
		}
	}
}