Shader "Hidden/SC Post Effects/Cloud Shadows"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_NoiseTex);

	float4 _CloudParams;
	float4 _FadeParams;
	float _ProjectionEnabled;
	
	float4x4 unity_WorldToLight;
	
	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 sceneColor = SCREEN_COLOR(UV_VR);
		float depth = SAMPLE_DEPTH(UV_VR);
		//return LINEAR_DEPTH(depth).xxxx;

		float3 worldPos = GetWorldPosition(UV_VR, depth);
		//return float4(frac(worldPos), 1.0);
		
		float2 projection = worldPos.xz;
		if(_ProjectionEnabled == 1) projection = mul((float4x4)unity_WorldToLight, float4(worldPos, 1.0)).xy * LightProjectionMultiplier;
		
		float2 uv = projection * _CloudParams.x + (_Time.y * float2(_CloudParams.y, _CloudParams.z));
		float clouds = 1- SAMPLE_TEX(_NoiseTex, sampler_LinearRepeat, uv).r;

		//Clip skybox
		if (LINEAR_DEPTH(depth) > 0.99) clouds = 1;

		float fadeFactor = LinearDistanceFade(worldPos, _FadeParams.x, _FadeParams.y, 1.0, 1.0);
		
		clouds = lerp(1, clouds, _CloudParams.w * fadeFactor);

		float3 cloudsBlend = sceneColor.rgb * clouds;

		return float4(cloudsBlend.rgb, sceneColor.a);
	}


	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Cloud Shadows"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}