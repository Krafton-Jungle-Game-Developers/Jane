Shader "Hidden/SC Post Effects/Ambient Occlusion 2D"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"
	#include "../../../Shaders/Blurring.hlsl"

	DECLARE_TEX(_AO);
	float _SampleDistance;
	float _Threshold;
	float _Blur;
	float _Intensity;

	Varyings2 VertLum(Attributes v)
	{
		Varyings2 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);
		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);

		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, 1);

		o.texcoord[0].xy = uv;
		o.texcoord[0].zw = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
		o.texcoord[1].xy = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance;
		o.texcoord[1].zw = 0;

		return o;
	}

	float4 FragLuminanceDiff(Varyings3 i) : SV_Target
	{
		float4 original = SAMPLE_RT(_MainTex, sampler_LinearClamp, i.texcoord[0].xy).rgba;

		half3 p1 = original.rgb;
		half3 p2 = SAMPLE_RT(_MainTex, sampler_LinearClamp, i.texcoord[0].zw).rgb;
		half3 p3 = SAMPLE_RT(_MainTex, sampler_LinearClamp, i.texcoord[1].xy).rgb;

		half3 diff = p1 * 2 - p2 - p3;
		half edge = dot(diff, diff);
		edge = step(edge, _Threshold);

		//Edges only
		original.rgb = lerp(1, edge, _Intensity);

		return original;
	}

	float4 FragBlend(Varyings i) : SV_Target
	{
		float4 screenColor = SAMPLE_RT(_MainTex, sampler_LinearClamp, UV);
		float ao = SAMPLE_RT(_AO, sampler_LinearClamp, UV).r;

		return float4(screenColor.rgb * ao, screenColor.a);
	}

	float4 FragDebug(Varyings i) : SV_Target
	{
		 return SAMPLE_RT(_AO, sampler_LinearClamp, UV);
	}

	ENDHLSL

SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass //0
		{
			Name "Luminance filtering"
			HLSLPROGRAM
			#pragma vertex VertLum
			#pragma fragment FragLuminanceDiff
			ENDHLSL
		}
		Pass //1
		{	
			Name "Blurring"
			HLSLPROGRAM
			#pragma vertex VertGaussian
			#pragma fragment FragBlurGaussian
			ENDHLSL
		}
		Pass //2
		{
			Name "Composite"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragBlend
			ENDHLSL
		}
		Pass //3
		{
			Name "Debug"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragDebug
			ENDHLSL
		}
	}
}