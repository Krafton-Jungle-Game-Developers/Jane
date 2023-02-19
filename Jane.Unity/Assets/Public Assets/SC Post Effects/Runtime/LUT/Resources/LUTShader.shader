Shader "Hidden/SC Post Effects/LUT"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_LUT_Near);
	DECLARE_TEX(_LUT_Far);
	SamplerState sampler_LUT_Near;
	SamplerState sampler_LUT_Far;

	float4 _LUT_Near_TexelSize;
	//X: 1.0 / Width
	//Y: 1.0 / Height
	//Z: Resolution Width
	//W: Resolution Height

	float4 _LUT_Params;
	//X: Intensity
	//Y: Color inversion
	float4 _FadeParams;
	//X: Start distance
	//Y: End distance

	float4 _Params;
	//XYZ: Vibrance balance
	//W: Vibrance intensity
	
	#define LUTIntensity _LUT_Params.x
	#define ColorInversion _LUT_Params.y
	#define VibranceBalance _Params.xyz
	#define VibranceStrength _Params.w

	//Note: When the GLES/OpenCL API is used TEXTURE2D_ARGS does not accept a sampler parameter
    //2.1.1, macros handle this now, safe to pass in a samplerstate

	float3 FormatStrip(half3 uvw, half3 scaleOffset)
	{
		//Strip format where `height = sqrt(width)`
		uvw.z *= scaleOffset.z;
		half shift = floor(uvw.z);
		uvw.xy = uvw.xy * scaleOffset.z * scaleOffset.xy + scaleOffset.xy * 0.5;
		uvw.x += shift * scaleOffset.y;

		uvw.z -= shift;

		return uvw;
	}

	half3 ApplyLut2d(TEX_ARG(tex, samplerTex), half3 uvw, half3 scaleOffset)
	{
		uvw = FormatStrip(uvw, scaleOffset);

		float3 centerLut = SAMPLE_TEX(tex, samplerTex, uvw.xy).rgb;
		float3 offsetLut = SAMPLE_TEX(tex, samplerTex, uvw.xy + half2(scaleOffset.y, 0)).rgb;

		uvw.xyz = lerp(centerLut, offsetLut, uvw.z);

		return uvw;
	}

	float3 GetScaleOffset()
	{
		return float3(_LUT_Near_TexelSize.x, _LUT_Near_TexelSize.y, _LUT_Near_TexelSize.w - 1.0);
	}
    
    inline float3 Grade(TEX_ARG(lut, samplerTex), half3 rgb) 
	{
		half3 colorGraded;

		//Working with LDR information. Effect must execute after Bloom. So this breaks the HDR buffer in URP
		rgb = saturate(rgb);
        
        #if !UNITY_COLORSPACE_GAMMA //Linear
        rgb = LinearToSRGB(rgb);
        #endif

        colorGraded = ApplyLut2d(TEX_PARAM(lut, samplerTex), rgb, GetScaleOffset());

		#if !UNITY_COLORSPACE_GAMMA //Linear
        colorGraded = SRGBToLinear(colorGraded);
		#endif

		return colorGraded;
	}

	void ApplyColorGrading(inout float3 color, float2 uv, bool depthBlend)
	{
		if(LUTIntensity > 0)
		{
			float3 colorGraded = 0;

			//Constant at compile time, so will properly branch
			if(depthBlend)
			{
				float depth = SAMPLE_DEPTH(uv);

				half3 gradedNear = Grade(TEX_PARAM(_LUT_Near, sampler_LUT_Near), color.rgb);
				half3 gradedFar = Grade(TEX_PARAM(_LUT_Far, sampler_LUT_Far), color.rgb);

				float fadeDist = LinearDepthFade(LINEAR_DEPTH(depth), _FadeParams.x, _FadeParams.y, 0.0, 1.0);
		
				colorGraded = lerp(gradedFar, gradedNear, fadeDist);
			}
			else
			{
				colorGraded = Grade(TEX_PARAM(_LUT_Near, sampler_LUT_Near), color.rgb);		
			}

			color = lerp(color.rgb, colorGraded.rgb, LUTIntensity);
		}
	}

	void ApplyVibrancy(inout float3 color)
	{
		if(VibranceStrength > 0)
		{
			float3 inputColor = saturate(color);
			const float luminance = Luminance(inputColor);

			const float maxColor = max3(inputColor);
			const float minColor = min3(inputColor);

			const float saturation = maxColor - minColor;

			const float3 vibrance = VibranceBalance * VibranceStrength;
		
			color = lerp(luminance, color, 1.0 + (vibrance * (1.0 - (sign(vibrance) * saturation))));
		}
	}

	void ApplyColorInversion(inout float3 color)
	{
		if(ColorInversion > 0)
		{
			float3 inputColor = color;
		
			#if !UNITY_COLORSPACE_GAMMA //Linear
			inputColor.rgb = LinearToSRGB(inputColor.rgb);
			#endif
		
			float3 inverted = 1.0-(inputColor.rgb);

			#if !UNITY_COLORSPACE_GAMMA //Linear
			inverted = SRGBToLinear(inverted);
			#endif

			color = lerp(color, inverted, ColorInversion);
		}
	}

	float4 FragSingle(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);

		ApplyColorGrading(screenColor.rgb, UV_VR, false);
		
		ApplyVibrancy(screenColor.rgb);

		ApplyColorInversion(screenColor.rgb);

		return float4(screenColor.rgb, screenColor.a);
	}

	float4 FragDuo(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 screenColor = ScreenColor(UV_VR);

		ApplyColorGrading(screenColor.rgb, UV_VR, true);

		ApplyVibrancy(screenColor.rgb);

		ApplyColorInversion(screenColor.rgb);

		return float4(screenColor.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Color Grading LUT"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragSingle

			ENDHLSL
		}
		Pass //Depth based
		{
			Name "Dual Color Grading LUT"
			HLSLPROGRAM
			
			#pragma vertex Vert
			#pragma fragment FragDuo

			ENDHLSL
		}
	}
}