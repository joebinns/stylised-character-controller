Shader "DynamicShadowProjector/Projector/Mipmapped Shadow" {
	Properties {
		[NoScaleOffset] _ShadowTex ("Cookie", 2D) = "gray" {}
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_Ambient ("Ambient", Range (0.01, 1)) = 0.3
		_DSPMipLevel ("Max Mip Level", float) = 4.0
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
			Blend DstColor Zero
			Offset [_OffsetSlope], [_Offset]
			Cull [_DSPCull]

			HLSLPROGRAM
			#pragma vertex DSPProjectorVertMipmap
			#pragma fragment DSPProjectorFragMipmap
			#pragma shader_feature_local _ FSR_PROJECTOR_FOR_LWRP
			#pragma shader_feature_local _ DSP_ALPHACUTOFF DSP_TRANSPARENT DSP_ALPHACUTOFF_AND_TRANSPARENT
			#pragma multi_compile_local _ FSR_RECEIVER 
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma target 3.0
			#define DSP_USE_AMBIENT
			#define DSP_USE_MIPLEVEL
			#include "DSPProjector.cginc"
			#include "DSPMipmappedShadow.cginc"
			ENDHLSL
		}
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}
