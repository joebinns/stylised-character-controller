Shader "Projector For LWRP/Shadow" 
{
	Properties {
		[NoScaleOffset] _ShadowTex ("Cookie", 2D) = "gray" {}
		[NoScaleOffset] _FalloffTex ("FallOff", 2D) = "white" {}
		_Offset ("Offset", Range (-10, 0)) = -1.0
		_OffsetSlope ("Offset Slope Factor", Range (-1, 0)) = -1.0
	}
	SubShader
	{
		Tags {"Queue"="Transparent-1"}
        // Shader code
		Pass
        {
			ZWrite Off
			Fog { Color (1, 1, 1) }
			ColorMask RGB
			Blend DstColor Zero
			Offset [_OffsetSlope], [_Offset]

			HLSLPROGRAM
			#pragma vertex p4lwrp_vert_projector
			#pragma fragment p4lwrp_frag_projector_shadow
			#pragma shader_feature_local FSR_PROJECTOR_FOR_LWRP
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#include "P4LWRP.cginc"
			ENDHLSL
		}
	} 
}
