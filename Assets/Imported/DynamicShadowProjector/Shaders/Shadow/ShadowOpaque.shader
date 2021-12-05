Shader "DynamicShadowProjector/Shadow/Opaque" {
	Properties {
		[HideInInspector] _Cull("__cull", Float) = 2.0 // Back
	}
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
			Cull [_Cull]
		
			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma vertex DSPShadowVertOpaque
			#pragma fragment DSPShadowFragOpaque
			#include "DSPShadow.cginc"
			ENDHLSL
		}
	}
}
