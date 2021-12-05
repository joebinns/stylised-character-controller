Shader "DynamicShadowProjector/FastShadowReceiver/Dynamic/Mipmapped Shadow For Lightmap Shadowmask" {
	Properties {
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_ShadowMaskSelector ("Shadowmask Channel", Vector) = (1,0,0,0) 
		_Offset ("Offset", Range (-10, 0)) = -1.0
		_OffsetSlope ("Offset Slope Factor", Range (-1, 0)) = -1.0
		[HideInInspector] _DSPMainTex ("Receiver Texture", 2D) = "white" {}
		[HideInInspector] _DSPBaseAlpha ("Receiver Alpha", Vector) = (1,0,0,1)
		[HideInInspector] _DSPCull("__cull", Float) = 2.0 // Back
	}
	Subshader {
		Tags {"Queue"="Transparent-1" "IgnoreProjector"="True"}
		UsePass "DynamicShadowProjector/FastShadowReceiver/Mipmapped Shadow For Lightmap Shadowmask/PASS"
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}
