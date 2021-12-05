Shader "Hidden/DynamicShadowProjector/Blit/DownsampleFallback" {
	CGINCLUDE
	#include "Downsample.cginc"
	ENDCG

	SubShader {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode Off }
		// pass 0: downsample
		Pass {
			CGPROGRAM
			#pragma vertex vert_downsample
			#pragma fragment frag_downsample
			ENDCG
		}
		// pass 1: downsample with shadow color
		Pass {
			CGPROGRAM
			#pragma vertex vert_downsample
			#pragma fragment frag_downsample_withShadowColor
			ENDCG
		}
		// pass 2: blit
		Pass {
			CGPROGRAM
			#pragma vertex vert_blit
			#pragma fragment frag_blit
			ENDCG
		}
		// pass 3: blit with shadow color
		Pass {
			CGPROGRAM
			#pragma vertex vert_blit
			#pragma fragment frag_blit_withShadowColor
			ENDCG
		}
		// pass 4: downsample with blur
		Pass {
			CGPROGRAM
			#pragma vertex vert_blit
			#pragma fragment frag_downsample_with_blur
			ENDCG
		}
		// pass 5: downsample with blur for mipmap
		Pass {
			CGPROGRAM
			#pragma vertex vert_blit
			#pragma fragment frag_downsample_with_blur
			ENDCG
		}
	}
}
