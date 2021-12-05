//
// PropertyBlockForTransparentReceiver.cs
//
// Dynamic Shadow Projector
//
// Copyright 2021 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;

namespace DynamicShadowProjector
{
	[ExecuteAlways]
	public class PropertyBlockForTransparentReceiver : MonoBehaviour
	{
		[System.Flags]
		public enum Options
		{
			CopyMainTexFromMaterial = 1 << 0,
			CopyAlphaCutoffFromMaterial = 1 << 1,
			CopyBaseColorFromMaterial = 1 << 2,
			EnableAlphaCutoffByKeyword = 1 << 3,
			EnableAlphaCutoff = 1 << 4,
			CopyCullModeFromMaterial = 1 << 5,
			CullFront = 1 << 6,
			CullBack = 1 << 7
		}
		[SerializeField]
		private Options m_options = Options.CopyMainTexFromMaterial | Options.CopyAlphaCutoffFromMaterial | Options.CopyBaseColorFromMaterial | Options.EnableAlphaCutoffByKeyword | Options.CullBack;
		public Texture mainTexture = null;
		public Vector4 mainTextureST = new Vector4(1, 1, 0, 0);
		public float baseAlpha = 1.0f;
		public float alphaCutoff = 0.5f;
		public string mainTextureName;
		public string baseColorName;
		public string alphaCutoffName;
		public string alphaCutoffKeyword = "_ALPHATEST_ON";
		public string cullModeName;
		private string m_mainTextureSTName;
		private bool GetOption(Options option)
		{
			return (m_options & option) == option;
		}
		private void SetOption(Options option, bool value)
		{
			if (value)
			{
				m_options |= option;
			}
			else
			{
				m_options &= ~option;
			}
		}
		public bool copyMainTexFromMaterial
		{
			get
			{
				return GetOption(Options.CopyMainTexFromMaterial);
			}
			set
			{
				SetOption(Options.CopyMainTexFromMaterial, value);
			}
		}
		public bool copyBaseColorFromMaterial
		{
			get
			{
				return GetOption(Options.CopyBaseColorFromMaterial);
			}
			set
			{
				SetOption(Options.CopyBaseColorFromMaterial, value);
			}
		}
		public bool copyAlphaCutoffFromMaterial
		{
			get
			{
				return GetOption(Options.CopyAlphaCutoffFromMaterial);
			}
			set
			{
				SetOption(Options.CopyAlphaCutoffFromMaterial, value);
			}
		}
		public bool copyCullModeFromMaterial
		{
			get
			{
				return GetOption(Options.CopyCullModeFromMaterial);
			}
			set
			{
				SetOption(Options.CopyCullModeFromMaterial, value);
			}
		}
		public enum CullMode {
			None = 0,
			Front = Options.CullFront,
			Back = Options.CullBack
		}
		public CullMode cullMode
		{
			get
			{
				return (CullMode)(m_options & (Options.CullFront | Options.CullBack));
			}
			set
			{
				m_options &= ~(Options.CullFront | Options.CullBack);
				m_options |= (Options)value;
			}
		}
		public bool enableAlphaCutoffByKeyword
		{
			get
			{
				return GetOption(Options.EnableAlphaCutoffByKeyword);
			}
			set
			{
				SetOption(Options.EnableAlphaCutoffByKeyword, value);
			}
		}
		public bool enableAlphaCutoff
		{
			get
			{
				return (m_options & (Options.EnableAlphaCutoff | Options.EnableAlphaCutoffByKeyword)) == Options.EnableAlphaCutoff;
			}
			set
			{
				SetOption(Options.EnableAlphaCutoff, value);
			}
		}


		private Renderer m_renderer = null;
		private MaterialPropertyBlock m_propertyBlock = null;

		static int s_dspBaseAlphaId;
		static int s_dspCullModeId;
#if UNITY_2017_1_OR_NEWER
		static int s_dspMainTexId;
		static int s_dspMainTexStId;
		static PropertyBlockForTransparentReceiver()
		{
			StaticInitialize();
		}
#else
		static int s_dspMainTexId = -1;
		static int s_dspMainTexStId = -1;
#endif
		static void StaticInitialize()
		{
			s_dspMainTexId = Shader.PropertyToID("_DSPMainTex");
			s_dspMainTexStId = Shader.PropertyToID("_DSPMainTex_ST");
			s_dspBaseAlphaId = Shader.PropertyToID("_DSPBaseAlpha");
			s_dspCullModeId = Shader.PropertyToID("_DSPCull");
		}

