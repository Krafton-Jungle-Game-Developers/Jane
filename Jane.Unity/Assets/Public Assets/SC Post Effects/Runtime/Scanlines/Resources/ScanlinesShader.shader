Shader "Hidden/SC Post Effects/Scanlines"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float4 _Params;
	//X: Amount
	//Y: Intensity
	//Z: Speed

	float4 Frag(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV;

		float4 screenColor = SAMPLE_RT(_MainTex, sampler_MainTex, UV_VR);

		half linesY = uv.y - sin(uv.y * _Params.x + (_Time.w * _Params.z)) * _Params.x;

		float3 color = lerp(screenColor, screenColor * linesY, _Params.y).rgb;

		return float4(color.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always Blend Off

		Pass
		{
			Name "Scanlines"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			ENDHLSL
		}
	}

	Fallback Off
}
