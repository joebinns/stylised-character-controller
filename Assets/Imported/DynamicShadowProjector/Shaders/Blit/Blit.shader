Shader "DynamicShadowProjector/Blit/Blit" {
	HLSLINCLUDE
	#include "../EnableCbuffer.cginc"
	#include "UnityCG.cginc"
	struct v2f_blit
	{
		float4 pos : SV_POSITION;
		half4  uv0 : TEXCOORD0;
	};
	half _MipLevel;
	half _MipBias;
	sampler2D _MainTex;
	ENDHLSL
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode Off }
		Pass {
			HLSLPROGRAM
			#pragma vertex vert_lod
			#pragma fragment frag_lod
			#pragma target 3.0
			v2f_blit vert_lod(appdata_img v)
			{
				v2f_blit o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0.xy = v.texcoord.xy;
				o.uv0.zw = _MipLevel;
				return o;
			}
			fixed4 frag_lod(v2f_blit i) : SV_Target
			{
				return tex2Dlod(_MainTex, i.uv0);
			}
			ENDHLSL
		}
	} 
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode Off }
		Pass {
			HLSLPROGRAM
			#pragma vertex vert_bias
			#pragma fragment frag_bias
			v2f_blit vert_bias(appdata_img v)
			{
				v2f_blit o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0.xy = v.texcoord.xy;
				o.uv0.zw = _MipBias;
				return o;
			}
			fixed4 frag_bias(v2f_blit i) : SV_Target
			{
				return tex2Dbias(_MainTex, i.uv0);
			}
			ENDHLSL
		}
	} 
}
