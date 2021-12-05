#if !defined(DSP_PROJECTOR_CGINC_INCLUDED)
#define DSP_PROJECTOR_CGINC_INCLUDED

#include "../EnableCbuffer.cginc"
#include "UnityCG.cginc"

#if !defined(DSP_USE_MAIN_TEXTURE)
	#if defined(DSP_ALPHACUTOFF) || defined(DSP_TRANSPARENT) || defined(DSP_ALPHACUTOFF_AND_TRANSPARENT)
		#define DSP_USE_MAIN_TEXTURE
	#endif
#endif

#ifdef UNITY_HDR_ON
#define FSR_LIGHTCOLOR4 half4
#define FSR_LIGHTCOLOR3 half3
#else
#define FSR_LIGHTCOLOR4 fixed4
#define FSR_LIGHTCOLOR3 fixed3
#endif

#if defined(FSR_PROJECTOR_FOR_LWRP)
FSR_LIGHTCOLOR4 _MainLightColor;
#define FSR_MAINLIGHTCOLOR	_MainLightColor
#else
fixed4 _LightColor0;
#define FSR_MAINLIGHTCOLOR	_LightColor0
#endif

struct DSP_PROJECTOR_VERTEXATTRIBUTES
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
#if defined(DSP_USE_MAIN_TEXTURE)
	float2 uv     : TEXCOORD0;
#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct DSP_V2F_PROJECTOR {
#if defined(DSP_USE_MAIN_TEXTURE)
	half4  uv_alpha : TEXCOORD0;
	float4 uvShadow : TEXCOORD1;
#else
	float4 uvShadow : TEXCOORD0;
	half2  alpha     : COLOR;  // fixed precision is ok for most GPU, but I saw a problem on Tegra 3.
#endif
	UNITY_FOG_COORDS(2)
	float4 pos : SV_POSITION;
	UNITY_VERTEX_OUTPUT_STEREO
};

#if defined(FSR_RECEIVER)

CBUFFER_START(FSR_ProjectorTransform)
float4x4 _FSRProjector;
float4 _FSRProjectDir;
CBUFFER_END

void fsrTransformVertex(float4 v, out float4 clipPos, out float4 shadowUV)
{
	clipPos = UnityObjectToClipPos(v);
	shadowUV = mul(_FSRProjector, v);
}
float3 fsrProjectorDir()
{
	return _FSRProjectDir.xyz;
}
#elif defined(FSR_PROJECTOR_FOR_LWRP)

CBUFFER_START(FSR_ProjectorTransform)
uniform float4x4 _FSRWorldToProjector;
uniform float4 _FSRWorldProjectDir;
CBUFFER_END

void fsrTransformVertex(float4 v, out float4 clipPos, out float4 shadowUV)
{
	float4 worldPos;
	worldPos.xyz = mul(unity_ObjectToWorld, v).xyz;
	worldPos.w = 1.0f;
	shadowUV = mul(_FSRWorldToProjector, worldPos);
#if defined(STEREO_CUBEMAP_RENDER_ON)
    worldPos.xyz += ODSOffset(worldPos.xyz, unity_HalfStereoSeparation.x);
#endif
	clipPos = mul(UNITY_MATRIX_VP, worldPos);
}
float3 fsrProjectorDir()
{
	return UnityWorldToObjectDir(_FSRWorldProjectDir.xyz);
}
#else

CBUFFER_START(FSR_ProjectorTransform)
float4x4 unity_Projector;
float4x4 unity_ProjectorClip;
CBUFFER_END

void fsrTransformVertex(float4 v, out float4 clipPos, out float4 shadowUV)
{
	clipPos = UnityObjectToClipPos(v);
	shadowUV = mul (unity_Projector, v);
	shadowUV.z = mul (unity_ProjectorClip, v).x;
}
float3 fsrProjectorDir()
{
	return normalize(float3(unity_Projector[2][0],unity_Projector[2][1], unity_Projector[2][2]));
}
#endif