		private Texture GetMaterialMainTexture()
		{
			// check property block first
			Texture mainTex = m_propertyBlock.GetTexture(mainTextureName);
			if (mainTex == null)
			{
				mainTex = m_renderer.sharedMaterial.GetTexture(mainTextureName);
			}
			return mainTex;
		}
		private Vector4 GetMaterialMainTextureST()
		{
			// check property block first
			Vector4 mainTexST = m_propertyBlock.GetVector(m_mainTextureSTName);
			if (mainTexST == Vector4.zero)
			{
				mainTexST = m_renderer.sharedMaterial.GetVector(m_mainTextureSTName);
			}
			return mainTexST;
		}
		private Color GetMaterialBaseColor()
		{
			// check property block first
			Color color = m_propertyBlock.GetColor(baseColorName);
			if (color.r == 0 && color.g == 0 && color.b == 0 && color.a == 0)
			{
				color = m_renderer.sharedMaterial.GetColor(baseColorName);
			}
			return color;
		}
		private float GetMaterialAlphaCutoff()
		{
			// check property block first
			float cutoff = m_propertyBlock.GetFloat(alphaCutoffName);
			if (cutoff == 0)
			{
				cutoff = m_renderer.sharedMaterial.GetFloat(alphaCutoffName);
			}
			return cutoff;
		}
		private float GetMaterialCullMode()
		{
			// check property block first
			float cullMode = m_propertyBlock.GetFloat(cullModeName);
			if (cullMode == 0)
			{
				cullMode = m_renderer.sharedMaterial.GetFloat(cullModeName);
			}
			return cullMode;
		}
		public void UpdatePropertyBlock()
		{
			m_renderer.GetPropertyBlock(m_propertyBlock);
			float alpha = 1.0f;
			if (copyMainTexFromMaterial)
			{
				Texture mainTex = GetMaterialMainTexture();
				if (mainTex != null)
				{
					m_propertyBlock.SetTexture(s_dspMainTexId, mainTex);
					m_propertyBlock.SetVector(s_dspMainTexStId, GetMaterialMainTextureST());
				}
				else
				{
					m_propertyBlock.SetTexture(s_dspMainTexId, Texture2D.whiteTexture);
				}
			}
			else
			{
				if (mainTexture != null)
				{
					m_propertyBlock.SetTexture(s_dspMainTexId, mainTexture);
				}
				else
				{
					m_propertyBlock.SetTexture(s_dspMainTexId, Texture2D.whiteTexture);
				}
				m_propertyBlock.SetVector(s_dspMainTexStId, mainTextureST);
			}
			if (copyBaseColorFromMaterial)
			{
				alpha = GetMaterialBaseColor().a;
			}
			else
			{
				alpha = baseAlpha;
			}
			float cutoff;
			if (copyAlphaCutoffFromMaterial)
			{
				cutoff = GetMaterialAlphaCutoff();
			}
			else
			{
				cutoff = alphaCutoff;
			}
			bool isCutoffEnabled;
			if (enableAlphaCutoffByKeyword)
			{
				Material material = m_renderer.sharedMaterial;
				isCutoffEnabled = material.IsKeywordEnabled(alphaCutoffKeyword);
			}
			else
			{
				isCutoffEnabled = enableAlphaCutoff;
			}
			if (isCutoffEnabled)
			{
				bool isTransparent = 3000 <= m_renderer.sharedMaterial.renderQueue;
				if (isTransparent)
				{
					m_propertyBlock.SetVector(s_dspBaseAlphaId, new Vector4(alpha, 0, cutoff, alpha));
				}
				else
				{
					m_propertyBlock.SetVector(s_dspBaseAlphaId, new Vector4(1, 0, cutoff, alpha));
				}
			}
			else
			{
				m_propertyBlock.SetVector(s_dspBaseAlphaId, new Vector4(0, 1, 0, alpha));
			}
			if (copyCullModeFromMaterial)
			{
				m_propertyBlock.SetFloat(s_dspCullModeId, GetMaterialCullMode());
			}
			else
			{
				CullMode cull = cullMode;
				float cullModeValue = cull == CullMode.None ? 0.0f : cull == CullMode.Front ? 1.0f : 2.0f;
				m_propertyBlock.SetFloat(s_dspCullModeId, cullModeValue);
			}
			m_renderer.SetPropertyBlock(m_propertyBlock);
		}

		void Initialize()
		{
			m_renderer = GetComponent<Renderer>();
			m_propertyBlock = new MaterialPropertyBlock();
			m_mainTextureSTName = mainTextureName + "_ST";
		}

		void Awake()
		{
#if !UNITY_2017_1_OR_NEWER
			if (s_mainTexId == -1 && s_mainTexStId == -1)
			{
				StaticInitialize();
			}
#endif
#if UNITY_EDITOR
			if (string.IsNullOrEmpty(mainTextureName) && string.IsNullOrEmpty(baseColorName) && string.IsNullOrEmpty(alphaCutoffName))
			{
				// set default values when created.
				SetDefaultPropertyNames();
			}
#endif
			Initialize();
		}

