
//
// UnityShaderVariablesForSRP.cginc
//
// Dynamic Shadow Projector
//
// Copyright 2020 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

// UNITY_SHADER_NO_UPGRADE

#if !defined(DYNAMICSHADOWPROJECTOR_UNITYSHADERVARIABLESFORSRP_CGINC_DEFINED)
#define DYNAMICSHADOWPROJECTOR_UNITYSHADERVARIABLESFORSRP_CGINC_DEFINED

#if 201820 <= UNITY_VERSION // SRP Batcher is not available before Unity 2018.2

#include "../EnableCbuffer.cginc"

// rename variables before include UnityShaderVariables.cginc
#define unity_ObjectToWorld __unity_ObjectToWorld
#define unity_WorldToObject __unity_WorldToObject
#define unity_LODFade __unity_LODFade
#define unity_WorldTransformParams __unity_WorldTransformParams
#define unity_RenderingLayer __unity_RenderingLayer

#define unity_MatrixMVP __unity_MatrixMVP
#define unity_MatrixMV __unity_MatrixMV
#define unity_MatrixTMV __unity_MatrixTMV
#define unity_MatrixITMV __unity_MatrixITMV

#define unity_LightmapST __unity_LightmapST
#define unity_DynamicLightmapST __unity_DynamicLightmapST
#define unity_SHAr __unity_SHAr
#define unity_SHAg __unity_SHAg
#define unity_SHAb __unity_SHAb
#define unity_SHBr __unity_SHBr
#define unity_SHBg __unity_SHBg
#define unity_SHBb __unity_SHBb
#define unity_SHC __unity_SHC

#define UnityPerDraw __UnityPerDraw

#include "UnityShaderVariables.cginc"

#undef unity_ObjectToWorld
#undef unity_WorldToObject
#undef unity_LODFade
#undef unity_WorldTransformParams
#undef unity_RenderingLayer

#undef unity_MatrixMVP
#undef unity_MatrixMV
#undef unity_MatrixTMV
#undef unity_MatrixITMV

#undef unity_LightmapST
#undef unity_DynamicLightmapST
#undef unity_SHAr
#undef unity_SHAg
#undef unity_SHAb
#undef unity_SHBr
#undef unity_SHBg
#undef unity_SHBb
#undef unity_SHC

#undef UnityPerDraw
#undef UNITY_MATRIX_M

// re-declare variables in UnityPerDraw cbuffer
CBUFFER_START(UnityPerDraw)
    float4x4 unity_ObjectToWorld;
    float4x4 unity_WorldToObject;
    float4 unity_LODFade; // x is the fade value ranging within [0,1]. y is x quantized into 16 levels
    float4 unity_WorldTransformParams; // w is usually 1.0, or -1.0 for odd-negative scale transforms
    float4 unity_RenderingLayer;
    float4 unity_LightmapST;
    float4 unity_DynamicLightmapST;
    half4 unity_SHAr;
    half4 unity_SHAg;
    half4 unity_SHAb;
    half4 unity_SHBr;
    half4 unity_SHBg;
    half4 unity_SHBb;
    half4 unity_SHC;
CBUFFER_END

static float4x4 unity_MatrixMVP = mul(unity_MatrixVP, unity_ObjectToWorld);
static float4x4 unity_MatrixMV = mul(unity_MatrixV, unity_ObjectToWorld);
static float4x4 unity_MatrixTMV = transpose(unity_MatrixMV);
static float4x4 unity_MatrixITMV = transpose(mul(unity_WorldToObject, unity_MatrixInvV));
#define UNITY_MATRIX_M unity_ObjectToWorld

#endif

#endif // !defined(DYNAMICSHADOWPROJECTOR_UNITYSHADERVARIABLESFORSRP_CGINC_DEFINED)
