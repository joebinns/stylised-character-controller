//
// EnableCbuffer.cginc
//
// Projector For LWRP
//
// Copyright 2020 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

#if !defined(P4LWRP_ENABLECBUFFER_CGINC_DEFINED)
#define P4LWRP_ENABLECBUFFER_CGINC_DEFINED

// Min version required for SRP Batcher
#if defined(SHADER_API_PSSL) || defined(SHADER_API_D3D11)
#define SRP_BATCHER_COMPATIBLE_VERSION 201820
#elif defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_SWITCH)
#define SRP_BATCHER_COMPATIBLE_VERSION 201830
#elif defined(SHADER_API_GLES)
// not supported
#define SRP_BATCHER_COMPATIBLE_VERSION 999999
#else
#define SRP_BATCHER_COMPATIBLE_VERSION 201920
#endif

// enable CBUFFER macros for SRP Batcher
#if 201930 <= UNITY_VERSION

#define UNITY_ENABLE_CBUFFER

#elif SRP_BATCHER_COMPATIBLE_VERSION <= UNITY_VERSION

#include "HLSLSupport.cginc"
#if !(defined(SHADER_API_D3D11) || defined(SHADER_API_PSSL))
#undef CBUFFER_START
#undef CBUFFER_END
#define CBUFFER_START(name) cbuffer name {
#define CBUFFER_END };
#endif

#endif

#endif // !defined(P4LWRP_ENABLECBUFFER_CGINC_DEFINED)
