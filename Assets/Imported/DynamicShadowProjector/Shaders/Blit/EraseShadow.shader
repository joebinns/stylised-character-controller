Shader "DynamicShadowProjector/Blit/EraseShadow" {
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode Off }
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				return vertex;
			}
		
			fixed4 frag() : SV_Target
			{
				return fixed4(1,1,1,0);
			}
			ENDCG
		}
	} 
}