// define DSP_USE_XXXX macros before include DSPProjector.cginc to put shader variables into UnityPerMaterial cbuffer
CBUFFER_START(UnityPerMaterial)
uniform float _ClipScale;
uniform fixed _Alpha;
#if defined(DSP_USE_AMBIENT)
uniform fixed _Ambient;
#endif
#if defined(DSP_USE_AMBIENTCOLOR)
uniform FSR_LIGHTCOLOR4 _AmbientColor;
#endif
#if defined(DSP_USE_SHADOWMASK)
uniform fixed4 _ShadowMaskSelector;
#endif
#if defined(DSP_USE_MIPLEVEL)
uniform half _DSPMipLevel;
#endif
#if defined(DSP_USE_MAIN_TEXTURE)
uniform float4 _DSPMainTex_ST;
uniform fixed4 _DSPBaseAlpha;
#endif
CBUFFER_END

#if !defined(DSP_USE_AMBIENT)
uniform fixed _Ambient;
#endif
#if !defined(DSP_USE_AMBIENTCOLOR)
uniform FSR_LIGHTCOLOR4 _AmbientColor;
#endif
#if !defined(DSP_USE_SHADOWMASK)
uniform fixed4 _ShadowMaskSelector;
#endif
#if !defined(DSP_USE_MIPLEVEL)
uniform half _DSPMipLevel;
#endif

sampler2D _ShadowTex;
sampler2D _LightTex;
#if defined(DSP_USE_MAIN_TEXTURE)
sampler2D _DSPMainTex;
#endif

#if defined(DSP_USE_MAIN_TEXTURE)
#define DSP_ALPHA_NEARCLIP_COMPONENT uv_alpha.z
#define DSP_ALPHA_DIFFUSE_COMPONENT  uv_alpha.w
#define DSP_OUTPUT_MAINTEX_UV(o, v)	(o.uv_alpha.xy = (v).uv.xy * _DSPMainTex_ST.xy + _DSPMainTex_ST.zw)
#else
#define DSP_ALPHA_NEARCLIP_COMPONENT alpha.x
#define DSP_ALPHA_DIFFUSE_COMPONENT  alpha.y
#define DSP_OUTPUT_MAINTEX_UV(o, v)
#endif

half DSPCalculateDiffuseLightAlpha(float4 vertex, float3 normal)
{
	float diffuse = -dot(normal, fsrProjectorDir());
	return diffuse;
}

half DSPCalculateDiffuseShadowAlpha(float4 vertex, float3 normal)
{
	float diffuse = -dot(normal, fsrProjectorDir());
	// this calculation is not linear. it is better to do in fragment shader. but in most case, it won't be a problem.
	return _Alpha * diffuse / (_Ambient + saturate(diffuse));
}

