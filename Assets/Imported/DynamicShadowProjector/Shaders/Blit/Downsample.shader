Shader "DynamicShadowProjector/Blit/Downsample" {
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
			#pragma fragment frag_downsample_with_blur_lod
			#pragma target 3.0
			fixed4 frag_downsample_with_blur_lod(v2f_blit i) : SV_Target
			{
				fixed4 uv;
				uv.xy = i.uv0 + _Offset0.xy;
				uv.zw = 0;
				fixed4 c0 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset1.xy;
				fixed4 c1 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset2.xy;
				fixed4 c2 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset3.xy;
				fixed4 c3 = tex2Dlod(_MainTex, uv);

				uv.xy = i.uv0 + _Offset0.xw;
				fixed4 color = _Weight.x * c0;
				c0 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset1.xw;
				color += _Weight.y * c1;
				c1 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset2.xw;
				color += _Weight.z * c2;
				c2 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset3.xw;
				color += _Weight.w * c3;
				c3 = tex2Dlod(_MainTex, uv);

				uv.xy = i.uv0 + _Offset0.zy;
				color += _Weight.x * c0;
				c0 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset1.zy;
				color += _Weight.y * c1;
				c1 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset2.zy;
				color += _Weight.z * c2;
				c2 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset3.zy;
				color += _Weight.w * c3;
				c3 = tex2Dlod(_MainTex, uv);

				uv.xy = i.uv0 + _Offset0.zw;
				color += _Weight.x * c0;
				c0 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset1.zw;
				color += _Weight.y * c1;
				c1 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset2.zw;
				color += _Weight.z * c2;
				c2 = tex2Dlod(_MainTex, uv);
				uv.xy = i.uv0 + _Offset3.zw;
				color += _Weight.w * c3;
				c3 = tex2Dlod(_MainTex, uv);

				color += _Weight.x * c0;
				color += _Weight.y * c1;
				color += _Weight.z * c2;
				color += _Weight.w * c3;

				return color;
			}
			ENDCG
		}
	}
	Fallback "Hidden/DynamicShadowProjector/Blit/DownsampleFallback"
}
