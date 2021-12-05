Shader "DynamicShadowProjector/Shadow/Transparent Cutout" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BaseColor ("Color", Color) = (0.5,0.5,0.5,1)
		_BaseMap ("Albedo", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _Cull("__cull", Float) = 2.0 // Back
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
            Blend [_SrcBlend] [_DstBlend]
			Cull [_Cull]
		
			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			#define _MainTex_ST _BaseMap_ST
			#define _MainTex _BaseMap
			#define _Color _BaseColor
			#define _ALPHATEST_ON
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
	SubShader {
		Tags { "RenderPipeline" = "LightweightPipeline" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
            Blend [_SrcBlend] [_DstBlend]
			Cull [_Cull]
		
			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			#define _MainTex_ST _BaseMap_ST
			#define _MainTex _BaseMap
			#define _Color _BaseColor
			#define _ALPHATEST_ON
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Cull [_Cull]
		
			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			#define _ALPHATEST_ON
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
}
