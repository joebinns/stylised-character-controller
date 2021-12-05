Shader "Hidden/DynamicShadowProjector/Caps/tex2Dlod" {
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
#if defined(SHADER_API_OPENGL)
#extension GL_ARB_shader_texture_lod : require
#elif defined(SHADER_API_GLES)
#extension GL_EXT_shader_texture_lod : require
#define texture2DLod texture2DLodEXT
#elif defined(SHADER_API_GLES3)
#define texture2DLod textureLod
#if !defined(gl_FragColor)
#define gl_FragColor SV_Target0
layout(location = 0) out mediump out vec4 SV_Target0;
#endif
#elif defined(SHADER_API_GLCORE)
#define texture2DLod textureLod
#if !defined(gl_FragColor)
#define gl_FragColor SV_Target0
out vec4 SV_Target0;
#endif
#endif
			uniform sampler2D _ShadowTex;
			uniform vec2 _coord;
			uniform float _lod;
			void main ()
			{
					gl_FragColor = texture2DLod(_ShadowTex, _coord, _lod);
			}
#endif
			ENDGLSL
		}
	}
}
