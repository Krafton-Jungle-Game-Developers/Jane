Shader "Hidden/SC Post Effects/3D Hue Shift"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH_NORMALS
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_GradientTex);

	float4 _Params;
	//x: Speed;
	//y: Size
	//z: Geometry influence
	//w: Intensity
	
	// Simple circular wave function
	float RadialWave(float2 pos, float2 center)
	{
		float d = length(pos - center);
		d = log(1 + exp(d));
		return 1.0 / (1.0 + 0.0 * d * d) * sin(2.0 * 3.1415 * ( - _Params.y * d + (_Time.x * _Params.x * 3)));
	}
	
	float3 mod289(float3 x)
	{
		return x - floor(x * (1.0 / 289.0)) * 289.0;
	}

	float4 mod289(float4 x)
	{
		return x - floor(x * (1.0 / 289.0)) * 289.0;
	}

	float4 permute(float4 x)
	{
		return mod289((x * 34.0 + 1.0) * x);
	}

	float4 taylorInvSqrt(float4 r)
	{
		return 1.79284291400159 - 0.85373472095314 * r;
	}

	//3D Simplex Perlin noise
	float snoise(float3 v)
	{
		const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);

		// First corner
		float3 i = floor(v + dot(v, C.yyy));
		float3 x0 = v - i + dot(i, C.xxx);

		// Other corners
		float3 g = step(x0.yzx, x0.xyz);
		float3 l = 1.0 - g;
		float3 i1 = min(g.xyz, l.zxy);
		float3 i2 = max(g.xyz, l.zxy);

		// x1 = x0 - i1  + 1.0 * C.xxx;
		// x2 = x0 - i2  + 2.0 * C.xxx;
		// x3 = x0 - 1.0 + 3.0 * C.xxx;
		float3 x1 = x0 - i1 + C.xxx;
		float3 x2 = x0 - i2 + C.yyy;
		float3 x3 = x0 - 0.5;

		// Permutations
		i = mod289(i); // Avoid truncation effects in permutation
		float4 p = permute(permute(permute(i.z + float4(0.0, i1.z, i2.z, 1.0))
		+ i.y + float4(0.0, i1.y, i2.y, 1.0))
		+ i.x + float4(0.0, i1.x, i2.x, 1.0));

		// Gradients: 7x7 points over a square, mapped onto an octahedron.
		// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
		float4 j = p - 49.0 * floor(p * (1.0 / 49.0));  // mod(p,7*7)

		float4 x_ = floor(j * (1.0 / 7.0));
		float4 y_ = floor(j - 7.0 * x_);  // mod(j,N)

		float4 x = x_ * (2.0 / 7.0) + 0.5 / 7.0 - 1.0;
		float4 y = y_ * (2.0 / 7.0) + 0.5 / 7.0 - 1.0;

		float4 h = 1.0 - abs(x) - abs(y);

		float4 b0 = float4(x.xy, y.xy);
		float4 b1 = float4(x.zw, y.zw);

		float4 s0 = floor(b0) * 2.0 + 1.0;
		float4 s1 = floor(b1) * 2.0 + 1.0;
		float4 sh = - step(h, float4(0, 0, 0, 0));

		float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
		float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

		float3 g0 = float3(a0.xy, h.x);
		float3 g1 = float3(a0.zw, h.y);
		float3 g2 = float3(a1.xy, h.z);
		float3 g3 = float3(a1.zw, h.w);

		// Normalise gradients
		float4 norm = taylorInvSqrt(float4(dot(g0, g0), dot(g1, g1), dot(g2, g2), dot(g3, g3)));
		g0 *= norm.x;
		g1 *= norm.y;
		g2 *= norm.z;
		g3 *= norm.w;

		// Mix final noise value
		float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
		m = m * m;
		m = m * m;

		float4 px = float4(dot(x0, g0), dot(x1, g1), dot(x2, g2), dot(x3, g3));
		return 42.0 * dot(m, px);
	}


	struct v2f
	{
		float4 positionCS : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		float2 texcoordStereo : TEXCOORD1;
		float3 noiseCoords : TEXCOORD2;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f VertNoise(Attributes v)
	{
		v2f o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);
		o.texcoord = GET_TRIANGLE_UV(v);
		o.texcoord = FlipUV(o.texcoord);
		o.texcoordStereo = GET_TRIANGLE_UV_VR(o.texcoord, v.vertexID);

		o.noiseCoords = float3(o.texcoordStereo.x * _Params.y, o.texcoordStereo.y * _Params.y, (_Time.y * _Params.x));

		return o;
	}


	//x: Speed;
	//y: Size
	//z: Geometry influence
	//w: Intensity
	float4 FragSpectrum(v2f i): SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = i.texcoordStereo;

		half4 normals = SAMPLE_DEPTH_NORMALS(uv);

		half noise = 0.5 + 0.5 * snoise(i.noiseCoords + ( normals.rgb * _Params.z));
		half waveSize = 0.5;

		//Add waves
		noise = noise + RadialWave(i.texcoord * waveSize, float2(0.5, 0.5) * waveSize);

		//HSV
		float3 hsv = float3(noise, 1, 1);
		//RGB
		float4 col = float4(HsvToRgb(hsv).rgb, 1);

		float4 screenColor = ScreenColor(uv);

		float luminance = pow((screenColor.r + screenColor.g + screenColor.b) /3, 1);

		//Overlay
		float3 overlayColor = BlendOverlay(screenColor.rgb, col.rgb);
		
		return lerp(screenColor, float4(overlayColor, screenColor.a) , _Params.w);
	}

	float4 FragGradientTex(v2f i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = i.texcoordStereo;

		half4 normals = SAMPLE_DEPTH_NORMALS(uv);

		half noise = 0.5 + 0.5 * snoise(i.noiseCoords + (normals.rgb * _Params.z));
		half waveSize = 0.5;

		//Add waves
		noise = noise + RadialWave(i.texcoord * waveSize, float2(0.5, 0.5) * waveSize);

		//return noise;
		//HSV
		float3 hsv = float3(noise, 1, 1);
		//RGB
		float4 col = SAMPLE_TEX(_GradientTex, Repeat, float2(noise, 0));

		float4 screenColor = ScreenColor(uv);

		//Overlay
		float3 overlayColor = BlendOverlay(screenColor.rgb, col.rgb);

		return lerp(screenColor, float4(overlayColor, screenColor.a), _Params.w * col.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma multi_compile_local _ _RECONSTRUCT_NORMAL
			#pragma vertex VertNoise
			#pragma fragment FragSpectrum

			ENDHLSL
		}
		Pass
		{
			HLSLPROGRAM
			#pragma multi_compile_local _ _RECONSTRUCT_NORMAL
			#pragma vertex VertNoise
			#pragma fragment FragGradientTex

			ENDHLSL
		}
	}
}