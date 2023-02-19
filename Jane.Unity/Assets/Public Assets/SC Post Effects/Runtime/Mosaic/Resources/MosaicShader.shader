Shader "Hidden/SC Post Effects/Mosaic"
{
		HLSLINCLUDE

		#include "../../../Shaders/Pipeline/Pipeline.hlsl"

		float4 _Params;

		float2 TrianglesUV(float2 uv, float size) 
		{
			float2 coord = floor(uv*size) / size;
			uv -= coord;
			uv *= size;

			//Intersect
			return coord + float2(
				//X
				step(1 - uv.y, uv.x) / (size),
				//Y                                            
				step(uv.x, uv.y) / (size)
			);
		}

		inline float mod(float2 a, float b)
		{
			return (a.xy - (b * floor(a.xy / b))).x;
		}

		float2 triChecker(float2 uv, float size)
		{
			float2 m = mod(uv, size);
			float2 base = uv - m;
			uv = m / size;

			base.x *=  step(uv.x, uv.y);

			return base;
		}


		float2 SquareUV(float2 uv, float size) {
			float dx = 10*(1.0 /size);
			float dy = 10*(1.0 / size);
			float2 coord = float2(dx*floor(uv.x / dx),
				dy*floor(uv.y / dy));

			return coord;
		}

		//Translated with persmission from http://coding-experiments.blogspot.nl/2010/06/pixelation.html
		//Modified for Unity
		float2 HexUV(float2 hexIndex) {
			int i = hexIndex.x ;
			int j = hexIndex.y;
			float2 r;
			r.x = i * _Params.x ;
			r.y = j * _Params.y + (i % 2.0) * _Params.y / 2.0;
			return r;
		}

		float2 HexIndex(float2 uv, float size) {

			float2 r;

			int it = int(floor(uv.x / size));
			float yts = uv.y - float(it % 2.0) * _Params.y / 2.0;
			int jt = int(floor((1.0 / _Params.y) * yts));
			float xt = uv.x - it * size;
			float yt = yts - jt * _Params.y;
			int deltaj = (yt > _Params.y / 2.0) ? 1 : 0;
			float fcond = size * (2.0 / 3.0) * abs(0.5 - yt / _Params.y);

			if (xt > fcond) {
				r.x = it;
				r.y = jt;
			}
			else {
				r.x = it - 1;
				r.y = jt - (r.x % 2) + deltaj;
			}

			return r;
		}

		float4 FragTriangles(Varyings i) : SV_Target
		{
			STEREO_EYE_INDEX_POST_VERTEX(i);

			return SCREEN_COLOR(TrianglesUV(UV_VR, _Params.x));
		}

		float4 FragHex(Varyings i) : SV_Target
		{
			STEREO_EYE_INDEX_POST_VERTEX(i);

			return SCREEN_COLOR(HexUV(HexIndex(UV_VR, _Params.x)));
		}

		float4 FragCircle(Varyings i) : SV_Target
		{
			STEREO_EYE_INDEX_POST_VERTEX(i);

			float circleSize = 1.0 / _Params.x;

			float aspect = _ScreenParams.y / _ScreenParams.x;
			float2 uv = UV_VR;
			uv.x = uv.x / aspect;

			float2 blockPos = floor(uv * _Params.x);
			float2 blockCenter = blockPos * circleSize + circleSize * 0.5;

			float dist = length(uv - blockCenter) * _Params.x;

			blockCenter.x /= _ScreenParams.x / _ScreenParams.y;
			float4 screenColor = SCREEN_COLOR(blockCenter);

			if (dist > 0.45)  screenColor = 0;
			//if (dist > 0.45 || dist < 0.23) return 0;

			return screenColor;
		}


		ENDHLSL

	SubShader
		{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Mosaic: Triangles"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragTriangles

			ENDHLSL
		}

		Pass
		{
			Name "Mosaic: Hex"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragHex

			ENDHLSL
		}
		Pass
		{
			Name "Mosaic: Circle"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragCircle

			ENDHLSL
		}
	}
}