Shader "DynamicShadowProjector/Projector/Dynamic/Simple Shadow Without Falloff" {
	Properties {
		_ClipScale ("Near Clip Sharpness", Float) = 100
		_Alpha ("Shadow Darkness", Range (0, 1)) = 1.0
		_Offset ("Offset", Range (-10, 0)) = -1.0
		_OffsetSlope ("Offset Slope Factor", Range (-1, 0)) = -1.0
		[HideInInspector] _DSPMainTex ("Receiver Texture", 2D) = "white" {}
		[HideInInspector] _DSPBaseAlpha ("Receiver Alpha", Vector) = (1,0,0,1)
		[HideInInspector] _DSPCull("__cull", Float) = 2.0 // Back
	}
	Subshader {
		Tags {"Queue"="Transparent-1" "DSPHasEnableFarClipFeature"="True"}
		UsePass "DynamicShadowProjector/Projector/Simple Shadow Without Falloff/PASS"
	}
	CustomEditor "DynamicShadowProjector.ProjectorShaderGUI"
}
