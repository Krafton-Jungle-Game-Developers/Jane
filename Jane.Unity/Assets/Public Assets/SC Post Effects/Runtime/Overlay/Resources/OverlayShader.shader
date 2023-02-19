Shader "Hidden/SC Post Effects/Overlay"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_OverlayTex);
	float4 _Params;
	//X: Intensity
	//Y: Tiling
	//Z: Auto aspect (bool)
	//W: Blend mode
	float _LuminanceThreshold;

	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);

		float2 uv = i.uv;

		if(_Params.z == 1) uv.x *= _ScreenParams.x / _ScreenParams.y;

		#if UNITY_SINGLE_PASS_STEREO 
			uv = float2(UV_VR.x / 1, UV_VR.y);
		#endif
		float4 overlay = SAMPLE_TEX(_OverlayTex, Repeat, uv * _Params.y);

		float luminance = smoothstep(-0.01,  _LuminanceThreshold, Luminance(screenColor));
		//return float4(luminance.xxx, 1);
		overlay.a *= luminance;
		
		float3 color = 0;

		if (_Params.w == 0) color = lerp(screenColor.rgb, overlay.rgb, overlay.a * _Params.x);
		if (_Params.w == 1) color = lerp(screenColor.rgb, BlendAdditive(overlay.rgb, screenColor.rgb), overlay.a * _Params.x);
		if (_Params.w == 2) color = lerp(screenColor.rgb, overlay.rgb * screenColor.rgb, overlay.a * _Params.x);
		if (_Params.w == 3) color = lerp(screenColor.rgb, BlendScreen(overlay.rgb, screenColor.rgb), overlay.a * _Params.x);

		return float4(color.rgb, screenColor.a);
	}


	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Texture overlay"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}