using UnityEngine;

namespace DynamicShadowProjector {
	[RequireComponent(typeof(ShadowTextureRenderer))]
	public class MipmappedShadowFallback : MonoBehaviour {
		public Object   m_fallbackShaderOrMaterial;
		public int      m_blurLevel = 1;
		public float    m_blurSize = 2.0f;
		public bool     m_modifyTextureSize = false;
		public ShadowTextureRenderer.TextureMultiSample m_multiSampling = ShadowTextureRenderer.TextureMultiSample.x4;
		public ShadowTextureRenderer.TextureSuperSample m_superSampling = ShadowTextureRenderer.TextureSuperSample.x1;
		public int      m_textureWidth = 64;
		public int      m_textureHeight = 64;
		public Shader   m_tex2DlodCheckShader;
		public Shader   m_glslCheckShader;
		void Awake()
		{
			Projector projector = GetComponent<Projector>();
			if (projector == null || projector.material == null) {
				return;
			}
			bool bSupported = projector.material.shader.isSupported;
			// shader.isSupport returns true even on devices which don't support tex2Dlod,
			// because Unity shader compiler replaces tex2Dlod with tex2Dbias for those devices.
			// That's why additional check is needed here.
			if (bSupported && m_tex2DlodCheckShader != null && m_glslCheckShader != null && m_glslCheckShader.isSupported) {
				bSupported = m_tex2DlodCheckShader.isSupported;
			}
			if (!bSupported) {
				if (Debug.isDebugBuild) {
					Debug.Log("This device does not support tex2Dlod. Use fallback shader instead: " + SystemInfo.graphicsDeviceID);
				}
				ApplyFallback(projector);
			}
		}

		public void ApplyFallback(Projector projector)
		{
			if (m_fallbackShaderOrMaterial is Shader) {
				projector.material.shader = m_fallbackShaderOrMaterial as Shader;
			}
			else if (m_fallbackShaderOrMaterial is Material) {
				projector.material = m_fallbackShaderOrMaterial as Material;
			}
			ShadowTextureRenderer shadowRenderer = projector.GetComponent<ShadowTextureRenderer>();
			shadowRenderer.blurLevel = m_blurLevel;
			shadowRenderer.blurSize = m_blurSize;
			shadowRenderer.mipLevel = 0;
			if (m_modifyTextureSize) {
				shadowRenderer.textureWidth = m_textureWidth;
				shadowRenderer.textureHeight = m_textureHeight;
				shadowRenderer.multiSampling = m_multiSampling;
				shadowRenderer.superSampling = m_superSampling;
			}
		}
	}
}
