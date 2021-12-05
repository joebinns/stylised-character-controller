//
// ShadowTextureRenderer.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace DynamicShadowProjector {
#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
#else
	[ExecuteInEditMode]
#endif
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Projector))]
#pragma warning disable CS1692, RECS0001
    public partial class ShadowTextureRenderer : MonoBehaviour, ISerializationCallbackReceiver
	{
#pragma warning restore // CS1692, RECS0001
        public enum TextureMultiSample {
			x1 = 1,
			x2 = 2,
			x4 = 4,
			x8 = 8,
		}
		public enum TextureSuperSample {
			x1 = 1,
			x4 = 2,
			x16 = 4,
		}
		public enum MipmapFalloff {
			None = 0,
			Linear,
			Custom,
		}
		public enum BlurFilter {
			Uniform = 0,
			Gaussian,
		}
		// Serialize Fields
		[SerializeField]
		private TextureMultiSample m_multiSampling = TextureMultiSample.x4;
		[SerializeField]
		private TextureSuperSample m_superSampling = TextureSuperSample.x1;
		[SerializeField]
		private MipmapFalloff m_mipmapFalloff = MipmapFalloff.Linear;
		[SerializeField]
		private BlurFilter m_blurFilter = BlurFilter.Uniform;
		[SerializeField]
		private bool m_testViewClip = true;
		[SerializeField]
		private int m_textureWidth = 64;
		[SerializeField]
		private int m_textureHeight = 64;
		[SerializeField]
		private int m_mipLevel = 0;
		[SerializeField]
		private int m_blurLevel = 1;
		[SerializeField]
		private float m_blurSize = 3;
		[SerializeField]
		private float m_mipmapBlurSize = 0;
		[SerializeField]
		private bool m_singlePassMipmapBlur = false;
		[SerializeField]
		private Color m_shadowColor = new Color(0, 0, 0, 1);
		[SerializeField]
		private Material m_blurShader;
		[SerializeField]
		private Material m_downsampleShader;
		[SerializeField]
		private Material m_copyMipmapShader;
		[SerializeField]
		private Material m_eraseShadowShader;
		[SerializeField]
		private float[] m_customMipmapFalloff;
		[SerializeField]
		private RenderTextureFormat[] m_preferredTextureFormats;
		[SerializeField]
		private Camera[] m_camerasForViewClipTest;

		// public properties
		public TextureMultiSample multiSampling
		{
			get { return m_multiSampling; }
			set {
				if (m_multiSampling != value) {
					m_multiSampling = value;
					SetTexturePropertyDirty();
				}
			}
		}
		public TextureSuperSample superSampling
		{
			get { return m_superSampling; }
			set {
				if (m_superSampling != value) {
					bool b = useIntermediateTexture;
					m_superSampling = value;
					if (b != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1) {
						SetTexturePropertyDirty();
					}
				}
			}
		}
		public int textureWidth
		{
			get { return m_textureWidth; }
			set {
				if (m_textureWidth != value) {
					m_textureWidth = value;
					SetTexturePropertyDirty();
				}
			}
		}
		public int textureHeight
		{
			get { return m_textureHeight; }
			set {
				if (m_textureHeight != value) {
					m_textureHeight = value;
					SetTexturePropertyDirty();
				}
			}
		}
		public RenderTextureFormat[] preferredTextureFormats
		{
			get { return m_preferredTextureFormats; }
			set {
				m_preferredTextureFormats = value;
				SetTexturePropertyDirty();
			}
		}
		public int mipLevel
		{
			get { return m_mipLevel; }
			set {
				if (m_mipLevel != value) {
					if (m_mipLevel == 0 || value == 0) {
						SetTexturePropertyDirty();
					}
					m_mipLevel = value;
				}
			}
		}
		public int blurLevel
		{
			get { return m_blurLevel; }
			set
			{
				if (m_blurLevel != value) {
					bool b = useIntermediateTexture;
					m_blurLevel = value;
					if (b != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1) {
						SetTexturePropertyDirty();
					}
				}
			}
		}
		public float blurSize
		{
			get { return m_blurSize; }
			set { m_blurSize = value; }
		}
		public BlurFilter blurFilter
		{
			get { return m_blurFilter; }
			set { m_blurFilter = value; }
		}
		public float mipmapBlurSize
		{
			get { return m_mipmapBlurSize; }
			set
			{
				m_mipmapBlurSize = value;
			}
		}
		public bool singlePassMipmapBlur {
			get { return m_singlePassMipmapBlur; }
			set { m_singlePassMipmapBlur = value; }
		}
		public MipmapFalloff mipmapFalloff
		{
			get { return m_mipmapFalloff; }
			set
			{
				m_mipmapFalloff = value;
			}
		}
		public float[] customMipmapFalloff
		{
			get { return m_customMipmapFalloff; }
			set
			{
				m_customMipmapFalloff = value;
			}
		}
		public Color shadowColor
		{
			get { return m_shadowColor; }
			set {
				if (m_shadowColor != value) {
					bool b = useIntermediateTexture;
					m_shadowColor = value;
					if (b != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1) {
						SetTexturePropertyDirty();
					}
				}
			}
		}
		public Material blurShader
		{
			get { return m_blurShader; }
			set
			{
				m_blurShader = value;
			}
		}
		public Material downsampleShader
		{
			get { return m_downsampleShader; }
			set { m_downsampleShader = value; }
		}
		public Material copyMipmapShader
		{
			get { return m_copyMipmapShader; }
			set
			{
				m_copyMipmapShader = value;
			}
		}
		public Material eraseShadowShader
		{
			get { return m_eraseShadowShader; }
			set { m_eraseShadowShader = value; }
		}
		public RenderTexture shadowTexture
		{
			get; private set;
		}
		public bool testViewClip
		{
			get { return m_testViewClip; }
			set { m_testViewClip = value; }
		}
		public Camera[] camerasForViewClipTest
		{
			get { return m_camerasForViewClipTest; }
			set { m_camerasForViewClipTest = value; }
		}
		public Camera projectorCamera
		{
			get
			{
				if (m_camera == null) Initialize();
				return m_camera;
			}
		}
		public float cameraNearClipPlane
		{
			get {
				if (m_camera == null) {
					Initialize();
				}
				return m_camera.nearClipPlane;
			}
			set {
				if (m_camera == null) {
					Initialize();
				}
				m_camera.nearClipPlane = value;
			}
		}
		public LayerMask cameraCullingMask
		{
			get {
				if (m_camera == null) {
					Initialize();
				}
				return m_camera.cullingMask;
			}
			set {
				if (m_camera == null) {
					Initialize();
				}
				m_camera.cullingMask = value;
			}
		}
		public void SetReplacementShader(Shader shader, string replacementTag)
		{
			if (m_camera == null) {
				Initialize();
			}
			if (shader != null) {
				m_camera.SetReplacementShader(shader, replacementTag);
			}
			else {
				m_camera.ResetReplacementShader();
			}
		}

		private static int s_falloffParamID;
		private static int s_blurOffsetHParamID;
		private static int s_blurOffsetVParamID;
		private static int s_blurWeightHParamID;
		private static int s_blurWeightVParamID;
		private static int s_downSampleBlurOffset0ParamID;
		private static int s_downSampleBlurOffset1ParamID;
		private static int s_downSampleBlurOffset2ParamID;
		private static int s_downSampleBlurOffset3ParamID;
		private static int s_downSampleBlurWeightParamID;
		private static int s_blitSrcTexParamID;
		private static int s_blitSrcTexelSizeParamID;
		private static int s_shadowTexParamID;
		private static int s_mipLevelParamID;
		private Projector m_projector;
		private Material m_projectorMaterial;
		private CommandBuffer m_commandBuffer;
		[SerializeField] [HideInInspector]
		private Camera m_camera;
		private CommandBuffer m_postProcessCommands = null;
		private bool m_isTexturePropertyChanged;
		private bool m_isVisible = false;
		private bool m_shadowTextureValid = false;

		public bool isProjectorVisible
		{
			get { return m_isVisible; }
		}

		private static bool isBuiltinRP
		{
			get
			{
#if UNITY_2019_3_OR_NEWER
				return GraphicsSettings.currentRenderPipeline == null;
#else
				return GraphicsSettings.renderPipelineAsset == null;
#endif
			}
		}
		// Call SetCommandBufferDirty or UpdateCommandBuffer when child objects are added/deleted/disabled/enabled.
		public void SetTexturePropertyDirty()
		{
			m_isTexturePropertyChanged = true;
		}
		public void CreateRenderTexture()
		{
			if (IsPrefabAsset())
			{
				return;
			}
			if (m_textureWidth <= 0 || m_textureHeight <= 0 || m_projector == null) {
				return;
			}
			// choose a texture format
			RenderTextureFormat textureFormat = RenderTextureFormat.ARGB32;
			if (m_preferredTextureFormats != null && 0 < m_preferredTextureFormats.Length) {
				foreach (RenderTextureFormat format in m_preferredTextureFormats) {
					if (SystemInfo.SupportsRenderTextureFormat(textureFormat)) {
						textureFormat = format;
					}
				}
			}
			// create texture
			if (shadowTexture != null) {
				if (m_camera != null) {
					m_camera.targetTexture = null;
				}
				DestroyImmediate(shadowTexture);
			}
			shadowTexture = new RenderTexture(m_textureWidth, m_textureHeight, 0, textureFormat, RenderTextureReadWrite.Linear);
			shadowTexture.hideFlags = HideFlags.DontSave;
			if (useIntermediateTexture) {
				shadowTexture.antiAliasing = 1;
			}
			else {
				shadowTexture.antiAliasing = (int)m_multiSampling;
			}
			if (0 < m_mipLevel) {
				shadowTexture.useMipMap = true;
#if UNITY_5_5_OR_NEWER
				shadowTexture.autoGenerateMips = false;
#else
				shadowTexture.generateMips = false;
#endif
				shadowTexture.mipMapBias = 0.0f;
				shadowTexture.filterMode = FilterMode.Trilinear;
			}
			else {
				shadowTexture.useMipMap = false;
				shadowTexture.filterMode = FilterMode.Bilinear;
			}
			shadowTexture.wrapMode = TextureWrapMode.Clamp;
			shadowTexture.Create();
			m_shadowTextureValid = false;
			if (m_projector.material != null) {
				m_projector.material.SetTexture("_ShadowTex", shadowTexture);
				m_projector.material.SetFloat("_DSPMipLevel", m_mipLevel);
			}
			if (m_camera != null) {
				m_camera.targetTexture = shadowTexture;
			}
			m_isTexturePropertyChanged = false;
			OnRenderTextureCreated();
		}
		internal void AddCommandBuffer(CommandBuffer commandBuffer)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer); // just in case
			m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		}

		internal void RemoveCommandBuffer(CommandBuffer commandBuffer)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		}

		bool useIntermediateTexture
		{
			get {
				return m_superSampling != TextureSuperSample.x1 || 0 < m_blurLevel || HasShadowColor() || (0 < m_mipLevel && m_multiSampling != TextureMultiSample.x1);
			}
		}
