Shader "Hidden/ProjectorForLWRP/StencilPass"
{
	SubShader
	{
		// pass 1
		Pass
		{
			Cull Back
			ZWrite Off
			ColorMask 0
			Stencil {
				Ref [P4LWRP_StencilRef]
				WriteMask [P4LWRP_StencilMask]
				Comp Always
				Pass Replace
				ZFail Keep
			}
			HLSLPROGRAM
			#include "EnableCbuffer.cginc"
			#include "UnityCG.cginc"
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}
			fixed4 frag () : SV_Target
			{
				return 0;
			}
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
		// pass 2
		Pass
		{
			Cull Front
			ZWrite Off
			ColorMask 0
			Stencil {
				Ref [P4LWRP_StencilRef]
				WriteMask [P4LWRP_StencilMask]
				Comp Always
				Pass Zero
				ZFail Keep
			}
			HLSLPROGRAM
			#include "EnableCbuffer.cginc"
			#include "UnityCG.cginc"
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}
			fixed4 frag () : SV_Target
			{
				return 0;
			}
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
		// a pass for a case that the camera goes into the projector frustum.
		Pass
		{
			Cull Front
			ZWrite Off
			ColorMask 0
			Stencil {
				Ref [P4LWRP_StencilRef]
				WriteMask [P4LWRP_StencilMask]
				Comp Always
				Pass Replace
				ZFail Keep
			}
			HLSLPROGRAM
			#include "EnableCbuffer.cginc"
			#include "UnityCG.cginc"
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}
			fixed4 frag () : SV_Target
			{
				return 0;
			}
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
		// clear pass
		Pass
		{
			Cull Back
			ZWrite Off
			ColorMask 0
			Stencil {
				Ref 0
				ReadMask [P4LWRP_StencilMask]
				WriteMask [P4LWRP_StencilMask]
				Comp NotEqual
				Pass Zero
				ZFail Keep
			}
			HLSLPROGRAM
			#include "EnableCbuffer.cginc"
			#include "UnityCG.cginc"
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}
			fixed4 frag () : SV_Target
			{
				return 0;
			}
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
		// clear fullscreen pass
		Pass
		{
			Cull Off
			ZWrite Off
			ColorMask 0
			Stencil {
				Ref 0
				ReadMask [P4LWRP_StencilMask]
				WriteMask [P4LWRP_StencilMask]
				Comp NotEqual
				Pass Zero
				ZFail Zero
			}
			HLSLPROGRAM
			#include "EnableCbuffer.cginc"
			#include "UnityCG.cginc"
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				return vertex;
			}
			fixed4 frag () : SV_Target
			{
				return 0;
			}
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
	}
}
