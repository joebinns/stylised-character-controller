Shader "DynamicShadowProjector/Projector/Light x Shadow With Linear Falloff" {
	Properties {
		[NoScaleOffset] _LightTex ("Light Cookie", 2D) = "gray" {}
		[NoScaleOffset] _ShadowTex ("Shadow Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Light Intensity", Range (0, 10)) = 1.0
		_Offset ("Offset", Range (-10, 0)) = -1.0
		_OffsetSlope ("Offset Slope Factor", Range (-1, 0)) = -1.0
		[HideInInspector] _DSPMainTex ("Receiver Texture", 2D) = "white" {}
		[HideInInspector] _DSPBaseAlpha ("Receiver Alpha", Vector) = (1,0,0,1)
		[HideInInspector] _DSPCull("__cull", Float) = 2.0 // Back
	}
	Subshader {
		Tags {"Queue"="Transparent-1"}
		Pass {
			Name "PASS"
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset [_OffsetSlope], [_Offset]
			Cull [_DSPCull]

			HLSLPROGRAM
			#pragma vertex DSPProjectorVertLightLinearFalloff
			#pragma fragment DSPProjectorFragLightWithShadow
			#pragma shader_feature_local _ FSR_PROJECTOR_FOR_LWRP
			#pragma shader_feature_local _ DSP_ALPHACUTOFF DSP_TRANSPARENT DSP_ALPHACUTOFF_AND_TRANSPARENT
			#pragma multi_compile_local _ FSR_RECEIVER 
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include "DSPProjector.cginc"
			ENDHLSL
		}
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}
