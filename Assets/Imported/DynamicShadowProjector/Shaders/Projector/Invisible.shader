Shader "DynamicShadowProjector/Projector/Invisible (For Performance Test)"
{
	SubShader {
		Tags { "CompatibleWithProjectorForLWRP"="True" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			float4 vert(float4 vertex   : POSITION) : SV_POSITION
			{
				return float4(vertex.x,vertex.y,-1,-1);
			}
			fixed4 frag() : SV_Target
			{
				return 0;
			}
			ENDCG
		}
	}
}
