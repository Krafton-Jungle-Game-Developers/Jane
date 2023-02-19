Shader "Hidden/SC Post Effects/SpeedLines"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_NoiseTex);
	float4 _Params;

	float2 CartToPolar(float2 uv) {
		float2 polar = uv - float2(0.5, 0.5);
		float2 uv2 = polar;

		//Radial
		polar.x = length(polar) *0.01;
		//Angular
		polar.y = 0.5 + (atan2(uv2.x, uv2.y) / 6.283185);

		polar.y *= _Params.z;
		polar.y += frac(_Time.w);

		return polar;
	}

	float RadialMask(float2 uv) {
		float falloff = length(float2(float2(0.5, 0.5) - frac(uv)) / 0.70);
		falloff = pow(falloff, _Params.y);
		falloff = saturate(falloff);

		return falloff;
	}

	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float2 uv = CartToPolar(UV);

		float4 screenColor = ScreenColor(UV_VR);

		float noise = SAMPLE_TEX(_NoiseTex, Repeat, uv).r;

		noise *= RadialMask(UV);
		float3 color = lerp(screenColor.rgb, screenColor.rgb + noise, _Params.x);

		return float4(color.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Speedlines"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}