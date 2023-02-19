Shader "Hidden/SC Post Effects/Sun Shafts"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_RT(_SunshaftBuffer);

	half _BlendMode;
	float4 _SunThreshold;
	float4 _SunColor;
	float4 _SunPosition;
	float _BlurRadius;

	float4 FragSky(Varyings i) : SV_Target
	{
		float2 uv = UV.xy;

		float depth = LINEAR_DEPTH(SAMPLE_DEPTH(UV).r);
		float4 skyColor = SCREEN_COLOR(UV);

		half2 vec = _SunPosition.xy - uv;
		//Correct for aspect ratio
		vec.x *= _ScreenParams.x / _ScreenParams.y;
		
		half dist = saturate(_SunPosition.w - length(vec.xy));

		float4 outColor = 0;
		//reject near depth pixels
		if (depth > 0.99) 
		{
			outColor = dot(max(skyColor.rgb - _SunThreshold.rgb, float3(0, 0, 0)), float3(1, 1, 1)) * dist;
		}

		return outColor;
	}

	float4 FragRadialBlur(Varyings i) : SV_Target
	{
		half4 c = half4(0,0,0,0);

		float2 uv = UV;
		for (int s = 0; s < 12; s++)
		{
			half4 color = SCREEN_COLOR(uv);
			c += color;
			//uv.xy += i.blurDir;
			uv.xy += (_SunPosition.xy - uv.xy) * _BlurRadius;
		}
		return c / 12;
	}

	float4 FragBlend(Varyings i) : SV_Target
	{
		float4 screenColor = SCREEN_COLOR(UV_VR);
		//return screenColor;

		float3 sunshafts = SAMPLE_RT(_SunshaftBuffer, Clamp, UV).rgb;
		sunshafts.rgb *= _SunColor.rgb * _SunPosition.z;
		//return float4(sunshafts.rgb, screenColor.a);

		float3 blendedColor = 0;

		if (_BlendMode == 0) blendedColor = BlendAdditive(screenColor.rgb, sunshafts.rgb); //Additive blend
		if (_BlendMode == 1) blendedColor = BlendScreen(sunshafts.rgb, screenColor.rgb); //Screen blend

		return float4(blendedColor.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Sunshafts sky mask"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragSky

			ENDHLSL
		}
		Pass
		{
			Name "Sunshafts blur"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragRadialBlur

			ENDHLSL
		}
		Pass
		{
			Name "Sunshafts composite"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragBlend

			ENDHLSL
		}
	}
}