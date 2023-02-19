Shader "Hidden/SC Post Effects/Color Split"
{
	HLSLINCLUDE

#include "../../../Shaders/Pipeline/Pipeline.hlsl"
#include "../../../Shaders/Blurring.hlsl"

		float _Offset;

	Varyings2 VertSingle(Attributes v)
	{
		Varyings2 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		
		o.positionCS = OBJECT_TO_CLIP(v);

		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);

		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, v.vertexID);

		o.texcoord[0].xy = uv;
		o.texcoord[0].zw = uv - float2(_Offset, 0);
		o.texcoord[1].xy = uv + float2(_Offset, 0);
		o.texcoord[1].zw = 0;

		return o;
	}

	float4 FragSingle(Varyings2 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float red = ScreenColor(i.texcoord[0].zw).r;
		float4 original = SCREEN_COLOR(i.texcoord[0].xy);
		float blue = ScreenColor(i.texcoord[1].xy).b;

		float4 splitColors = float4(red, original.g, blue, original.a);

		return splitColors;
	}

	float4 FragSingleBoxFiltered(Varyings2 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float red = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[0].zw, _MainTex_TexelSize.xy * (_Offset * 200), 1.0).r;
		float4 original = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[0].xy, _MainTex_TexelSize.xy * (_Offset * 200), 1.0);
		float blue = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[1].xy, _MainTex_TexelSize.xy * (_Offset * 200), 1.0).b;

		float4 splitColors = float4(red, original.g, blue, original.a);

		return splitColors;
	}

	Varyings3 VertDouble(Attributes v)
	{
		Varyings3 o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		
		o.positionCS = OBJECT_TO_CLIP(v);;

		float2 uv = GET_TRIANGLE_UV(v);

		uv = FlipUV(uv);

		//UNITY_SINGLE_PASS_STEREO
		uv = GET_TRIANGLE_UV_VR(uv, v.vertexID);

		o.texcoord[0].xy = uv;

		//X
		o.texcoord[0].zw = uv - float2(_Offset, 0);
		o.texcoord[1].xy = uv + float2(_Offset, 0);

		//Y
		o.texcoord[1].zw = uv - float2(0, _Offset);
		o.texcoord[2].xy = uv + float2(0, _Offset);
		o.texcoord[2].zw = 0;

		o.texcoordStereo = GET_TRIANGLE_UV_VR(uv, v.vertexID);

		return o;
	}

	float4 FragDouble(Varyings3 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float redX = ScreenColor(i.texcoord[0].zw).r;
		float redY = ScreenColor(i.texcoord[1].zw).r;

		float4 original = SCREEN_COLOR(i.texcoord[0].xy);

		float blueX = ScreenColor(i.texcoord[1].xy).b;
		float blueY = ScreenColor(i.texcoord[2].xy).b;


		float4 splitColorsX = float4(redX, original.g, blueX, original.a);
		float4 splitColorsY = float4(redY, original.g, blueY, original.a);

		float4 blendedColors = (splitColorsX + splitColorsY) / 2;

		return blendedColors;
	}

	float4 FragDoubleBoxFiltered(Varyings3 i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 redX = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[0].zw, _MainTex_TexelSize.xy * (_Offset * 200), 1.0);
		float4 redY = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[1].zw, _MainTex_TexelSize.xy * (_Offset * 200), 1.0);

		float4 original = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[0].xy, _MainTex_TexelSize.xy * (_Offset * 200), 1.0);

		float4 blueX = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[1].xy, _MainTex_TexelSize.xy * (_Offset * 200), 1.0);
		float4 blueY = BoxFilter4(RT_PARAM(_MainTex, Clamp), i.texcoord[2].xy, _MainTex_TexelSize.xy * (_Offset * 200), 1.0);


		float4 splitColorsX = float4(redX.r, original.g, blueX.b, original.a);
		float4 splitColorsY = float4(redY.r, original.g, blueY.b, original.a);

		float4 blendedColors = (splitColorsX + splitColorsY) / 2;

		return blendedColors;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Color Split: Single"
			HLSLPROGRAM

			#pragma vertex VertSingle
			#pragma fragment FragSingle

			ENDHLSL
		}
		Pass
		{
			Name "Color Split: Single Box Filtered"
			HLSLPROGRAM

			#pragma vertex VertSingle
			#pragma fragment FragSingleBoxFiltered

			ENDHLSL
		}
		Pass
		{
			Name "Color Split: Double"
			HLSLPROGRAM

			#pragma vertex VertDouble
			#pragma fragment FragDouble

			ENDHLSL
		}
		Pass
		{
			Name "Color Split: Double Box Filtered"
			HLSLPROGRAM

			#pragma vertex VertDouble
			#pragma fragment FragDoubleBoxFiltered

			ENDHLSL
		}
	}
}