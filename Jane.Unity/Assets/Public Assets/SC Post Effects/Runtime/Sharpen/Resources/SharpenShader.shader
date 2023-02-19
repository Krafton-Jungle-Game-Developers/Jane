Shader "Hidden/SC Post Effects/Sharpen"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	float4 _Params;
	//X: Amount
	//Y: Radius
	//Z: Contrast

	static const int kernelSize = 4;
	static const float kernelSize_RCP = 0.25;
	static const float2 kernel[kernelSize] = {
		float2(-1,-1),
		float2(1,-1),
		float2(-1,1),
		float2(1,1)
	};

	#define Sharpening _Params.x
	#define Radius _Params.y
	#define Contrast _Params.z * 1.25

	float3 AverageNeighbors(float2 uv, float2 texelSize, float radius)
	{
		float3 sum = 0;

		UNITY_UNROLL
		for(int i = 0; i < kernelSize; i++)
		{
			sum += ScreenColor(uv + (kernel[i] * texelSize.xy) * radius).rgb;
		}

		return sum * kernelSize_RCP;
	}

	float4 FragLuminanceEnhancement(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV_VR;
		float4 screenColor = ScreenColor(UV_VR);

		float3 average = AverageNeighbors(uv, 1.0 / _ScreenParams.xy, Radius);

		float3 sourceColor = screenColor.rgb;
		
		#ifdef URP //Effect can only execute before Bloom, treat as LDR
		//sourceColor = saturate(sourceColor);
		#endif
		
		float3 sharpenedColor = screenColor.rgb + (sourceColor.rgb - average) * Sharpening;

		return float4(sharpenedColor.rgb, screenColor.a);
	}

	//https://github.com/GPUOpen-Effects/FidelityFX-CAS
	// LICENSE
	// =======
	// Copyright (c) 2017-2019 Advanced Micro Devices, Inc. All rights reserved.
	// -------
	// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
	// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
	// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
	// Software is furnished to do so, subject to the following conditions:
	// -------
	// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
	// Software.
	// -------
	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
	// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE AUTHORS OR
	// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
	// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
	
	float4 FragContrastAdaptive(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UV_VR;
		float4 screenColor = ScreenColor(UV_VR);

		float2 radius = 1.0 / _ScreenParams.xy * Radius;
		// fetch a 3x3 neighborhood around the pixel 'e',
		// a b c
		// d(e)f
		// g h i
		float3 a = ScreenColor(uv + float2(-radius.x, -radius.y)).rgb;
		float3 b = ScreenColor(uv + float2(0, -radius.y)).rgb;
		float3 c = ScreenColor(uv + float2(radius.x, -radius.y)).rgb;
		float3 d = ScreenColor(uv + float2(-radius.x, 0)).rgb;
		float3 f = ScreenColor(uv + float2(radius.x, 0)).rgb;
		float3 g = ScreenColor(uv + float2(-radius.x, radius.y)).rgb;
		float3 h = ScreenColor(uv + float2(0, radius.y)).rgb;
		float3 iB = ScreenColor(uv + float2(radius.x, radius.y)).rgb;

		// Soft min and max.
		// a b c b
		// d e f * 0.5 + d e f * 0.5
		// g h i h
		// These are 2.0x bigger (factored out the extra multiply).
		float3 mnRGB = min(min(min(d, screenColor.rgb), min(f, b)), h);
		float3 mnRGB2 = min(mnRGB, min(min(a, c), min(g, iB)));
		mnRGB += mnRGB2;

		float3 mxRGB = max(max(max(d, screenColor.rgb), max(f, b)), h);
		float3 mxRGB2 = max(mxRGB, max(max(a, c), max(g, iB)));
		mxRGB += mxRGB2;

		// Smooth minimum distance to signal limit divided by smooth max.
		float3 rcpMRGB = rcp3(mxRGB);
		float3 ampRGB = saturate(min(mnRGB, 2.0 - mxRGB) * rcpMRGB);

		// Shaping amount of sharpening.
		ampRGB = rsqrt(ampRGB);

		float peak = 8.0 - 3.0 * Contrast;
		float3 wRGB = -rcp3(ampRGB * peak);

		float3 rcpWeightRGB = rcp3(1.0 + 4.0 * wRGB);

		// 0 w 0
		// Filter shape: w 1 w
		// 0 w 0
		float3 window = (b + d) + (f + h);
		float3 sharpenedColor = (window * wRGB + screenColor.rgb) * rcpWeightRGB;
		
		return float4(lerp(screenColor.rgb, sharpenedColor, Sharpening), screenColor.a);
	}


	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Sharpen (Luminance Enhancement)"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragLuminanceEnhancement

			ENDHLSL
		}
		Pass
		{
			Name "Sharpen (Contrast Adaptive)"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragContrastAdaptive

			ENDHLSL
		}
	}
}