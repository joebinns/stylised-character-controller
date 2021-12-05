#if !defined(DSP_MIPMAPPEDSHADPW_CGINC_INCLUDED)
#define DSP_MIPMAPPEDSHADPW_CGINC_INCLUDED

#include "DSPProjector.cginc"

struct DSP_V2F_PROJECTOR_LIGHTMAPUV {
#if defined(DSP_USE_MAIN_TEXTURE)
	half2  uv_alpha         : TEXCOORD0;
	half4  lightmapuv_alpha : TEXCOORD1;
	float4 uvShadow         : TEXCOORD2;
	UNITY_FOG_COORDS(3)
#else
	half4  lightmapuv_alpha : TEXCOORD0;
	float4 uvShadow         : TEXCOORD1;
	UNITY_FOG_COORDS(2)
#endif
	float4 pos : SV_POSITION;
	UNITY_VERTEX_OUTPUT_STEREO
};


#if UNITY_VERSION < 201800 && !defined(SHADOWS_SHADOWMASK)
UNITY_DECLARE_TEX2D(unity_ShadowMask);
#endif

DSP_V2F_PROJECTOR DSPProjectorVertMipmap(DSP_PROJECTOR_VERTEXATTRIBUTES v)
{
	DSP_V2F_PROJECTOR o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	fsrTransformVertex(v.vertex, o.pos, o.uvShadow);
	float z = o.uvShadow.z;
	o.uvShadow.z = _DSPMipLevel * z;
	o.DSP_ALPHA_NEARCLIP_COMPONENT = _ClipScale * z;
	o.DSP_ALPHA_DIFFUSE_COMPONENT = DSPCalculateDiffuseShadowAlpha(v.vertex, v.normal);
	DSP_OUTPUT_MAINTEX_UV(o, v);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR_LIGHTMAPUV DSPProjectorVertMipmapForLightmap(DSP_PROJECTOR_VERTEXATTRIBUTES v, float2 lightmapUV : TEXCOORD1)
{
	DSP_V2F_PROJECTOR_LIGHTMAPUV o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	fsrTransformVertex(v.vertex, o.pos, o.uvShadow);
	float z = o.uvShadow.z;
	o.uvShadow.z = _DSPMipLevel * z;
	o.lightmapuv_alpha.xy = lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
	o.lightmapuv_alpha.z = _ClipScale * z;
	o.lightmapuv_alpha.w = -dot(v.normal, fsrProjectorDir());
	DSP_OUTPUT_MAINTEX_UV(o, v);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

fixed4 DSPGetMipmappedShadowTex(float4 uvShadow)
{
	float3 uv;
	uv.xy = saturate(uvShadow.xy/uvShadow.w);
	uv.z = uvShadow.z;
	return tex2Dlod(_ShadowTex, uv.xyzz);
}

fixed3 DSPCalculateSubtractiveShadow(fixed3 shadowColor, half2 lightmapUV, half ndotl, fixed falloff, fixed texAlpha)
{
	half4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, lightmapUV);
	FSR_LIGHTCOLOR3 bakedColor = DecodeLightmap(bakedColorTex);
	FSR_LIGHTCOLOR3 mainLight = FSR_MAINLIGHTCOLOR * saturate(ndotl);
	FSR_LIGHTCOLOR3 ambientColor = max(bakedColor - mainLight, _AmbientColor.rgb);
	shadowColor = lerp(fixed3(1,1,1), shadowColor, saturate(_Alpha * falloff * texAlpha));
	return saturate((shadowColor * (bakedColor - ambientColor) + ambientColor) / bakedColor);
}

fixed3 DSPCalculateShadowmaskShadow(fixed3 shadowColor, half2 lightmapUV, half ndotl, fixed falloff, fixed texAlpha)
{
	fixed4 shadowmask = UNITY_SAMPLE_TEX2D(unity_ShadowMask, lightmapUV);
	shadowmask = saturate(dot(shadowmask, _ShadowMaskSelector));
	half4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, lightmapUV);
	FSR_LIGHTCOLOR3 bakedColor = DecodeLightmap(bakedColorTex);
	FSR_LIGHTCOLOR3 mainLight = FSR_MAINLIGHTCOLOR * saturate(shadowmask*ndotl);
	fixed3 shadow = saturate((_Alpha * falloff * mainLight * texAlpha)/(bakedColor + mainLight));
	return lerp(fixed3(1,1,1), shadowColor, shadow);
}

fixed4 DSPProjectorFragMipmap(DSP_V2F_PROJECTOR i) : SV_Target
{
	fixed4 shadow = DSPGetMipmappedShadowTex(i.uvShadow);
	return DSPCalculateFinalShadowColor(shadow, i);
}

fixed4 DSPProjectorFragMipmapForLightmapSubtractive(DSP_V2F_PROJECTOR_LIGHTMAPUV i) : SV_Target
{
	fixed4 shadow = DSPGetMipmappedShadowTex(i.uvShadow);
	shadow.rgb = DSPCalculateSubtractiveShadow(shadow.rgb, i.lightmapuv_alpha.xy, i.lightmapuv_alpha.w, saturate(i.lightmapuv_alpha.z), DSP_GET_MAINTEX_ALPHA(i));

	UNITY_APPLY_FOG_COLOR(i.fogCoord, shadow, fixed4(1,1,1,1));
	return shadow;
}

fixed4 DSPProjectorFragMipmapForLightmapShadowmask(DSP_V2F_PROJECTOR_LIGHTMAPUV i) : SV_Target
{
	fixed4 shadow = DSPGetMipmappedShadowTex(i.uvShadow);
	shadow.rgb = DSPCalculateShadowmaskShadow(shadow.rgb, i.lightmapuv_alpha.xy, i.lightmapuv_alpha.w, saturate(i.lightmapuv_alpha.z), DSP_GET_MAINTEX_ALPHA(i));

	UNITY_APPLY_FOG_COLOR(i.fogCoord, shadow, fixed4(1,1,1,1));
	return shadow;
}

#endif // !defined(DSP_MIPMAPPEDSHADPW_CGINC_INCLUDED)
