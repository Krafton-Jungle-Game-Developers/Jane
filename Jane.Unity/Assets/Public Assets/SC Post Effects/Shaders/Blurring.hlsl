#include "Pipeline/Pipeline.hlsl"

float4 _BlurOffsets; //Always present in blur passes

struct v2fGaussian {
	float4 positionCS : POSITION;
	float2 texcoord : TEXCOORD0;

	float4 uv01 : TEXCOORD1;
	float4 uv23 : TEXCOORD2;
	float4 uv45 : TEXCOORD3;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2fGaussian VertGaussian(Attributes v) {
	v2fGaussian o;

	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	o.positionCS = OBJECT_TO_CLIP(v);

	o.texcoord = GET_TRIANGLE_UV(v);
	o.texcoord = FlipUV(o.texcoord);
	o.texcoord = GET_TRIANGLE_UV_VR(o.texcoord, v.vertexID);

	o.uv01 = o.texcoord.xyxy + _BlurOffsets.xyxy * float4(1, 1, -1, -1);
	o.uv23 = o.texcoord.xyxy + _BlurOffsets.xyxy * float4(1, 1, -1, -1) * 2.0;
	o.uv45 = o.texcoord.xyxy + _BlurOffsets.xyxy * float4(1, 1, -1, -1) * 6.0;

	return o;
}

half4 BoxFilter4(RT_ARG(textureName, samplerTex), float2 uv, float2 texelSize, float amount)
{
	//return float4(0, 1, 0, 0);
	float4 d = texelSize.xyxy * float4(-amount, -amount, amount, amount);

	half4 s;
	s = (SAMPLE_RT(textureName, samplerTex, (uv + d.xy)));
	s += (SAMPLE_RT(textureName, samplerTex, (uv + d.zy)));
	s += (SAMPLE_RT(textureName, samplerTex, (uv + d.xw)));
	s += (SAMPLE_RT(textureName, samplerTex, (uv + d.zw)));

	return s * 0.25h;
}

#ifdef URP
// Standard box filtering
half4 UpsampleBox(RT_ARG(tex, samplerTex), float2 uv, float2 texelSize, float4 sampleScale)
{
	float4 d = texelSize.xyxy * float4(-1.0, -1.0, 1.0, 1.0) * (sampleScale * 0.5);

	half4 s;
	s = (SAMPLE_RT(tex, samplerTex, (uv + d.xy)));
	s += (SAMPLE_RT(tex, samplerTex, (uv + d.zy)));
	s += (SAMPLE_RT(tex, samplerTex, (uv + d.xw)));
	s += (SAMPLE_RT(tex, samplerTex, (uv + d.zw)));

	return s * (1.0 / 4.0);
}
#endif

float4 FragBlurBox(Varyings i) : SV_Target
{
	STEREO_EYE_INDEX_POST_VERTEX(i);

	return BoxFilter4(RT_PARAM(_MainTex, sampler_MainTex), UV, _BlurOffsets.xy, 1.0).rgba;

}

float4 FragBlurGaussian(v2fGaussian i) : SV_Target
{
	STEREO_EYE_INDEX_POST_VERTEX(i);

	half4 color = float4(0, 0, 0, 0);

	color += 0.40 * SAMPLE_RT(_MainTex, sampler_MainTex, i.texcoord);
	color += 0.15 * SAMPLE_RT(_MainTex, sampler_MainTex, i.uv01.xy);
	color += 0.15 * SAMPLE_RT(_MainTex, sampler_MainTex, i.uv01.zw);
	color += 0.10 * SAMPLE_RT(_MainTex, sampler_MainTex, i.uv23.xy);
	color += 0.10 * SAMPLE_RT(_MainTex, sampler_MainTex, i.uv23.zw);
	color += 0.05 * SAMPLE_RT(_MainTex, sampler_MainTex, i.uv45.xy);
	color += 0.05 * SAMPLE_RT(_MainTex, sampler_MainTex, i.uv45.zw);

	return color;
}