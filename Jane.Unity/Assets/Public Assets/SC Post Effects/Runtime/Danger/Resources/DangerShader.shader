Shader "Hidden/SC Post Effects/Danger"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_Overlay);
	float4 _Color;
	float4 _Params;
	//X: Intensity
	//Y: Size

	float Vignette(float2 uv)
	{
		float vignette = uv.x * uv.y * (1 - uv.x) * (1 - uv.y);
		return clamp(16.0 * vignette, 0, 1);
	}

	float4 Frag(Varyings i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float overlay = SAMPLE_TEX(_Overlay, Clamp, UV_VR).a;

		float vignette = Vignette(UV_VR);
		overlay = (overlay * _Params.y) ;
		vignette = (vignette / overlay);
		vignette = 1-saturate(vignette);

		float4 screenColor = ScreenColor(UV_VR);

		float alpha = vignette * _Color.a * _Params.x;

		return float4(lerp(screenColor.rgb, _Color.rgb, alpha), screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Danger"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}