DSP_V2F_PROJECTOR DSPProjectorVertLightNoFalloff(DSP_PROJECTOR_VERTEXATTRIBUTES v)
{
	DSP_V2F_PROJECTOR o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	fsrTransformVertex(v.vertex, o.pos, o.uvShadow);
	o.DSP_ALPHA_NEARCLIP_COMPONENT = _ClipScale * o.uvShadow.z;
#if defined(DSP_NO_DIFFUSE_EFFECT)
	o.DSP_ALPHA_DIFFUSE_COMPONENT = 1;
#else
	o.DSP_ALPHA_DIFFUSE_COMPONENT = DSPCalculateDiffuseLightAlpha(v.vertex, v.normal);
#endif
	DSP_OUTPUT_MAINTEX_UV(o, v);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR DSPProjectorVertNoFalloff(DSP_PROJECTOR_VERTEXATTRIBUTES v)
{
	DSP_V2F_PROJECTOR o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	fsrTransformVertex(v.vertex, o.pos, o.uvShadow);
	o.DSP_ALPHA_NEARCLIP_COMPONENT = _ClipScale * o.uvShadow.z;
#if defined(DSP_NO_DIFFUSE_EFFECT)
	o.DSP_ALPHA_DIFFUSE_COMPONENT = _Alpha;
#else
	o.DSP_ALPHA_DIFFUSE_COMPONENT = DSPCalculateDiffuseShadowAlpha(v.vertex, v.normal);
#endif
	DSP_OUTPUT_MAINTEX_UV(o, v);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR DSPProjectorVertLightLinearFalloff(DSP_PROJECTOR_VERTEXATTRIBUTES v)
{
	DSP_V2F_PROJECTOR o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	fsrTransformVertex(v.vertex, o.pos, o.uvShadow);
	float z = o.uvShadow.z;
	o.DSP_ALPHA_NEARCLIP_COMPONENT = _ClipScale * z;
	o.DSP_ALPHA_DIFFUSE_COMPONENT = (1.0f - z); // falloff
#if !defined(DSP_NO_DIFFUSE_EFFECT)
	o.DSP_ALPHA_DIFFUSE_COMPONENT *= DSPCalculateDiffuseLightAlpha(v.vertex, v.normal);
#endif
	DSP_OUTPUT_MAINTEX_UV(o, v);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

DSP_V2F_PROJECTOR DSPProjectorVertLinearFalloff(DSP_PROJECTOR_VERTEXATTRIBUTES v)
{
	DSP_V2F_PROJECTOR o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	fsrTransformVertex(v.vertex, o.pos, o.uvShadow);
	float z = o.uvShadow.z;
	o.DSP_ALPHA_NEARCLIP_COMPONENT = _ClipScale * z;
#if defined(DSP_NO_DIFFUSE_EFFECT)
	o.DSP_ALPHA_DIFFUSE_COMPONENT = _Alpha;
#else
	o.DSP_ALPHA_DIFFUSE_COMPONENT = DSPCalculateDiffuseShadowAlpha(v.vertex, v.normal);
#endif
	o.DSP_ALPHA_DIFFUSE_COMPONENT *= (1.0f - z); // falloff
	DSP_OUTPUT_MAINTEX_UV(o, v);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

#if defined(DSP_USE_MAIN_TEXTURE)
fixed _DSPGetMainTexAlpha(half2 uv)
{
	fixed alpha = tex2D(_DSPMainTex, uv).a * _DSPBaseAlpha.a;
#if defined(DSP_ALPHACUTOFF_AND_TRANSPARENT)
	return step(_DSPBaseAlpha.z, alpha) * _DSPBaseAlpha.x + alpha * _DSPBaseAlpha.y;
#elif defined(DSP_ALPHACUTOFF)
	return step(_DSPBaseAlpha.z, alpha);
#else
	return alpha;
#endif
}
#define DSP_GET_MAINTEX_ALPHA(i) _DSPGetMainTexAlpha(i.uv_alpha.xy)
#else
#define DSP_GET_MAINTEX_ALPHA(i) 1
#endif

fixed DSPGetShadowAlpha(DSP_V2F_PROJECTOR i)
{
	fixed a = saturate(saturate(i.DSP_ALPHA_NEARCLIP_COMPONENT)*i.DSP_ALPHA_DIFFUSE_COMPONENT);
#if defined(DSP_ENABLE_FARCLIP)
	a *= step(i.uvShadow.z, 1);
#endif
	return a;
}

fixed4 DSPCalculateFinalLightColor(fixed4 texColor, DSP_V2F_PROJECTOR i)
{
	fixed alpha = DSP_GET_MAINTEX_ALPHA(i) * saturate(saturate(i.DSP_ALPHA_NEARCLIP_COMPONENT)*i.DSP_ALPHA_DIFFUSE_COMPONENT);
	texColor.rgb *= _Alpha * alpha;
	UNITY_APPLY_FOG_COLOR(i.fogCoord, texColor, fixed4(0,0,0,0));
	return texColor;
}

fixed4 DSPCalculateFinalShadowColor(fixed4 texColor, DSP_V2F_PROJECTOR i)
{
	fixed alpha = DSP_GET_MAINTEX_ALPHA(i) * DSPGetShadowAlpha(i);
	texColor.rgb = lerp(fixed3(1,1,1), texColor.rgb, alpha);
	UNITY_APPLY_FOG_COLOR(i.fogCoord, texColor, fixed4(1,1,1,1));
	return texColor;
}

fixed4 DSPProjectorFrag(DSP_V2F_PROJECTOR i) : SV_Target
{
	fixed4 shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
	return DSPCalculateFinalShadowColor(shadow, i);
}

fixed4 DSPProjectorFragLight(DSP_V2F_PROJECTOR i) : SV_Target
{
	fixed4 light = tex2Dproj(_LightTex, UNITY_PROJ_COORD(i.uvShadow));
	return DSPCalculateFinalLightColor(light, i);
}

fixed4 DSPProjectorFragLightWithShadow(DSP_V2F_PROJECTOR i) : SV_Target
{
	fixed4 light = tex2Dproj(_LightTex, UNITY_PROJ_COORD(i.uvShadow));
	fixed3 shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
	light.rgb *= shadow.rgb;
	return DSPCalculateFinalLightColor(light, i);
}

#endif // !defined(DSP_PROJECTOR_CGINC_INCLUDED)
