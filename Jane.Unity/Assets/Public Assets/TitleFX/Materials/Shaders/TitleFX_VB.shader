Shader "TitleFX_VB"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_XTiling("XTiling", Float) = 1.2
		_YTiling("YTiling", Float) = 1
		_Main("Main", 2D) = "white" {}
		[HDR]_MainColor("MainColor", Color) = (1.308841,1.308841,1.308841,1)
		_MainMASK("MainMASK", 2D) = "white" {}
		_TransitionFactor("TransitionFactor", Range( -1 , 1)) = 1
		_DetailsMASK("DetailsMASK", 2D) = "white" {}
		_DetailsMaskDistortionMult("DetailsMask Distortion Mult", Range( 0 , 1)) = 1
		[Toggle(_INVERSEDIRECTION_ON)] _InverseDirection("InverseDirection", Float) = 0
		[Toggle(_FORMAT_ON)] _Format("1:1 | 1:2 Format", Float) = 0
		[Toggle]_AutoManualAnimation("Auto/Manual Animation", Float) = 0
		[Toggle(_UPDOWNDIRECTION_ON)] _UpDownDirection("Up/Down Direction", Float) = 1
		_Animation_Factor("Animation_Factor", Range( 0 , 2)) = 0
		_TransitionSpeed("Transition Speed", Range( 2 , 5)) = 2
		_VignetteMaskFallof("Vignette Mask Fallof", Range( 0 , 0.5)) = 0.25
		[ASEEnd]_VignetteMaskSize("VignetteMaskSize", Range( 0 , 1)) = 0.4

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"
			#pragma shader_feature_local _FORMAT_ON
			#pragma shader_feature_local _INVERSEDIRECTION_ON
			#pragma shader_feature_local _UPDOWNDIRECTION_ON

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform sampler2D _Main;
			uniform sampler2D _MainMASK;
			uniform float _XTiling;
			uniform float _YTiling;
			uniform float _AutoManualAnimation;
			uniform float _Animation_Factor;
			uniform float _TransitionFactor;
			uniform float _TransitionSpeed;
			uniform sampler2D _DetailsMASK;
			uniform float _DetailsMaskDistortionMult;
			uniform float4 _MainColor;
			uniform float _VignetteMaskSize;
			uniform float _VignetteMaskFallof;
			float4 MyCustomExpression215( float3 c, float a )
			{
				float4 colors = float4(c.x,c.y,c.z,a);
				return colors;
			}
			

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 texCoord2 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _FORMAT_ON
				float staticSwitch263 = 2.0;
				#else
				float staticSwitch263 = 1.0;
				#endif
				#ifdef _FORMAT_ON
				float2 staticSwitch265 = float2( 0.5,1 );
				#else
				float2 staticSwitch265 = float2( 1,1 );
				#endif
				float2 temp_output_259_0 = ( staticSwitch263 * staticSwitch265 );
				float2 UV209 = ( ( ( texCoord2 - float2( 0.5,0 ) ) * temp_output_259_0 ) + float2( 0.5,0 ) );
				float2 appendResult186 = (float2(_XTiling , _YTiling));
				float2 temp_output_33_0 = ( UV209 * appendResult186 );
				float Animation221 = (( _AutoManualAnimation )?( _Animation_Factor ):( _SinTime.w ));
				float temp_output_180_0 = ( 1.0 * Animation221 );
				#ifdef _UPDOWNDIRECTION_ON
				float2 staticSwitch256 = float2( 0,1 );
				#else
				float2 staticSwitch256 = float2( 1,0 );
				#endif
				#ifdef _INVERSEDIRECTION_ON
				float2 staticSwitch258 = ( staticSwitch256 * float2( -1,-1 ) );
				#else
				float2 staticSwitch258 = staticSwitch256;
				#endif
				float2 temp_output_157_0 = pow( ( temp_output_180_0 * staticSwitch258 ) , 2.0 );
				float ifLocalVar117 = 0;
				if( temp_output_180_0 <= 0.0 )
				ifLocalVar117 = temp_output_180_0;
				else
				ifLocalVar117 = ( temp_output_180_0 * -1.0 );
				float temp_output_39_0 = ( ( ifLocalVar117 + 0.5 ) * _TransitionSpeed );
				float clampResult45 = clamp( ( ( tex2D( _MainMASK, ( temp_output_33_0 + temp_output_157_0 ) ).r * _TransitionFactor ) - temp_output_39_0 ) , 0.0 , 1.0 );
				float clampResult41 = clamp( ( ( tex2D( _DetailsMASK, ( temp_output_33_0 + temp_output_157_0 ) ).r * _DetailsMaskDistortionMult ) - temp_output_39_0 ) , 0.0 , 1.0 );
				float temp_output_46_0 = ( clampResult45 + clampResult41 );
				float MaskData187 = temp_output_46_0;
				#ifdef _FORMAT_ON
				float staticSwitch261 = 0.75;
				#else
				float staticSwitch261 = 0.5;
				#endif
				float2 temp_output_52_0 = ( ( UV209 + ( MaskData187 * staticSwitch261 * staticSwitch258 ) ) - ( staticSwitch258 * temp_output_259_0 ) );
				float4 tex2DNode1 = tex2D( _Main, temp_output_52_0 );
				float temp_output_51_0 = ( tex2DNode1.r * pow( ( temp_output_46_0 * 0.5 ) , 2.0 ) );
				float4 clampResult253 = clamp( ( ( ( 1.0 - ( temp_output_51_0 - 1.5 ) ) * ( temp_output_51_0 - 1.6 ) ) * _MainColor * 8.0 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float3 c215 = ( ( tex2DNode1 * _MainColor ) + clampResult253 ).rgb;
				float2 texCoord279 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (( _VignetteMaskSize * 0.5 )).xx;
				float clampResult301 = clamp( ( distance( max( ( abs( ( texCoord279 - float2( 0.5,0.5 ) ) ) - temp_cast_1 ) , float2( 0,0 ) ) , float2( 0,0 ) ) / _VignetteMaskFallof ) , 0.0 , 1.0 );
				float a215 = ( temp_output_51_0 * ( 1.0 - clampResult301 ) );
				float4 localMyCustomExpression215 = MyCustomExpression215( c215 , a215 );
				float4 clampResult224 = clamp( localMyCustomExpression215 , float4( 0,0,0,0 ) , float4( 10000,10000,10000,10000 ) );
				
				half4 color = clampResult224;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "Title_fx_GUI_V2"
}
