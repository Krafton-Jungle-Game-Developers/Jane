Shader "Hidden/SC Post Effects/Kuwahara"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float _Radius;
	float4 _FadeParams;

	struct Window {
		int x1, y1, x2, y2;
	};

	//Based on the LightweightBloom method as described in GPU Pro chapter 5.1, translated from GLHSL
	inline float4 Kuwahara(float2 uv, float4 inputColor) {

		//float jitter = (sin(_Time.y * 10));
		float n = float((_Radius + 1) * (_Radius + 1));

		float3 m[4] = {
			float3(0, 0, 0),
			float3(0, 0, 0),
			float3(0, 0, 0),
			float3(0, 0, 0),
		};
		float3 s[4] = {
			float3(0, 0, 0),
			float3(0, 0, 0),
			float3(0, 0, 0),
			float3(0, 0, 0),
		};

		int k = 0;

		Window W[4] = {
			{ -_Radius, -_Radius,       0,       0 },
			{ 0, -_Radius, _Radius,       0 },
			{ 0,        0, _Radius, _Radius },
			{ -_Radius,        0,       0, _Radius }
		};

		UNITY_UNROLL
		for (k = 0; k < 4; ++k) {

			for (int j = W[k].y1; j <= W[k].y2; ++j) {
				for (int i = W[k].x1; i <= W[k].x2; ++i) {

					//Shader warning in 'Hidden/SC Post Effects/Kuwahara': gradient instruction used in a loop with varying iteration, attempting to unroll the loop
					float3 c = SAMPLE_RT_LOD(_MainTex, Clamp, uv + float2(i * _MainTex_TexelSize.x * 1, j * _MainTex_TexelSize.y), 0).rgb;
					m[k] += c;
					s[k] += c * c ;
				}
			}
		}

		float min_sigma2 = 1e+6;
		float s2;

		UNITY_LOOP
		for (k = 0; k < 4; ++k) {
			m[k] /= n;
			s[k] = abs(s[k] / n - m[k] * m[k]);

			s2 = s[k].r + s[k].g + s[k].b;
			if (s2 < min_sigma2) {
				min_sigma2 = s2;
				inputColor.rgb = m[k].rgb;
			}
		}

		return inputColor;
	}

	float4 Frag(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV_VR;
		float4 screenColor = ScreenColor(uv);

		float4 paintColor = Kuwahara(uv, screenColor);

		return paintColor;
	}

	float4 FragDepthAware(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV_VR;
		float4 screenColor = ScreenColor(uv);

		float depth = SAMPLE_DEPTH(uv);

		float fadeFactor = LinearDepthFade(LINEAR_DEPTH(depth), _FadeParams.x, _FadeParams.y, 0.0, 1.0);

		float4 paintColor = lerp(Kuwahara(uv, screenColor), screenColor, fadeFactor);

		return paintColor;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Kuwahara"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}

		Pass
		{
			Name "Kuwahara (Depth-based)"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragDepthAware

			ENDHLSL
		}
	}
}