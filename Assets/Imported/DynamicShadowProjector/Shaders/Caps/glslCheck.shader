Shader "Hidden/DynamicShadowProjector/Caps/GLSL" {
	SubShader {
		Pass {
			GLSLPROGRAM
#ifdef VERTEX
			void main ()
			{
				gl_Position = gl_Vertex;
			}
#endif
#ifdef FRAGMENT
			void main ()
			{
				gl_FragColor = vec4(0,0,0,0);
			}
#endif
			ENDGLSL
		}
	} 
}
