Shader "DynamicShadowProjector/Blit/Blur" {
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	struct v2f_3tap
	{
		float4 pos : SV_POSITION;
		half2  uv0 : TEXCOORD0;
		half4  uv1 : TEXCOORD1;
	};
	struct v2f_5tap
	{
		float4 pos : SV_POSITION;
		half2  uv0 : TEXCOORD0;
		half4  uv1 : TEXCOORD1;
		half4  uv2 : TEXCOORD2;
	};
	struct v2f_7tap
	{
		float4 pos : SV_POSITION;
		half2  uv0 : TEXCOORD0;
		half4  uv1 : TEXCOORD1;
		half4  uv2 : TEXCOORD2;
		half4  uv3 : TEXCOORD3;
	};
	half4  _OffsetH;
	half4  _OffsetV;
	fixed4 _WeightH;
	fixed4 _WeightV;

	v2f_3tap vert_3tap_H(appdata_img v)
	{
		v2f_3tap o;
		o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
		o.pos.y = -o.pos.y;
#endif
       	o.uv0 = v.texcoord.xy;
       	o.uv1 = v.texcoord.xyxy + _MainTex_TexelSize.x * half4(_OffsetH.x, 0, -_OffsetH.x, 0);
       	return o;
	}
	v2f_3tap vert_3tap_V(appdata_img v)
	{
		v2f_3tap o;
		o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
		o.pos.y = -o.pos.y;
#endif
       	o.uv0 = v.texcoord.xy;
       	o.uv1 = v.texcoord.xyxy + _MainTex_TexelSize.y * half4(0, _OffsetV.x, 0, -_OffsetV.x);
       	return o;
	}
	fixed4 frag_3tap_H(v2f_3tap i) : SV_Target
	{
		fixed4 color = _WeightH.x*tex2D(_MainTex, i.uv0);
		color += _WeightH.y*tex2D(_MainTex, i.uv1.xy);
		color += _WeightH.y*tex2D(_MainTex, i.uv1.zw);
		return color;
	}
	fixed4 frag_3tap_V(v2f_3tap i) : SV_Target
	{
		fixed4 color = _WeightV.x*tex2D(_MainTex, i.uv0);
		color += _WeightV.y*tex2D(_MainTex, i.uv1.xy);
		color += _WeightV.y*tex2D(_MainTex, i.uv1.zw);
		return color;
	}
	v2f_5tap vert_5tap_H(appdata_img v)
	{
		v2f_5tap o;
		o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
		o.pos.y = -o.pos.y;
#endif
       	o.uv0 = v.texcoord.xy;
       	o.uv1 = v.texcoord.xyxy + _MainTex_TexelSize.x * half4(_OffsetH.x, 0, -_OffsetH.x, 0);
       	o.uv2 = v.texcoord.xyxy + _MainTex_TexelSize.x * half4(_OffsetH.y, 0, -_OffsetH.y, 0);
       	return o;
	}
	v2f_5tap vert_5tap_V(appdata_img v)
	{
		v2f_5tap o;
		o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
		o.pos.y = -o.pos.y;
#endif
       	o.uv0 = v.texcoord.xy;
       	o.uv1 = v.texcoord.xyxy + _MainTex_TexelSize.y * half4(0, _OffsetV.x, 0, -_OffsetV.x);
       	o.uv2 = v.texcoord.xyxy + _MainTex_TexelSize.y * half4(0, _OffsetV.y, 0, -_OffsetV.y);
       	return o;
	}
	fixed4 frag_5tap_H(v2f_5tap i) : SV_Target
	{
		fixed4 color = _WeightH.x*tex2D(_MainTex, i.uv0);
		color += _WeightH.y*tex2D(_MainTex, i.uv1.xy);
		color += _WeightH.y*tex2D(_MainTex, i.uv1.zw);
		color += _WeightH.z*tex2D(_MainTex, i.uv2.xy);
		color += _WeightH.z*tex2D(_MainTex, i.uv2.zw);
		return color;
	}
	fixed4 frag_5tap_V(v2f_5tap i) : SV_Target
	{
		fixed4 color = _WeightV.x*tex2D(_MainTex, i.uv0);
		color += _WeightV.y*tex2D(_MainTex, i.uv1.xy);
		color += _WeightV.y*tex2D(_MainTex, i.uv1.zw);
		color += _WeightV.z*tex2D(_MainTex, i.uv2.xy);
		color += _WeightV.z*tex2D(_MainTex, i.uv2.zw);
		return color;
	}
	v2f_7tap vert_7tap_H(appdata_img v)
	{
		v2f_7tap o;
		o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
		o.pos.y = -o.pos.y;
#endif
       	o.uv0 = v.texcoord.xy;
       	o.uv1 = v.texcoord.xyxy + _MainTex_TexelSize.x * half4(_OffsetH.x, 0, -_OffsetH.x, 0);
       	o.uv2 = v.texcoord.xyxy + _MainTex_TexelSize.x * half4(_OffsetH.y, 0, -_OffsetH.y, 0);
       	o.uv3 = v.texcoord.xyxy + _MainTex_TexelSize.x * half4(_OffsetH.z, 0, -_OffsetH.z, 0);
       	return o;
	}
	v2f_7tap vert_7tap_V(appdata_img v)
	{
		v2f_7tap o;
		o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
		o.pos.y = -o.pos.y;
#endif
       	o.uv0 = v.texcoord.xy;
       	o.uv1 = v.texcoord.xyxy + _MainTex_TexelSize.y * half4(0, _OffsetV.x, 0, -_OffsetV.x);
       	o.uv2 = v.texcoord.xyxy + _MainTex_TexelSize.y * half4(0, _OffsetV.y, 0, -_OffsetV.y);
       	o.uv3 = v.texcoord.xyxy + _MainTex_TexelSize.y * half4(0, _OffsetV.z, 0, -_OffsetV.z);
       	return o;
	}
	fixed4 frag_7tap_H(v2f_7tap i) : SV_Target
	{
		fixed4 color = _WeightH.x*tex2D(_MainTex, i.uv0);
		color += _WeightH.y*tex2D(_MainTex, i.uv1.xy);
		color += _WeightH.y*tex2D(_MainTex, i.uv1.zw);
		color += _WeightH.z*tex2D(_MainTex, i.uv2.xy);
		color += _WeightH.z*tex2D(_MainTex, i.uv2.zw);
		color += _WeightH.w*tex2D(_MainTex, i.uv3.xy);
		color += _WeightH.w*tex2D(_MainTex, i.uv3.zw);
		return color;
	}
	fixed4 frag_7tap_V(v2f_7tap i) : SV_Target
	{
		fixed4 color = _WeightV.x*tex2D(_MainTex, i.uv0);
		color += _WeightV.y*tex2D(_MainTex, i.uv1.xy);
		color += _WeightV.y*tex2D(_MainTex, i.uv1.zw);
		color += _WeightV.z*tex2D(_MainTex, i.uv2.xy);
		color += _WeightV.z*tex2D(_MainTex, i.uv2.zw);
		color += _WeightV.w*tex2D(_MainTex, i.uv3.xy);
		color += _WeightV.w*tex2D(_MainTex, i.uv3.zw);
		return color;
	}
	
	ENDCG

	SubShader {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode Off }

		Pass {		
			CGPROGRAM
			#pragma vertex vert_3tap_H
			#pragma fragment frag_3tap_H
			ENDCG
		}
		Pass {		
			CGPROGRAM
			#pragma vertex vert_3tap_V
			#pragma fragment frag_3tap_V
			ENDCG
		}
		Pass {		
			CGPROGRAM
			#pragma vertex vert_5tap_H
			#pragma fragment frag_5tap_H
			ENDCG
		}
		Pass {		
			CGPROGRAM
			#pragma vertex vert_5tap_V
			#pragma fragment frag_5tap_V
			ENDCG
		}
		Pass {		
			CGPROGRAM
			#pragma vertex vert_7tap_H
			#pragma fragment frag_7tap_H
			ENDCG
		}
		Pass {		
			CGPROGRAM
			#pragma vertex vert_7tap_V
			#pragma fragment frag_7tap_V
			ENDCG
		}
	}
}
