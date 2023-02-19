Shader "Hidden/SC Post Effects/Pixelize"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float _Resolution;

	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float aspect = _ScreenParams.y / _ScreenParams.x;

		float x = (int)((UV_VR.x / _Resolution / aspect) + 0.5) * _Resolution * aspect;
		float y = (int)((UV_VR.y / _Resolution) + 0.5) * _Resolution;

		return ScreenColor(half2(x,y));
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Pixelize"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}