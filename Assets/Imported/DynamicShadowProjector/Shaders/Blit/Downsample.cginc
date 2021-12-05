#if !defined(DYNAMICSHADOWPROJECTOR_DOWNSAMPLE_CGINC)
#define DYNAMICSHADOWPROJECTOR_DOWNSAMPLE_CGINC

#include "UnityCG.cginc"

sampler2D _MainTex;
half4 _MainTex_TexelSize;
fixed4 _Color;

struct v2f_blit
{
	float4 pos : SV_POSITION;
	half2  uv0 : TEXCOORD0;
};
struct v2f_downsample
{
	float4 pos : SV_POSITION;
	half2  uv0 : TEXCOORD0;
	half2  uv1 : TEXCOORD1;
	half2  uv2 : TEXCOORD2;
	half2  uv3 : TEXCOORD3;
};

v2f_blit vert_blit(appdata_img v)
{
	v2f_blit o;
	o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
	o.pos.y = -o.pos.y;
#endif
   	o.uv0 = v.texcoord.xy;
	return o;
}

fixed4 frag_blit(v2f_blit i) : SV_Target
{
	return tex2D(_MainTex, i.uv0);
}

fixed4 frag_blit_withShadowColor(v2f_blit i) : SV_Target
{
	return lerp(fixed4(1,1,1,0), _Color, tex2D(_MainTex, i.uv0).a);
}

v2f_downsample vert_downsample(appdata_img v)
{
	v2f_downsample o;
	o.pos = v.vertex;
#if UNITY_UV_STARTS_AT_TOP
	o.pos.y = -o.pos.y;
#endif
   	o.uv0 = v.texcoord.xy + _MainTex_TexelSize.xy;
	o.uv1 = v.texcoord.xy - _MainTex_TexelSize.xy;
	o.uv2 = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1,-1);
	o.uv3 = v.texcoord.xy + _MainTex_TexelSize.xy * half2(-1,1);
	return o;
}

fixed4 frag_downsample(v2f_downsample i) : SV_Target
{
	fixed4 color = 0.25*tex2D(_MainTex, i.uv0);
	color += 0.25*tex2D(_MainTex, i.uv1);
	color += 0.25*tex2D(_MainTex, i.uv2);
	color += 0.25*tex2D(_MainTex, i.uv3);
	return color;
}

fixed4 frag_downsample_withShadowColor(v2f_downsample i) : SV_Target
{
	fixed a = 0.25*tex2D(_MainTex, i.uv0).a;
	a += 0.25*tex2D(_MainTex, i.uv1).a;
	a += 0.25*tex2D(_MainTex, i.uv2).a;
	a += 0.25*tex2D(_MainTex, i.uv3).a;
	return lerp(fixed4(1,1,1,0), _Color, a);
}

#define DSP_OFFSET_TYPE4 fixed4
#define DSP_OFFSET_TYPE2 fixed2
DSP_OFFSET_TYPE4 _Offset0;
DSP_OFFSET_TYPE4 _Offset1;
DSP_OFFSET_TYPE4 _Offset2;
DSP_OFFSET_TYPE4 _Offset3;
fixed4 _Weight;

fixed4 frag_downsample_with_blur(v2f_blit i) : SV_Target
{
	DSP_OFFSET_TYPE2 uv = i.uv0 + _Offset0.xy;
	fixed4 c0 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset1.xy;
	fixed4 c1 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset2.xy;
	fixed4 c2 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset3.xy;
	fixed4 c3 = tex2D(_MainTex, uv);

	uv = i.uv0 + _Offset0.xw;
	fixed4 color = _Weight.x * c0;
	c0 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset1.xw;
	color += _Weight.y * c1;
	c1 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset2.xw;
	color += _Weight.z * c2;
	c2 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset3.xw;
	color += _Weight.w * c3;
	c3 = tex2D(_MainTex, uv);

	uv = i.uv0 + _Offset0.zy;
	color += _Weight.x * c0;
	c0 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset1.zy;
	color += _Weight.y * c1;
	c1 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset2.zy;
	color += _Weight.z * c2;
	c2 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset3.zy;
	color += _Weight.w * c3;
	c3 = tex2D(_MainTex, uv);

	uv = i.uv0 + _Offset0.zw;
	color += _Weight.x * c0;
	c0 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset1.zw;
	color += _Weight.y * c1;
	c1 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset2.zw;
	color += _Weight.z * c2;
	c2 = tex2D(_MainTex, uv);
	uv = i.uv0 + _Offset3.zw;
	color += _Weight.w * c3;
	c3 = tex2D(_MainTex, uv);

	color += _Weight.x * c0;
	color += _Weight.y * c1;
	color += _Weight.z * c2;
	color += _Weight.w * c3;

	return color;
}
#endif // !defined(DYNAMICSHADOWPROJECTOR_DOWNSAMPLE_CGINC)