#if !UNITY_2017_1_OR_NEWER
        static bool s_staticInitialized = false;
#endif
        partial void PartialInitialize();
		partial void OnRenderTextureCreated();
		bool Initialize()
		{
#if !UNITY_2017_1_OR_NEWER
            if (!s_staticInitialized)
            {
                StaticInitialize();
                s_staticInitialized = true;
            }
#endif
			if (IsInitialized()) {
				return true;
			}
			m_isVisible = false;
			m_isTexturePropertyChanged = true;
			m_projector = GetComponent<Projector>();
			CloneProjectorMaterialIfShared();
			if (m_camera == null) {
				m_camera = gameObject.GetComponent<Camera>();
				if (m_camera == null) {
					m_camera = gameObject.AddComponent<Camera>();
				}
				m_camera.hideFlags = HideFlags.HideInInspector;
#if UNITY_EDITOR
				if (IsPrefabAsset())
				{
					UnityEditor.EditorUtility.SetDirty(gameObject);
				}
#endif
			}
			else {
				m_camera.RemoveAllCommandBuffers();
			}
			m_camera.depth = -100;
			m_camera.cullingMask = 0;
			m_camera.clearFlags = CameraClearFlags.Nothing;
			m_camera.backgroundColor = new Color(1, 1, 1, 0);
			m_camera.useOcclusionCulling = false;
			m_camera.renderingPath = RenderingPath.Forward;
			m_camera.nearClipPlane = 0.01f;
			m_camera.opaqueSortMode = OpaqueSortMode.NoDistanceSort;
#if UNITY_5_6_OR_NEWER
			m_camera.forceIntoRenderTexture = true;
			m_camera.allowHDR = false;
			m_camera.allowMSAA = false;
			m_camera.depthTextureMode = DepthTextureMode.None;
#endif
#if UNITY_2017_3_OR_NEWER
			m_camera.allowDynamicResolution = false;
#endif
			m_camera.stereoTargetEye = StereoTargetEyeMask.None;
			m_camera.enabled = true;
			CreateRenderTexture();
			PartialInitialize();
			return true;
		}

		bool IsInitialized()
		{
			return m_projector != null && m_camera != null;
		}

		void Awake()
		{
			Initialize();
		}

		void OnEnable()
		{
			if (m_camera != null) {
				m_camera.enabled = isBuiltinRP;
			}
		}

		void OnDisable()
		{
			if (m_camera != null) {
				m_camera.enabled = false;
			}
		}

		void Start()
		{
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
			if (m_testViewClip && Application.isPlaying && !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
#else
			if (m_testViewClip && Application.isPlaying)
#endif
#else
			if (m_testViewClip)
#endif
			{
				bool isEmpty = m_camerasForViewClipTest == null || m_camerasForViewClipTest.Length == 0;
				if (isEmpty)
				{
					if (Camera.main != null)
					{
						m_camerasForViewClipTest = new Camera[1] { Camera.main };
					}
				}
				else
				{
					bool hasNonNullCamera = false;
					foreach (Camera cam in m_camerasForViewClipTest)
					{
						if (cam != null)
						{
							hasNonNullCamera = true;
							break;
						}
					}
					if (!hasNonNullCamera)
					{
						Debug.LogError("The array of 'Cameras For View Clip Test' does not have any valid cameras. The main camera is temporarily added to the array.", this);
						Camera[] cameras = new Camera[m_camerasForViewClipTest.Length + 1];
						int i = 0;
						for (; i < m_camerasForViewClipTest.Length; ++i)
						{
							cameras[i] = m_camerasForViewClipTest[i];
						}
						cameras[i] = Camera.main;
						m_camerasForViewClipTest = cameras;
					}
				}
			}
		}

		private static bool IsPrefabAsset(Object obj)
		{
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
			if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(obj))
			{
				return true;
			}
#else
            UnityEditor.PrefabType prefabType = UnityEditor.PrefabUtility.GetPrefabType(obj);
            if (prefabType == UnityEditor.PrefabType.Prefab || prefabType == UnityEditor.PrefabType.ModelPrefab)
            {
                return true;
            }
#endif
#endif
			return false;
		}
		private bool IsPrefabAsset()
		{
			return IsPrefabAsset(this);
		}
		void OnValidate()
		{
			// check custom mipmap falloff
			if (m_mipmapFalloff == MipmapFalloff.Custom && 0 < m_mipLevel) {
				if (m_customMipmapFalloff == null || m_customMipmapFalloff.Length == 0) {
					m_customMipmapFalloff = new float[m_mipLevel];
					for (int i = 0; i < m_mipLevel; ++i) {
						m_customMipmapFalloff[i] = ((float)(m_mipLevel - i)) / (float)(m_mipLevel + 1);
					}
				}
				else if (m_mipLevel != m_customMipmapFalloff.Length) {
					float[] customFalloff = new float[m_mipLevel];
					for (int i = 0; i < m_mipLevel; ++i) {
						float oldIndex = ((float)(m_customMipmapFalloff.Length + 1) * (i + 1)) / (float)(m_mipLevel + 1);
						int j = Mathf.FloorToInt(oldIndex);
						float w = oldIndex - j;
						float v0 = (j == 0 ? 1.0f : m_customMipmapFalloff[j - 1]);
						float v1 = (j < m_customMipmapFalloff.Length) ? m_customMipmapFalloff[j] : 0.0f;
						customFalloff[i] = Mathf.Lerp(v0, v1, w);
					}
					m_customMipmapFalloff = customFalloff;
				}
			}
#if !UNITY_2017_1_OR_NEWER
            if (!s_staticInitialized)
            {
                StaticInitialize();
                s_staticInitialized = true;
            }
#endif
            CreateRenderTexture();
			if (!IsPrefabAsset())
			{
				if (m_camerasForViewClipTest != null)
				{
					for (int i = 0; i < m_camerasForViewClipTest.Length; ++i)
					{
						if (IsPrefabAsset(m_camerasForViewClipTest[i]))
						{
							Debug.LogError("No prefab assets can be added to 'Cameras For View Clip Test'!", this);
							m_camerasForViewClipTest[i] = null;
						}
					}
				}
			}
            if (!IsInitialized() && !Initialize())
			{
				m_camera.enabled = false;
				return;
			}
			if (IsPrefabAsset())
			{
				return;
			}
			if (m_projector.material == null)
			{
				if (m_isVisible)
				{
					SetVisible(false);
				}
				m_camera.enabled = false;
				return;
			}
			m_projector.material.SetTexture(s_shadowTexParamID, shadowTexture);
			m_projector.material.SetFloat(s_mipLevelParamID, m_mipLevel);
			if (!IsReadyToExecute())
			{
				m_camera.enabled = false;
				return;
			}
		}
		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				CreateRenderTexture();
			}
		}
		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				CreateRenderTexture();
			}
		}
		private static HashSet<Material> s_sharedMaterials;
		const HideFlags CLONED_MATERIAL_HIDE_FLAGS = HideFlags.HideAndDontSave;
		void CloneProjectorMaterialIfShared()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				return;
			}
