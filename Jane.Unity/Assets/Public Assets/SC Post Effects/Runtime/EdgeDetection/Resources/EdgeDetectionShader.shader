Shader "Hidden/SC Post Effects/Edge Detection" {

	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#define REQUIRE_DEPTH_NORMALS
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"
	//Camera depth textures

	//Parameters
	uniform half4 _Sensitivity;
	uniform half _BackgroundFade;
	uniform float _EdgeSize;
	uniform float4 _EdgeColor;
	uniform float _EdgeOpacity;
	uniform float _Exponent;
	uniform float _Threshold;
	float4 _FadeParams;
	//X: Start
	//Y: End
	//Z: Invert
	//W: Enabled

	uniform float4 _SobelParams;

	inline half IsSame(half2 centerNormal, float centerDepth, half4 theSample)
	{
		// difference in normals
		half2 diff = abs(centerNormal - theSample.xy) * _Sensitivity.y;
		half isSameNormal = (diff.x + diff.y) * _Sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG(theSample.zw);
		float zdiff = abs(centerDepth - sampleDepth);
		// scale the required threshold by the distance
		half isSameDepth = zdiff * _Sensitivity.x < 0.09 * centerDepth;

		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise

		return isSameNormal * isSameDepth;
	}

	//TRIANGLE DEPTH NORMALS METHOD
	Varyings2 vertDNormals(Attributes v)
	{
		Varyings2 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);

		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);

		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, 1.0);

		o.texcoord[0].xy = uv;
		o.texcoord[0].zw = uv;
		// offsets for two additional samples
		o.texcoord[0].zw = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _EdgeSize;;
		o.texcoord[1].xy = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _EdgeSize;
		o.texcoord[1].zw = 0;

		return o;
	}

	half4 fragDNormals(Varyings2 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 original = SCREEN_COLOR(i.texcoord[0].xy);

		half4 center = SAMPLE_DEPTH_NORMALS(i.texcoord[0].xy);
		//return center;
		half4 sample1 = SAMPLE_DEPTH_NORMALS(i.texcoord[0].zw);
		half4 sample2 = SAMPLE_DEPTH_NORMALS(i.texcoord[1].xy);

		// encoded normal
		half2 centerNormal = center.xy;
		// decoded depth
		float centerDepth = DecodeFloatRG(center.zw);

		half edge = 1;
		edge *= IsSame(centerNormal, centerDepth, sample1);
		edge *= IsSame(centerNormal, centerDepth, sample2);
		edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BackgroundFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeOpacity * LinearDepthFade(centerDepth, _FadeParams.x, _FadeParams.y, _FadeParams.z, _FadeParams.w));
		edgeColor = saturate(edgeColor);

		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);

	}

	//ROBERTS CROSS DEPTH NORMALS METHOD
	Varyings3 vertRobert(Attributes v)
	{
		Varyings3 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);

		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);

		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, 1.0);

		o.texcoord[0].xy = uv;
		o.texcoord[0].zw = uv;

		o.texcoord[1].xy = uv + _MainTex_TexelSize.xy * half2(1, 1) * _EdgeSize;
		o.texcoord[1].zw = uv + _MainTex_TexelSize.xy * half2(-1, -1) * _EdgeSize;
		o.texcoord[2].xy = uv + _MainTex_TexelSize.xy * half2(-1, 1) * _EdgeSize;
		o.texcoord[2].zw = uv + _MainTex_TexelSize.xy * half2(1, -1) * _EdgeSize;

		return o;
	}

	half4 fragRobert(Varyings3 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 original = SCREEN_COLOR(i.texcoord[0].xy);

		half4 sample1 = SAMPLE_DEPTH_NORMALS(i.texcoord[1].xy);
		half4 sample2 = SAMPLE_DEPTH_NORMALS(i.texcoord[1].zw);
		half4 sample3 = SAMPLE_DEPTH_NORMALS(i.texcoord[2].xy);
		half4 sample4 = SAMPLE_DEPTH_NORMALS(i.texcoord[2].zw);

		float centerDepth = DecodeFloatRG(sample1.zw);
		float depth = LINEAR_DEPTH(centerDepth) * _FadeParams.x;

		half edge = 1.0;

		edge *= IsSame(sample1.xy, DecodeFloatRG(sample1.zw), sample2);
		edge *= IsSame(sample3.xy, DecodeFloatRG(sample3.zw), sample4);

		edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BackgroundFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeOpacity * LinearDepthFade(centerDepth, _FadeParams.x, _FadeParams.y, _FadeParams.z, _FadeParams.w));

		//return original;
		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}

	//SOBEL DEPTH METHOD

	Varyings3 vertSobel(Attributes v)
	{
		Varyings3 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);

		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);

		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, 1.0);

		o.texcoord[0].xy = uv;
		o.texcoord[0].zw = uv;

		float2 uvDist = _EdgeSize * _MainTex_TexelSize.xy;

		//Top-right
		o.texcoord[1].xy = uv + uvDist;
		//Top-left
		o.texcoord[1].zw = uv + uvDist * half2(-1, 1);
		//Bottom-right
		o.texcoord[2].xy = uv - uvDist * half2(-1, 1);
		//Bottom-left
		o.texcoord[2].zw = uv - uvDist;

		return o;
	}

	float4 fragSobel(Varyings3 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		// inspired by borderlands implementation of popular "sobel filter"
		half4 original = SCREEN_COLOR(i.texcoord[0].xy);

		float centerDepth = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[0].xy));
		float4 depthsDiag;
		float4 depthsAxis;

		depthsDiag.x = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[1].xy)); // TR
		depthsDiag.y = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[1].zw)); // TL
		depthsDiag.z = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[2].xy)); // BR
		depthsDiag.w = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[2].zw)); // BL

		float2 uvDist = _EdgeSize * _MainTex_TexelSize.xy;

		depthsAxis.x = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[0].xy + uvDist * half2(0, 1))); // T
		depthsAxis.y = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[0].xy - uvDist * half2(1, 0))); // L
		depthsAxis.z = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[0].xy + uvDist * half2(1, 0))); // R
		depthsAxis.w = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[0].xy - uvDist * half2(0, 1))); // B	

		//Thin edges
		if (_SobelParams.x == 1) {
			depthsDiag = (depthsDiag > centerDepth.xxxx) ? depthsDiag : centerDepth.xxxx;
			depthsAxis = (depthsAxis > centerDepth.xxxx) ? depthsAxis : centerDepth.xxxx;
		}
		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0 - pow(saturate(Sobel), _Exponent);

		float edge = 1 - Sobel;

		//Orthographic camera: Still not correct, but value should be flipped
		if (unity_OrthoParams.w) edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BackgroundFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeOpacity * LinearDepthFade(centerDepth, _FadeParams.x, _FadeParams.y, _FadeParams.z, _FadeParams.w));

		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}

	//TRIANGLE LUMINANCE VARIANCE METHOD

	Varyings2 vertLum(Attributes v)
	{
		Varyings2 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);
		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);
		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, 1.0);

		o.texcoord[0].xy = uv;
		o.texcoord[0].zw = uv;
		o.texcoord[1].xy = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _EdgeSize;
		o.texcoord[1].zw = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _EdgeSize;

		return o;
	}

	float4 fragLum(Varyings2 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 original = SCREEN_COLOR(i.texcoord[0].xy);

		float centerDepth = LINEAR_DEPTH(SAMPLE_DEPTH(i.texcoord[0].xy));
		float depth = centerDepth * _FadeParams.x;
		//return depth;

		half3 p1 = original.rgb;
		half3 p2 = SAMPLE_RT(_MainTex, Clamp, i.texcoord[1].xy).rgb;
		half3 p3 = SAMPLE_RT(_MainTex, Clamp, i.texcoord[1].zw).rgb;

		half3 diff = p1 * 2 - p2 - p3;
		half edge = dot(diff, diff);
		edge = step(edge, _Threshold);

		edge = 1 - edge;

		//Edges only
		original = lerp(original, float4(1, 1, 1, 1), _BackgroundFade);

		//Opacity
		float3 edgeColor = lerp(original.rgb, _EdgeColor.rgb, _EdgeOpacity * LinearDepthFade(centerDepth, _FadeParams.x, _FadeParams.y, _FadeParams.z, _FadeParams.w));
		edgeColor = saturate(edgeColor);

		//return original;
		return float4(lerp(original.rgb, edgeColor.rgb, edge).rgb, original.a);
	}

	ENDHLSL

		//Pass determined by EdgeDetectionMode enum value
		Subshader {
			Pass{
				 ZTest Always Cull Off ZWrite Off
				 Name "Edge Detection: Depth Normals"
				 HLSLPROGRAM
				 #pragma multi_compile_local _ _RECONSTRUCT_NORMAL
				 #ifndef URP 
				 #undef _RECONSTRUCT_NORMAL
				 #endif
				 #pragma vertex vertDNormals
				 #pragma fragment fragDNormals
				ENDHLSL
			}
			Pass{
				 ZTest Always Cull Off ZWrite Off
				 Name "Edge Detection: Cross Depth Normals"
				 HLSLPROGRAM
				 #pragma multi_compile_local _ _RECONSTRUCT_NORMAL
				 #ifndef URP 
				 #undef _RECONSTRUCT_NORMAL
				 #endif
				 #pragma vertex vertRobert
				 #pragma fragment fragRobert
				ENDHLSL
			}
			Pass{
				 ZTest Always Cull Off ZWrite Off
				 Name "Edge Detection: Sobel"
				 HLSLPROGRAM
				 #pragma vertex vertSobel
				 #pragma fragment fragSobel
				ENDHLSL
			}
			Pass{
				 ZTest Always Cull Off ZWrite Off
				 Name "Edge Detection: Luminance"
				 HLSLPROGRAM
				 #pragma vertex vertLum
				 #pragma fragment fragLum
				ENDHLSL
		}
	}

	Fallback off

} // shader
