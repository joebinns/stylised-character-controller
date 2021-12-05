Shader "Projector For LWRP/Visualize Projection Area" 
{
	Properties {
		_Offset ("Offset", Range (0, -10)) = -1.0
		_OffsetSlope ("Offset Slope Factor", Range (0, -1)) = -1.0
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
			Blend SrcAlpha One
			Offset [_OffsetSlope], [_Offset]

			HLSLPROGRAM
			#pragma vertex p4lwrp_vert_projector
			#pragma fragment frag
			#pragma shader_feature_local FSR_PROJECTOR_FOR_LWRP
			#pragma multi_compile_fog
			#include "../Shaders/P4LWRP.cginc"
			float4 frag(float4 uv : TEXCOORD0) : SV_Target
			{
				float4 color = float4(1,0,0,0.5);
				uv.xy /= uv.w;
				if (0 <= uv.x && uv.x <= 1 && 0 <= uv.y && uv.y <= 1)
				{
					color.g = 1;
				}
				return color;
			}
			ENDHLSL
		}
	} 
}
