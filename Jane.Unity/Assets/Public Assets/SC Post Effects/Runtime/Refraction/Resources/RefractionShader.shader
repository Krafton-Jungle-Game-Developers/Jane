Shader "Hidden/SC Post Effects/Refraction"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_RefractionTex);
	uniform float _Amount;

	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 dudv = SAMPLE_TEX(_RefractionTex, Clamp, UV).rgba;

		float2 refraction = lerp(UV_VR, (UV_VR) * dudv.rg, _Amount * dudv.rg);

		return ScreenColor(refraction);
	}

	float4 FragNormalMap(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 dudv = SAMPLE_TEX(_RefractionTex, Clamp, UV).rgba;

#if UNITY_VERSION >= 20172 //Pre 2017.2
		dudv.x *= dudv.w;
#else
		dudv.x = 1 - dudv.x;
#endif
		//Remap to -1,-1
		dudv.xy = dudv.xy * 2 - 1;

		float2 refraction = lerp(UV_VR, (UV_VR)* dudv.rg, _Amount * dudv.rg);

		return ScreenColor(refraction);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Refraction"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
		Pass
		{
			Name "Refraction by normal map"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragNormalMap

			ENDHLSL
		}
	}
}