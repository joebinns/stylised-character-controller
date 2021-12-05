Shader "Hidden/DynamicShadowProjector/Shadow/Replacement" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[HideInInspector] _DSPMainTex ("Main Texture", 2D) = "white" {}
		[HideInInspector] _DSPBaseAlpha ("Alpha", Vector) = (1,0,0,1)
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0 // One
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0 // Zero
		[HideInInspector] _DSPCull("__cull", Float) = 2.0 // Back
	}

	SubShader {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend [_SrcBlend] [_DstBlend]
			Cull [_DSPCull]
			HLSLPROGRAM
			#pragma multi_compile_local _ _ALPHATEST_ON
			#pragma multi_compile_local _ _ALPHABLEND_ON
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertStandard
			#pragma fragment DSPShadowFragStandard
			#define DSP_SHADOW_FOR_URP
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend [_SrcBlend] [_DstBlend]
			Cull [_DSPCull]
			HLSLPROGRAM
			#pragma multi_compile_local _ _ALPHATEST_ON
			#pragma multi_compile_local _ _ALPHABLEND_ON
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertStandard
			#pragma fragment DSPShadowFragStandard
			#define DSP_SHADOW_FOR_URP
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend [_SrcBlend] [_DstBlend]
			HLSLPROGRAM
			#pragma multi_compile_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertStandard
			#pragma fragment DSPShadowFragStandard
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
	SubShader {
		Tags { "RenderType" = "Transparent" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha
			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertTrans
			#pragma fragment DSPShadowFragTrans
			#define _ALPHABLEND_ON
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	} 
	SubShader {
		Tags { "RenderType" = "TransparentCutout" }
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
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