		void Start()
		{
			UpdatePropertyBlock();
		}

#if UNITY_EDITOR
		static string[] s_mainTextureNames = { "_BaseColorMap", "_MainTex" };
		static string[] s_baseColorNames = { "_BaseColor", "_Color" };
		static string[] s_alphaCutoffNames = { "_Cutoff" };
		static string[] s_cullModeNames = { "_Cull" };
		public void SetDefaultPropertyNames()
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			Material material = m_renderer.sharedMaterial;
			if (material != null)
			{
				UnityEditor.MaterialProperty[] materialProperties = UnityEditor.MaterialEditor.GetMaterialProperties(new Object[] { material });
				mainTextureName = null;
				foreach (string name in s_mainTextureNames)
				{
					foreach (var prop in materialProperties)
					{
						if (prop.name == name)
						{
							mainTextureName = name;
							break;
						}
					}
					if (mainTextureName != null)
					{
						break;
					}
				}
				baseColorName = null;
				foreach (string name in s_baseColorNames)
				{
					foreach (var prop in materialProperties)
					{
						if (prop.name == name)
						{
							baseColorName = name;
							break;
						}
					}
					if (baseColorName != null)
					{
						break;
					}
				}
				alphaCutoffName = null;
				foreach (string name in s_alphaCutoffNames)
				{
					foreach (var prop in materialProperties)
					{
						if (prop.name == name)
						{
							alphaCutoffName = name;
							break;
						}
					}
					if (alphaCutoffName != null)
					{
						break;
					}
					if (material.HasProperty(name))
					{
						alphaCutoffName = name;
						break;
					}
				}
				cullModeName = null;
				foreach (string name in s_cullModeNames)
				{
					foreach (var prop in materialProperties)
					{
						if (prop.name == name)
						{
							cullModeName = name;
							break;
						}
					}
					if (cullModeName != null)
					{
						break;
					}
					if (material.HasProperty(name))
					{
						cullModeName = name;
						break;
					}
				}
				if (cullModeName != null)
				{
					copyCullModeFromMaterial = true;
				}
			}
#if UNITY_2019_1_OR_NEWER
			if (UnityEngine.Rendering.RenderPipelineManager.currentPipeline != null)
			{
				if (string.IsNullOrEmpty(mainTextureName))
				{
					mainTextureName = s_mainTextureNames[0];
				}
				if (string.IsNullOrEmpty(baseColorName))
				{
					baseColorName = s_baseColorNames[0];
				}
				if (string.IsNullOrEmpty(alphaCutoffName))
				{
					alphaCutoffName = s_alphaCutoffNames[0];
				}
				if (string.IsNullOrEmpty(cullModeName))
				{
					cullModeName = s_cullModeNames[0];
				}
			}
			else
#endif
			{
				if (string.IsNullOrEmpty(mainTextureName))
				{
					mainTextureName = s_mainTextureNames[s_mainTextureNames.Length - 1];
				}
				if (string.IsNullOrEmpty(baseColorName))
				{
					baseColorName = s_baseColorNames[s_baseColorNames.Length - 1];
				}
				if (string.IsNullOrEmpty(alphaCutoffName))
				{
					alphaCutoffName = s_alphaCutoffNames[s_alphaCutoffNames.Length - 1];
				}
				if (string.IsNullOrEmpty(cullModeName))
				{
					cullModeName = s_cullModeNames[s_cullModeNames.Length - 1];
				}
			}
		}
		private void OnValidate()
		{
			m_renderer = GetComponent<Renderer>();
			if (m_renderer == null)
			{
				Debug.LogError("No renderer found! Please do not add 'Main Tex Property Block' component to a non-renderer object.");
				return;
			}
			if (string.IsNullOrEmpty(mainTextureName) && string.IsNullOrEmpty(baseColorName) && string.IsNullOrEmpty(alphaCutoffName))
			{
				// set default values when created.
				SetDefaultPropertyNames();
			}
			Initialize();

			UpdatePropertyBlock();
		}
		void Update()
		{
			if (m_renderer == null)
			{
				return;
			}
			if (!Application.isPlaying)
			{
				UpdatePropertyBlock();
				return;
			}
			m_renderer.GetPropertyBlock(m_propertyBlock);
			if (copyMainTexFromMaterial)
			{
				Texture mainTex = GetMaterialMainTexture();
				if (mainTex != m_propertyBlock.GetTexture(s_dspMainTexId))
				{
					Debug.LogWarning("Changing the main texture of the material may not be reflected until UpdatePropertyBlock() is called!", this);
				}
				Vector4 mainTexST = GetMaterialMainTextureST();
				if (mainTexST != m_propertyBlock.GetVector(s_dspMainTexStId))
				{
					Debug.LogWarning("Changing the main texture uv of the material may not be reflected until UpdatePropertyBlock() is called!", this);
				}
			}
			if (copyBaseColorFromMaterial)
			{
				float alpha = GetMaterialBaseColor().a;
				if (alpha != m_propertyBlock.GetVector(s_dspBaseAlphaId).w)
				{
					Debug.LogWarning("Changing the base coloar alpha of the material may not be reflected until UpdatePropertyBlock() is called!", this);
				}
			}
			if (copyAlphaCutoffFromMaterial)
			{
				if (enableAlphaCutoff || (enableAlphaCutoffByKeyword && m_renderer.material.IsKeywordEnabled(alphaCutoffKeyword)))
				{
					float cutoff = GetMaterialAlphaCutoff();
					Vector4 vec = m_propertyBlock.GetVector(s_dspBaseAlphaId);
					if (cutoff != vec.z)
					{
						Debug.LogWarning("Changing the alpha cutoff of the material may not be reflected until UpdatePropertyBlock() is called!", this);
					}
				}
			}
		}
#endif
	}
}
