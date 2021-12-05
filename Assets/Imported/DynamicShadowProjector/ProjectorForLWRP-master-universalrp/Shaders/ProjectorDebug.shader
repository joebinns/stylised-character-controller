Shader "Hidden/Projector For LWRP/Debug"
{
	SubShader
	{
		Tags {"Queue"="Transparent-1"}
		// Shader code
		Pass
		{
			ZWrite Off
			Fog { Color (1, 1, 1) }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			HLSLPROGRAM
			#pragma vertex p4lwrp_vert_projector
			#pragma fragment frag
			#pragma shader_feature_local FSR_PROJECTOR_FOR_LWRP
			#pragma multi_compile_fog
			#include "P4LWRP.cginc"
			fixed4 frag(P4LWRP_V2F_PROJECTOR i) : SV_Target
			{
				fixed alpha = step(0, i.uvShadow.z);
				half2 uv = i.uvShadow.xy / i.uvShadow.w;
				alpha *= step(0, uv.x);
				alpha *= step(0, uv.y);
				alpha *= step(uv.x, 1);
				alpha *= step(uv.y, 1);
				return fixed4(1.0f, 0.0f, 1.0f, alpha);
			}
			ENDHLSL
		}
	} 
}
