Shader "Hidden/SC Post Effects/DepthNormals"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#include "Pipeline/Pipeline.hlsl"

	// Reconstruct view-space position from UV and depth.
	// p11_22 = (unity_CameraProjection._11, unity_CameraProjection._22)
	// p13_31 = (unity_CameraProjection._13, unity_CameraProjection._23)
	float3 ReconstructViewPos(float3 uvDepth, float2 p11_22, float2 p13_31)
	{
		return float3(((uvDepth.xy * 2.0 - 1.0 - p13_31) / p11_22) * CheckPerspective(uvDepth.z), uvDepth.z);
	}

	float4 FastReconstruction(float depth, float2 uv)
	{
		// Parameters used in coordinate conversion
		float3x3 camProj = (float3x3)unity_CameraProjection;
		float2 p11_22 = float2(camProj._11, camProj._22);
		float2 p13_31 = float2(camProj._13, camProj._23);

		float3 P0 = ReconstructViewPos(float3(uv, LINEAR_EYE_DEPTH(depth)), p11_22, p13_31);
		float4 normals = float4(normalize(cross(ddy(P0), ddx(P0))), depth);

		return normals;
	}

	float4 AccurateReconstruction(float depth, float2 uv)
	{
		float2 delta = _ScreenParams.zw - 1.0;

		// Sample the neighbour fragments
		float2 lUV = float2(-delta.x, 0.0);
		float2 rUV = float2(delta.x, 0.0);
		float2 uUV = float2(0.0, delta.y);
		float2 dUV = float2(0.0, -delta.y);

		float3 c = float3(uv, 0.0); c.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(c.xy)); // Center
		float3 l1 = float3(uv + lUV, 0.0); l1.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(l1.xy)); // Left1
		float3 r1 = float3(uv + rUV, 0.0); r1.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(r1.xy)); // Right1
		float3 u1 = float3(uv + uUV, 0.0); u1.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(u1.xy)); // Up1
		float3 d1 = float3(uv + dUV, 0.0); d1.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(d1.xy)); // Down1

#if defined(_RECONSTRUCT_NORMAL_MEDIUM)
		uint closest_horizontal = l1.z > r1.z ? 0 : 1;
		uint closest_vertical = d1.z > u1.z ? 0 : 1;
#else
		float3 l2 = float3(uv + lUV * 2.0, 0.0); l2.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(l2.xy)); // Left2
		float3 r2 = float3(uv + rUV * 2.0, 0.0); r2.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(r2.xy)); // Right2
		float3 u2 = float3(uv + uUV * 2.0, 0.0); u2.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(u2.xy)); // Up2
		float3 d2 = float3(uv + dUV * 2.0, 0.0); d2.z = LINEAR_EYE_DEPTH(SAMPLE_DEPTH(d2.xy)); // Down2

		const uint closest_horizontal = abs((2.0 * l1.z - l2.z) - c.z) < abs((2.0 * r1.z - r2.z) - c.z) ? 0 : 1;
		const uint closest_vertical = abs((2.0 * d1.z - d2.z) - c.z) < abs((2.0 * u1.z - u2.z) - c.z) ? 0 : 1;
#endif

		// Parameters used in coordinate conversion
		float3x3 camProj = (float3x3)unity_CameraProjection;
		float2 p11_22 = float2(camProj._11, camProj._22);
		float2 p13_31 = float2(camProj._13, camProj._23);

		// Calculate the triangle, in a counter-clockwize order, to
		// use based on the closest horizontal and vertical depths.
		// h == 0.0 && v == 0.0: p1 = left,  p2 = down
		// h == 1.0 && v == 0.0: p1 = down,  p2 = right
		// h == 1.0 && v == 1.0: p1 = right, p2 = up
		// h == 0.0 && v == 1.0: p1 = up,    p2 = left
		// Calculate the view space positions for the three points...
		float3 P0 = ReconstructViewPos(c, p11_22, p13_31);
		float3 P1;
		float3 P2;
		if (closest_vertical == 0)
		{
			P1 = ReconstructViewPos((closest_horizontal == 0 ? l1 : d1), p11_22, p13_31);
			P2 = ReconstructViewPos((closest_horizontal == 0 ? d1 : r1), p11_22, p13_31);
		}
		else
		{
			P1 = ReconstructViewPos((closest_horizontal == 0 ? u1 : r1), p11_22, p13_31);
			P2 = ReconstructViewPos((closest_horizontal == 0 ? l1 : u1), p11_22, p13_31);
		}

		// Use the cross product to calculate the normal...
		return float4(normalize(cross(P2 - P0, P1 - P0)), depth);
	}

	//https://github.com/Unity-Technologies/Graphics/blob/d7e4eb9266bd768669a016b1549b67489841f847/com.unity.render-pipelines.universal/ShaderLibrary/SSAO.hlsl
	float4 Frag(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);
		float depth = LINEAR_DEPTH(SAMPLE_DEPTH(UV_VR));

		//return FastReconstruction(depth, UV_VR);
		return AccurateReconstruction(depth, UV_VR);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			Name "Depth Normals reconstruction"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}