#if UNITY_2018_3_OR_NEWER
			if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
			{
				return;
			}
#if UNITY_2021_2_OR_NEWER
			var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
			var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
			if (prefabStage != null && prefabStage.IsPartOfPrefabContents(gameObject))
			{
				return;
			}
#endif
#endif
			if (m_projector.material == null || m_projector.material == m_projectorMaterial) {
				return;
			}
			if (m_projectorMaterial != null && m_projectorMaterial.hideFlags == CLONED_MATERIAL_HIDE_FLAGS) {
				DestroyImmediate(m_projectorMaterial);
			}
			if (s_sharedMaterials == null) {
				s_sharedMaterials = new HashSet<Material>();
			}
			if (s_sharedMaterials.Contains(m_projector.material)) {
				m_projector.material = new Material(m_projector.material);
				m_projector.material.hideFlags = CLONED_MATERIAL_HIDE_FLAGS;
			}
			else {
				s_sharedMaterials.Add(m_projector.material);
			}
			m_projectorMaterial = m_projector.material;
		}

		void OnDestroy()
		{
			if (m_projectorMaterial != null) {
				if (s_sharedMaterials != null && s_sharedMaterials.Contains(m_projectorMaterial)) {
					s_sharedMaterials.Remove(m_projectorMaterial);
				}
				if (m_projectorMaterial.hideFlags == CLONED_MATERIAL_HIDE_FLAGS) {
					if (m_projector.material == m_projectorMaterial) {
						m_projector.material = null;
					}
					DestroyImmediate(m_projectorMaterial);
				}
			}
			if (shadowTexture != null) {
				if (m_camera != null) {
					m_camera.targetTexture = null;
				}
				DestroyImmediate(shadowTexture);
				shadowTexture = null;
			}
			if (m_camera != null) {
				m_camera.RemoveAllCommandBuffers();
				m_camera = null;
			}
			m_isVisible = false;
		}

		bool IsReadyToExecute()
		{
			if (m_projector == null || m_projector.material == null)
			{
				return false;
			}
			if (m_textureWidth <= 0 || m_textureHeight <= 0 || m_eraseShadowShader == null) {
				return false;
			}
			if (0 < m_mipLevel || m_superSampling != TextureSuperSample.x1) {
				if (m_downsampleShader == null) {
					return false;
				}
			}
			if (0 < m_blurLevel || (0.0f < m_mipmapBlurSize && 0 < m_mipLevel)) {
				if (m_blurShader == null) {
					return false;
				}
			}
			if (0 < m_mipLevel && (m_copyMipmapShader == null || m_downsampleShader == null)) {
				return false;
			}
			return true;
		}

		void SetVisible(bool isVisible)
		{
			m_isVisible = isVisible;
			SendMessage("OnVisibilityChanged", isVisible);
		}

		public CommandBuffer postProcessCommandBuffer
		{
			get
			{
				if (m_postProcessCommands == null)
				{
					m_postProcessCommands = new CommandBuffer();
				}
				return m_postProcessCommands;
			}
		}
		static int[] s_temporaryRenderTargetNameIds;
		static RenderTargetIdentifier[] s_temporaryRenderTargetIdentifiers;
        static void StaticInitialize()
        {
            s_falloffParamID = Shader.PropertyToID("_Falloff");
            s_blurOffsetHParamID = Shader.PropertyToID("_OffsetH");
            s_blurOffsetVParamID = Shader.PropertyToID("_OffsetV");
            s_blurWeightHParamID = Shader.PropertyToID("_WeightH");
            s_blurWeightVParamID = Shader.PropertyToID("_WeightV");
            s_downSampleBlurOffset0ParamID = Shader.PropertyToID("_Offset0");
            s_downSampleBlurOffset1ParamID = Shader.PropertyToID("_Offset1");
            s_downSampleBlurOffset2ParamID = Shader.PropertyToID("_Offset2");
            s_downSampleBlurOffset3ParamID = Shader.PropertyToID("_Offset3");
            s_downSampleBlurWeightParamID = Shader.PropertyToID("_Weight");
            s_blitSrcTexParamID = Shader.PropertyToID("_MainTex");
            s_blitSrcTexelSizeParamID = Shader.PropertyToID("_MainTex_TexelSize");
			s_shadowTexParamID = Shader.PropertyToID("_ShadowTex");
			s_mipLevelParamID = Shader.PropertyToID("_DSPMipLevel");
			s_temporaryRenderTargetNameIds = new int[] {
                Shader.PropertyToID("_TemporaryColorTexture_1"),
                Shader.PropertyToID("_TemporaryColorTexture_2")
            };
            s_temporaryRenderTargetIdentifiers = new RenderTargetIdentifier[] {
                new RenderTargetIdentifier(s_temporaryRenderTargetNameIds[0]),
                new RenderTargetIdentifier(s_temporaryRenderTargetNameIds[1])
            };
        }
