Shader "TitleFX_VA"
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
		[HDR]_MainColor("MainColor", Color) = (1.308841,1.308841,1.308841,1)
		_Main("Main", 2D) = "white" {}
		[HDR]_OutlineColor("OutlineColor", Color) = (6.198622,1.092255,0.4385816,1)
		_Outline("Outline", 2D) = "white" {}
		_MainMASK("MainMASK", 2D) = "white" {}
		_TransitionFactor("TransitionFactor", Float) = 1
		_DetailsMASK("DetailsMASK", 2D) = "white" {}
		_DetailsMaskDistortionMult("DetailsMask Distortion Mult", Float) = 1
		[Toggle(_INVERSEDIRECTION_ON)] _InverseDirection("InverseDirection", Float) = 0
		[Toggle(_UPDOWNDIRECTION_ON)] _UpDownDirection("Up/Down Direction", Float) = 0
		[Toggle]_AutoManualAnimation("Auto/Manual Animation", Float) = 0
		_TransitionSpeed("Transition Speed", Range( 2 , 5)) = 5
		_VignetteMaskFallof("Vignette Mask Fallof", Range( 0 , 0.5)) = 0.25
		_Animation_Factor("Animation_Factor", Range( 0 , 2)) = 0
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
			uniform float _TransitionFactor;
			uniform float _AutoManualAnimation;
			uniform float _Animation_Factor;
			uniform float _TransitionSpeed;
			uniform sampler2D _DetailsMASK;
			uniform float _DetailsMaskDistortionMult;
			uniform float4 _MainColor;
			uniform sampler2D _Outline;
			uniform float4 _OutlineColor;
			uniform float _VignetteMaskSize;
			uniform float _VignetteMaskFallof;
			float4 MyCustomExpression160( float3 c, float a )
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
				float2 appendResult181 = (float2(_XTiling , _YTiling));
				float2 temp_output_33_0 = ( texCoord2 * appendResult181 );
				float Main_Mask168 = ( tex2D( _MainMASK, temp_output_33_0 ).r * _TransitionFactor );
				float Animation162 = (( _AutoManualAnimation )?( _Animation_Factor ):( _SinTime.w ));
				float ifLocalVar117 = 0;
				if( Animation162 <= 0.0 )
				ifLocalVar117 = Animation162;
				else
				ifLocalVar117 = ( Animation162 * -1.0 );
				float temp_output_39_0 = ( ( ifLocalVar117 + 0.5 ) * _TransitionSpeed );
				float clampResult45 = clamp( ( Main_Mask168 - temp_output_39_0 ) , 0.0 , 1.0 );
				float Details_Mask167 = ( tex2D( _DetailsMASK, temp_output_33_0 ).r * _DetailsMaskDistortionMult );
				float clampResult41 = clamp( ( Details_Mask167 - temp_output_39_0 ) , 0.0 , 1.0 );
				float AllMasks171 = ( clampResult45 + clampResult41 );
				#ifdef _UPDOWNDIRECTION_ON
				float2 staticSwitch190 = float2( 0,1 );
				#else
				float2 staticSwitch190 = float2( 1,0 );
				#endif
				#ifdef _INVERSEDIRECTION_ON
				float2 staticSwitch188 = ( staticSwitch190 * float2( -1,-1 ) );
				#else
				float2 staticSwitch188 = staticSwitch190;
				#endif
				float2 temp_output_53_0 = ( staticSwitch188 * 0.25 );
				float2 _Vector2 = float2(1,1);
				float2 _Vector1 = float2(0,0);
				float4 tex2DNode1 = tex2D( _Main, ( ( ( ( texCoord2 + ( 0.15 * AllMasks171 * staticSwitch188 ) ) - temp_output_53_0 ) * _Vector2 ) + _Vector1 ) );
				float ifLocalVar134 = 0;
				if( Animation162 <= 0.0 )
				ifLocalVar134 = Animation162;
				else
				ifLocalVar134 = ( Animation162 * -1.0 );
				float temp_output_136_0 = ( ( ifLocalVar134 + 0.5 ) * 3.5 );
				float clampResult140 = clamp( ( Main_Mask168 - temp_output_136_0 ) , 0.0 , 1.0 );
				float clampResult139 = clamp( ( Details_Mask167 - temp_output_136_0 ) , 0.0 , 1.0 );
				float temp_output_141_0 = ( clampResult140 + clampResult139 );
				float2 temp_output_156_0 = ( ( ( ( texCoord2 + ( 0.15 * temp_output_141_0 * staticSwitch188 ) ) - temp_output_53_0 ) * _Vector2 ) + _Vector1 );
				float clampResult150 = clamp( temp_output_141_0 , 0.0 , 1.0 );
				float3 c160 = ( ( tex2DNode1.r * _MainColor ) + ( ( tex2D( _Outline, temp_output_156_0 ).r * _OutlineColor ) * ( 1.0 - clampResult150 ) ) ).rgb;
				float clampResult119 = clamp( ( tex2DNode1.r * pow( AllMasks171 , 1.5 ) ) , 0.0 , 1.0 );
				float2 texCoord196 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_1 = (( _VignetteMaskSize * 0.5 )).xx;
				float clampResult208 = clamp( ( distance( max( ( abs( ( texCoord196 - float2( 0.5,0.5 ) ) ) - temp_cast_1 ) , float2( 0,0 ) ) , float2( 0,0 ) ) / _VignetteMaskFallof ) , 0.0 , 1.0 );
				float a160 = ( clampResult119 * ( 1.0 - clampResult208 ) );
				float4 localMyCustomExpression160 = MyCustomExpression160( c160 , a160 );
				
				half4 color = localMyCustomExpression160;
				
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
	CustomEditor "Title_fx_GUI"
}