#if UNITY_2017_1_OR_NEWER
        static ShadowTextureRenderer()
		{
            StaticInitialize();
		}
#endif
		RenderTexture m_temporaryRenderTarget = null;
		void CreateTemporaryRenderTarget()
		{
			if (useIntermediateTexture)
			{
				Debug.Assert(m_temporaryRenderTarget == null, "Please don't forget to call ReleaseTemporaryRenderTarget.");
				int width = m_textureWidth * (int)m_superSampling;
				int height = m_textureHeight * (int)m_superSampling;
				m_temporaryRenderTarget = RenderTexture.GetTemporary(width, height, 0, shadowTexture.format, RenderTextureReadWrite.Linear, (int)m_multiSampling);
				m_temporaryRenderTarget.filterMode = FilterMode.Bilinear;
			}
		}
		void ReleaseTemporaryRenderTarget()
		{
			if (m_temporaryRenderTarget != null)
			{
				RenderTexture.ReleaseTemporary(m_temporaryRenderTarget);
				m_temporaryRenderTarget = null;
			}
		}
		List<MaterialPropertyBlock> m_propertyBlockPool = new List<MaterialPropertyBlock>();
		int m_propetyBlockCount = 0;
		void ClearPropertyBlockPool()
		{
			m_propetyBlockCount = 0;
		}
		MaterialPropertyBlock AllocatePropertyBlock()
		{
			if (m_propetyBlockCount == m_propertyBlockPool.Count)
			{
				m_propertyBlockPool.Add(new MaterialPropertyBlock());
			}
			return m_propertyBlockPool[m_propetyBlockCount++];
		}
		void AddPostRenderPassCommands(CommandBuffer cmd, RenderTargetIdentifier srcId)
		{
			RenderTargetIdentifier finalDstId = new RenderTargetIdentifier(shadowTexture);
			int tempRTIndex = 0;
			if (!useIntermediateTexture)
			{
				srcId = finalDstId;
			}
			else
			{
				if (m_superSampling != TextureSuperSample.x1 || HasShadowColor())
				{
					m_downsampleShader.color = m_shadowColor;
					// downsample
					RenderTargetIdentifier dstId;
					if (0 < m_blurLevel)
					{
						cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], m_textureWidth, m_textureHeight, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
						dstId = s_temporaryRenderTargetIdentifiers[tempRTIndex];
					}
					else
					{
						dstId = finalDstId;
					}
					int pass = 2;
					MaterialPropertyBlock downsampleProperties = null;
					if (m_superSampling == TextureSuperSample.x16)
					{
						pass = 0;
						downsampleProperties = AllocatePropertyBlock();
						downsampleProperties.SetVector(s_blitSrcTexelSizeParamID, new Vector4(0.25f / m_textureWidth, 0.25f / m_textureHeight));
					}
					cmd.SetRenderTarget(dstId);
					AddBlitToCurrentTargetCommand(cmd, srcId, m_downsampleShader, HasShadowColor() ? pass + 1 : pass, downsampleProperties);
					tempRTIndex ^= 1;
					if (srcId == s_temporaryRenderTargetIdentifiers[tempRTIndex])
					{
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
					}
					srcId = dstId;
				}
				if (0 < m_blurLevel)
				{
					BlurParam blurH, blurV;
					MaterialPropertyBlock blurProperties = AllocatePropertyBlock();
					SetBlurShaderProperties(blurProperties, out blurH, out blurV);

					RenderTargetIdentifier dstId;
					cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], m_textureWidth, m_textureHeight, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
					dstId = s_temporaryRenderTargetIdentifiers[tempRTIndex];
					cmd.SetRenderTarget(dstId);
					AddBlitToCurrentTargetCommand(cmd, srcId, m_blurShader, blurH.tap, blurProperties);
					tempRTIndex ^= 1;
					if (srcId == s_temporaryRenderTargetIdentifiers[tempRTIndex])
					{
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
					}
					for (int i = 1; i < m_blurLevel - 1; ++i)
					{
						cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], m_textureWidth, m_textureHeight, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
						srcId = s_temporaryRenderTargetIdentifiers[tempRTIndex];
						cmd.SetRenderTarget(srcId);
						AddBlitToCurrentTargetCommand(cmd, dstId, m_blurShader, blurV.tap, blurProperties);
						tempRTIndex ^= 1;
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
						cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], m_textureWidth, m_textureHeight, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
						cmd.SetRenderTarget(dstId);
						AddBlitToCurrentTargetCommand(cmd, srcId, m_blurShader, blurH.tap, blurProperties);
						tempRTIndex ^= 1;
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
					}
					srcId = finalDstId;
					cmd.SetRenderTarget(srcId);
					AddBlitToCurrentTargetCommand(cmd, dstId, m_blurShader, blurV.tap, blurProperties);
					tempRTIndex ^= 1;
					cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
				}
				if (srcId != finalDstId)
				{
					cmd.SetRenderTarget(finalDstId);
					AddBlitToCurrentTargetCommand(cmd, srcId, m_downsampleShader, 2, null);
					tempRTIndex ^= 1;
					if (m_mipLevel == 0 && srcId == s_temporaryRenderTargetIdentifiers[tempRTIndex])
					{
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
					}
					srcId = finalDstId;
				}
			}
			cmd.SetRenderTarget(finalDstId);
			AddEraseShadowOnBorderCommand(cmd, m_textureWidth, m_textureHeight);
			if (0 < m_mipLevel)
			{
				// setup blur parameters
				BlurParam blurH = new BlurParam(), blurV = new BlurParam();
				MaterialPropertyBlock mipmapBlurProperties = AllocatePropertyBlock();
				int w = m_textureWidth >> 1;
				int h = m_textureHeight >> 1;
				if (0.1f < m_mipmapBlurSize)
				{
					GetMipmapBlurParams(ref blurH, ref blurV);
					if (m_singlePassMipmapBlur)
					{
						SetMipmapDownsampleWithBlurProperties(mipmapBlurProperties, blurH, blurV, w, h);
					}
					else
					{
						SetMipmapBlurShaderProperties(mipmapBlurProperties, blurH, blurV);
					}
				}
				cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], w, h, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
				RenderTargetIdentifier dstId = s_temporaryRenderTargetIdentifiers[tempRTIndex];
				bool downSampleWithBlur = m_singlePassMipmapBlur && 0.1f < m_mipmapBlurSize;
				mipmapBlurProperties.SetVector(s_blitSrcTexelSizeParamID, new Vector4(0.5f / w, 0.5f / h));
				cmd.SetRenderTarget(dstId);
				if (srcId == finalDstId)
				{
					if (downSampleWithBlur)
					{
						AddBlitToCurrentTargetCommand(cmd, srcId, m_downsampleShader, 5, mipmapBlurProperties);
					}
					else
					{
						AddBlitToCurrentTargetCommand(cmd, srcId, m_copyMipmapShader, 1, mipmapBlurProperties);
					}
				}
				else
				{
					AddBlitToCurrentTargetCommand(cmd, srcId, m_downsampleShader, downSampleWithBlur ? 4 : 0, mipmapBlurProperties);
				}
				tempRTIndex ^= 1;
				if (srcId == s_temporaryRenderTargetIdentifiers[tempRTIndex])
				{
					cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
				}
				srcId = dstId;
				int i = 0;
				float falloff = 1.0f;
				for (; ; )
				{
					if (0.1f < m_mipmapBlurSize && !m_singlePassMipmapBlur)
					{
						cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], w, h, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
						dstId = s_temporaryRenderTargetIdentifiers[tempRTIndex];
						cmd.SetRenderTarget(dstId);
						AddBlitToCurrentTargetCommand(cmd, srcId, m_blurShader, blurH.tap, mipmapBlurProperties);
						tempRTIndex ^= 1;
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
						cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], w, h, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
						cmd.SetRenderTarget(srcId);
						AddBlitToCurrentTargetCommand(cmd, dstId, m_blurShader, blurV.tap, mipmapBlurProperties);
						tempRTIndex ^= 1;
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
					}
					if (m_mipmapFalloff == MipmapFalloff.Linear)
					{
						falloff = ((float)(m_mipLevel - i)) / (m_mipLevel + 1.0f);
					}
					else if (m_mipmapFalloff == MipmapFalloff.Custom && m_customMipmapFalloff != null && 0 < m_customMipmapFalloff.Length)
					{
						falloff = m_customMipmapFalloff[Mathf.Min(i, m_customMipmapFalloff.Length - 1)];
					}
					MaterialPropertyBlock properties = AllocatePropertyBlock();
					properties.SetFloat(s_falloffParamID, falloff);
					++i;
					cmd.SetRenderTarget(finalDstId, i);
					AddBlitToCurrentTargetCommand(cmd, srcId, m_copyMipmapShader, 0, properties);
					AddEraseShadowOnBorderCommand(cmd, w, h);
					w = Mathf.Max(1, w >> 1);
					h = Mathf.Max(1, h >> 1);
					if (i == m_mipLevel || w <= 4 || h <= 4)
					{
						tempRTIndex ^= 1;
						cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
						break;
					}
					cmd.GetTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex], w, h, 0, FilterMode.Bilinear, shadowTexture.format, RenderTextureReadWrite.Linear);
					dstId = s_temporaryRenderTargetIdentifiers[tempRTIndex];
					cmd.SetRenderTarget(dstId);
					if (downSampleWithBlur)
					{
						SetMipmapDownsampleWithBlurProperties(properties, blurH, blurV, w, h);
						AddBlitToCurrentTargetCommand(cmd, srcId, m_downsampleShader, 4, properties);
					}
					else
					{
						properties.SetVector(s_blitSrcTexelSizeParamID, new Vector4(0.5f / w, 0.5f / h));
						AddBlitToCurrentTargetCommand(cmd, srcId, m_downsampleShader, 0, properties);
					}
					tempRTIndex ^= 1;
					cmd.ReleaseTemporaryRT(s_temporaryRenderTargetNameIds[tempRTIndex]);
					srcId = dstId;
				}
				while (1 <= w || 1 <= h)
				{
					++i;
					cmd.SetRenderTarget(finalDstId, i);
					cmd.ClearRenderTarget(false, true, new Color(1, 1, 1, 0));
					w = w >> 1;
					h = h >> 1;
				}
			}
		}
		static Dictionary<int, Mesh> s_borderMeshCache = new Dictionary<int, Mesh>();
		static readonly int[] s_borderMeshIndices = { 0, 1, 2, 3, 4, 5, 6, 7 };
		void AddEraseShadowOnBorderCommand(CommandBuffer cmd, int w, int h)
		{
			int key = w << 16 + h;
			Mesh mesh = null;
			if (!s_borderMeshCache.TryGetValue(key, out mesh) || mesh == null)
			{
				float x = 1.0f - 1.0f / w;
				float y = 1.0f - 1.0f / h;
				Vector3[] borderVertices = new Vector3[8];
				borderVertices[0] = new Vector3(-1.0f, -y, 0);
				borderVertices[1] = new Vector3(1.0f, -y, 0);
				borderVertices[2] = new Vector3(x, -1.0f, 0);
				borderVertices[3] = new Vector3(x, 1.0f, 0);
				borderVertices[4] = new Vector3(1.0f, y, 0);
				borderVertices[5] = new Vector3(-1.0f, y, 0);
				borderVertices[6] = new Vector3(-x, 1.0f, 0);
				borderVertices[7] = new Vector3(-x, -1.0f, 0);
				mesh = new Mesh();
				mesh.vertices = borderVertices;
				mesh.SetIndices(s_borderMeshIndices, MeshTopology.Lines, 0);
				s_borderMeshCache[key] = mesh;
			}
			cmd.DrawMesh(mesh, Matrix4x4.identity, m_eraseShadowShader);
		}
		static Mesh s_fullScreenQuad = null;
		void AddBlitToCurrentTargetCommand(CommandBuffer cmd, RenderTargetIdentifier srcId, Material mat, int pass, MaterialPropertyBlock properties)
		{
			if (s_fullScreenQuad == null)
			{
				s_fullScreenQuad = new Mesh();
				s_fullScreenQuad.vertices = new Vector3[] {
					new Vector3(-1.0f, -1.0f, 0.0f),
					new Vector3(-1.0f,  1.0f, 0.0f),
					new Vector3( 1.0f, -1.0f, 0.0f),
					new Vector3( 1.0f,  1.0f, 0.0f)
				};
				s_fullScreenQuad.uv = new Vector2[] {
					new Vector2(0.0f, 0.0f),
					new Vector2(0.0f, 1.0f),
					new Vector2(1.0f, 0.0f),
					new Vector2(1.0f, 1.0f)
				};
				s_fullScreenQuad.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
			}
			cmd.SetGlobalTexture(s_blitSrcTexParamID, srcId);
			cmd.DrawMesh(s_fullScreenQuad, Matrix4x4.identity, mat, 0, pass, properties);
		}
		void Update()
		{
			if (!IsReadyToExecute())
			{
				m_camera.enabled = false;
				return;
			}
			if (isBuiltinRP && !m_camera.enabled) {
				m_camera.enabled = true; // To call OnPreCull
			}
#if UNITY_EDITOR
			// set the shadow texture to targetTexture so that Scene View can distinguish m_camera from the main camera.
			// targetTexture can be null after serialization.
			if (m_camera.targetTexture == null)
			{
				m_camera.targetTexture = shadowTexture;
			}
#endif
		}

		void OnPreCull()
		{
			if (m_projector.material != m_projectorMaterial) {
				// projector material changed.
				CloneProjectorMaterialIfShared();
				m_projector.material.SetTexture(s_shadowTexParamID, shadowTexture);
				m_projector.material.SetFloat(s_mipLevelParamID, m_mipLevel);
			}
			if (m_isTexturePropertyChanged) {
				CreateRenderTexture();
			}
			m_camera.orthographic = m_projector.orthographic;
			m_camera.orthographicSize = m_projector.orthographicSize;
			m_camera.fieldOfView = m_projector.fieldOfView;
			m_camera.aspect = m_projector.aspectRatio;
			m_camera.farClipPlane = m_projector.farClipPlane;
			// view clip test
			bool isVisible = true;
			if (!m_projector.enabled) {
				isVisible = false;
			}
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
			else if (m_testViewClip && Application.isPlaying && !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
#else
			else if (m_testViewClip && Application.isPlaying)
#endif
#else
			else if (m_testViewClip)
#endif
			{
				if (m_camerasForViewClipTest != null && 0 < m_camerasForViewClipTest.Length) {
					Vector3 v0 = m_camera.ViewportToWorldPoint(new Vector3(0,0,m_camera.nearClipPlane));
					Vector3 v1 = m_camera.ViewportToWorldPoint(new Vector3(1,0,m_camera.nearClipPlane));
					Vector3 v2 = m_camera.ViewportToWorldPoint(new Vector3(0,1,m_camera.nearClipPlane));
					Vector3 v3 = m_camera.ViewportToWorldPoint(new Vector3(1,1,m_camera.nearClipPlane));
					Vector3 v4 = m_camera.ViewportToWorldPoint(new Vector3(0,0,m_camera.farClipPlane));
					Vector3 v5 = m_camera.ViewportToWorldPoint(new Vector3(1,0,m_camera.farClipPlane));
					Vector3 v6 = m_camera.ViewportToWorldPoint(new Vector3(0,1,m_camera.farClipPlane));
					Vector3 v7 = m_camera.ViewportToWorldPoint(new Vector3(1,1,m_camera.farClipPlane));
					isVisible = false;
					for (int i = 0; i < m_camerasForViewClipTest.Length; ++i) {
						Camera cam = m_camerasForViewClipTest[i];
						if (cam == null || !cam.enabled) {
							continue;
						}
						Vector3 min = cam.WorldToViewportPoint(v0);
						if (min.z < 0.0f) { min.x = -min.x; min.y = -min.y; }
						Vector3 max = min;
						Vector3 v = cam.WorldToViewportPoint(v1);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v2);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v3);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v4);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v5);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v6);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						v = cam.WorldToViewportPoint(v7);
						if (v.z < 0.0f) { v.x = -v.x; v.y = -v.y; }
						min.x = Mathf.Min(min.x, v.x); min.y = Mathf.Min(min.y, v.y); min.z = Mathf.Min(min.z, v.z);
						max.x = Mathf.Max(max.x, v.x); max.y = Mathf.Max(max.y, v.y); max.z = Mathf.Max(max.z, v.z);
						if (0.0f < max.x && min.x < 1.0f && 0.0f < max.y && min.y < 1.0f && cam.nearClipPlane < max.z && min.z < cam.farClipPlane) {
							isVisible = true;
							break;
						}
					}
				}
			}
			if (isVisible != m_isVisible) {
				SetVisible(isVisible);
			}
			if (!isVisible) {
				if (m_camera != null) {
					m_camera.enabled = false;
				}
				if (shadowTexture != null && !m_shadowTextureValid) {
					RenderTexture currentRT = RenderTexture.active;
					RenderTexture.active = shadowTexture;
					GL.Clear(false, true, new Color(1,1,1,0));
					m_shadowTextureValid = true;
					RenderTexture.active = currentRT;
				}
			}
		}

		bool HasShadowColor()
		{
			return m_shadowColor.a < 1.0f || 0.0f < (m_shadowColor.r + shadowColor.g + shadowColor.b);
		}

		void PrepareRendering()
		{
			shadowTexture.DiscardContents();
			m_shadowTextureValid = true;
			CreateTemporaryRenderTarget();
			ClearPropertyBlockPool();

			postProcessCommandBuffer.Clear();
			AddPostRenderPassCommands(postProcessCommandBuffer, new RenderTargetIdentifier(m_temporaryRenderTarget));
		}
		void OnPreRender()
		{
            PrepareRendering();
			if (useIntermediateTexture)
			{
				Debug.Assert(m_temporaryRenderTarget != null, "Please call CreateTemporaryRenderTarget in advance.");
				m_camera.targetTexture = m_temporaryRenderTarget;
			}
			else
			{
				m_camera.targetTexture = shadowTexture;
			}
			m_camera.clearFlags = CameraClearFlags.Color;
		}

		void OnPostRender()
		{
            Graphics.ExecuteCommandBuffer(postProcessCommandBuffer);
			m_camera.targetTexture = shadowTexture;
			m_camera.clearFlags = CameraClearFlags.Nothing;
			ReleaseTemporaryRenderTarget();
		}
		private const int MAX_BLUR_TAP_SIZE = 7;
		private static float[] s_blurWeights = new float[MAX_BLUR_TAP_SIZE];
		struct BlurParam {
			public int tap;
			public Vector4 offset;
			public Vector4 weight;
		};
		static BlurParam GetBlurParam(float blurSize, BlurFilter filter)
		{
			BlurParam param = new BlurParam();

			if (blurSize < 0.1f) {
				param.tap = 3;
				param.offset.x = 0.0f;
				param.offset.y = 0.0f;
				param.offset.z = 0.0f;
				param.offset.w = 0.0f;
				param.weight.x = 1.0f;
				param.weight.y = 0.0f;
				param.weight.z = 0.0f;
				param.weight.w = 0.0f;
				return param;
			}
			// calculate weights
			if (filter == BlurFilter.Gaussian) {
				// gaussian filter
				float a = 1.0f/(2.0f*blurSize*blurSize);
				float totalWeight = 1.0f;
				s_blurWeights[0] = 1.0f;
				for (int i = 1; i < s_blurWeights.Length; ++i) {
					s_blurWeights[i] = Mathf.Exp(-i*i*a);
					totalWeight += 2.0f*s_blurWeights[i];
				}
				float w = 1.0f/totalWeight;
				for (int i = 0; i < s_blurWeights.Length; ++i) {
					s_blurWeights[i] *= w;
				}
			}
			else {
				// uniform filter
				float a = 0.5f/(0.5f + blurSize);
				for (int i = 0; i < s_blurWeights.Length; ++i) {
					if (i <= blurSize) {
						s_blurWeights[i] = a;
					}
					else if (i - 1 < blurSize) {
						s_blurWeights[i] = a * (blurSize - (i - 1));
					}
					else {
						s_blurWeights[i] = 0.0f;
					}
				}
			}
			param.offset.x = 1.0f + s_blurWeights[2]/(s_blurWeights[1] + s_blurWeights[2]);
			param.offset.y = 3.0f + s_blurWeights[4]/(s_blurWeights[3] + s_blurWeights[4]); 
			param.offset.z = 5.0f + s_blurWeights[6]/(s_blurWeights[5] + s_blurWeights[6]);
			param.offset.w = 0.0f;

			if (s_blurWeights[3] < 0.02f) {
				param.tap = 3;
				float a = 0.5f/(0.5f*s_blurWeights[0] + s_blurWeights[1] + s_blurWeights[2]);
				param.weight.x = Mathf.Round(255*a*s_blurWeights[0])/255.0f;
				param.weight.y = 0.5f - 0.5f*param.weight.x;
				param.weight.z = 0.0f;
				param.weight.w = 0.0f;
			}
			else if (s_blurWeights[5] < 0.02f) {
				param.tap = 5;
				float a = 0.5f/(0.5f*s_blurWeights[0] + s_blurWeights[1] + s_blurWeights[2] + s_blurWeights[3] + s_blurWeights[4]);
				param.weight.x = Mathf.Round(255*a*s_blurWeights[0])/255.0f;
				param.weight.y = Mathf.Round(255*a*(s_blurWeights[1] + s_blurWeights[2]))/255.0f;
				param.weight.z = 0.5f - (0.5f*param.weight.x + param.weight.y);
				param.weight.w = 0.0f;
			}
			else {
				param.tap = 7;
				param.weight.x = Mathf.Round(255*s_blurWeights[0])/255.0f;
				param.weight.y = Mathf.Round(255*(s_blurWeights[1] + s_blurWeights[2]))/255.0f;
				param.weight.z = Mathf.Round(255*(s_blurWeights[3] + s_blurWeights[4]))/255.0f;
				param.weight.w = 0.5f - (0.5f*param.weight.x + param.weight.y + param.weight.z);
			}
			return param;
		}
		static BlurParam GetDownsampleBlurParam(float blurSize, BlurFilter filter)
		{
			BlurParam param = new BlurParam();
			param.tap = 4;
			if (blurSize < 0.1f) {
				param.offset.x = 0.0f;
				param.offset.y = 0.0f;
				param.offset.z = 0.0f;
				param.offset.w = 0.0f;
				param.weight.x = 1.0f;
				param.weight.y = 0.0f;
				param.weight.z = 0.0f;
				param.weight.w = 0.0f;
				return param;
			}
			// calculate weights
			if (filter == BlurFilter.Gaussian) {
				// gaussian filter
				float a = 1.0f/(2.0f*blurSize*blurSize);
				float totalWeight = 0.0f;
				for (int i = 0; i < param.tap; ++i) {
					float x = i + 0.5f;
					s_blurWeights[i] = Mathf.Exp(-x*x*a);
					totalWeight += 2.0f*s_blurWeights[i];
				}
				float w = 1.0f/totalWeight;
				for (int i = 0; i < param.tap; ++i) {
					s_blurWeights[i] *= w;
				}
			}
			else {
				// uniform filter
				float a = 0.5f/blurSize;
				for (int i = 0; i < param.tap; ++i) {
					if (i + 1 <= blurSize) {
						s_blurWeights[i] = a;
					}
					else if (i < blurSize) {
						s_blurWeights[i] = a * (blurSize - i);
					}
					else {
						s_blurWeights[i] = 0.0f;
					}
				}
			}
			param.offset.x = 0.5f + s_blurWeights[1]/(s_blurWeights[0] + s_blurWeights[1]);
			param.offset.y = 2.5f + s_blurWeights[3]/(s_blurWeights[2] + s_blurWeights[3]); 
			param.offset.z = 0.0f;
			param.offset.w = 0.0f;
			
			param.weight.x = s_blurWeights[0] + s_blurWeights[1];
			param.weight.y = s_blurWeights[2] + s_blurWeights[3];
			param.weight.z = 0.0f;
			param.weight.w = 0.0f;

			return param;
		}
		void SetBlurShaderProperties(MaterialPropertyBlock dst, out BlurParam blurH, out BlurParam blurV)
		{
			// adjust blur size according to texel aspect
			float texelAspect = (m_projector.aspectRatio * m_textureHeight) / (float)m_textureWidth;
			float blurSizeH = m_blurSize;
			float blurSizeV = m_blurSize;
			if (texelAspect < 1.0f)
			{
				blurSizeV *= texelAspect;
			}
			else
			{
				blurSizeH /= texelAspect;
			}
			// blur parameters
			blurH = GetBlurParam(blurSizeH, m_blurFilter);
			blurV = GetBlurParam(blurSizeV, m_blurFilter);
			blurH.tap = (blurH.tap - 3); // index of pass
			blurV.tap = (blurV.tap - 3) + 1; // index of pass
			dst.SetVector(s_blurOffsetHParamID, blurH.offset);
			dst.SetVector(s_blurOffsetVParamID, blurV.offset);
			dst.SetVector(s_blurWeightHParamID, blurH.weight);
			dst.SetVector(s_blurWeightVParamID, blurV.weight);
		}
		void GetMipmapBlurParams(ref BlurParam blurH, ref BlurParam blurV)
		{
			// adjust blur size according to texel aspect
			float texelAspect = (m_projector.aspectRatio * m_textureHeight) / (float)m_textureWidth;
			float blurSizeH = m_mipmapBlurSize;
			float blurSizeV = m_mipmapBlurSize;
			if (texelAspect < 1.0f)
			{
				blurSizeV *= texelAspect;
			}
			else
			{
				blurSizeH /= texelAspect;
			}
			// blur parameters
			if (m_singlePassMipmapBlur)
			{
				blurH = GetDownsampleBlurParam(2.0f + 2.0f * blurSizeH, m_blurFilter);
				blurV = GetDownsampleBlurParam(2.0f + 2.0f * blurSizeV, m_blurFilter);
			}
			else
			{
				blurH = GetBlurParam(blurSizeH, m_blurFilter);
				blurV = GetBlurParam(blurSizeV, m_blurFilter);
				blurH.tap = (blurH.tap - 3); // index of pass
				blurV.tap = (blurV.tap - 3) + 1; // index of pass
			}
		}
		void SetMipmapBlurShaderProperties(MaterialPropertyBlock dst, BlurParam blurH, BlurParam blurV)
		{
			// blur parameters
			dst.SetVector(s_blurOffsetHParamID, blurH.offset);
			dst.SetVector(s_blurOffsetVParamID, blurV.offset);
			dst.SetVector(s_blurWeightHParamID, blurH.weight);
			dst.SetVector(s_blurWeightVParamID, blurV.weight);
		}
		void SetMipmapDownsampleWithBlurProperties(MaterialPropertyBlock dst, BlurParam blurH, BlurParam blurV, int w, int h)
		{
			float invW = 0.5f/w;
			float invH = 0.5f/h;
			float offsetX0 = invW * blurH.offset.x;
			float offsetX1 = invW * blurH.offset.y;
			float offsetY0 = invH * blurV.offset.x;
			float offsetY1 = invH * blurV.offset.y;
			dst.SetVector(s_downSampleBlurOffset0ParamID, new Vector4(offsetX0, offsetY0, -offsetX0, -offsetY0));
			dst.SetVector(s_downSampleBlurOffset1ParamID, new Vector4(offsetX0, offsetY1, -offsetX0, -offsetY1));
			dst.SetVector(s_downSampleBlurOffset2ParamID, new Vector4(offsetX1, offsetY0, -offsetX1, -offsetY0));
			dst.SetVector(s_downSampleBlurOffset3ParamID, new Vector4(offsetX1, offsetY1, -offsetX1, -offsetY1));

			Vector4 weight = new Vector4(blurH.weight.x * blurV.weight.x, blurH.weight.x * blurV.weight.y, blurH.weight.y * blurV.weight.x, blurH.weight.y * blurV.weight.y);
			float a = 0.25f / (weight.x + weight.y + weight.z + weight.w);
			weight.x = Mathf.Round(255 * a * weight.x) / 255.0f;
			weight.y = Mathf.Round(255 * a * weight.y) / 255.0f;
			weight.z = Mathf.Round(255 * a * weight.z) / 255.0f;
			weight.w = 0.25f - weight.x - weight.y - weight.z;
			dst.SetVector(s_downSampleBlurWeightParamID, weight);
		}

		public void OnBeforeSerialize()
		{
			if (m_camera != null)
			{
				// prevent the render target texture from being serialized.
				// assuming that the camera component is serialized after this component.
				// this assumption seems okay because the camera is added by this component.
				m_camera.targetTexture = null;
			}
		}

		public void OnAfterDeserialize()
		{
		}
	}
}
