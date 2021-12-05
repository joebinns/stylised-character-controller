using UnityEngine;
using UnityEngine.Rendering;


namespace ProjectorForLWRP
{
	[ExecuteAlways]
	[RequireComponent(typeof(ProjectorForLWRP))]
    public class AdditionalProjectorRenderer : MonoBehaviour, ICustomRenderer
    {
		[Header("Receiver Object Filter")]
		public ProjectorForSRP.ProjectorForSRP.RenderingLayerMask renderingLayerMask = ProjectorForSRP.ProjectorForSRP.RenderingLayerMask.Everything;
		public int renderQueueLowerBound = RenderQueueRange.opaque.lowerBound;
		public int renderQueueUpperBound = RenderQueueRange.opaque.upperBound;
		[SerializeField]
		private string[] m_shaderTagList = null;
		public string[] shaderTagList
		{
			get { return m_shaderTagList; }
			set
			{
				m_shaderTagList = value;
				UpdateShaderTagIdList();
			}
		}
		[Header("Projector Rendering")]
		[SerializeField]
		private UnityEngine.Rendering.Universal.RenderPassEvent m_renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingOpaques;
		[SerializeField]
		private PerObjectData m_perObjectData = PerObjectData.None;
		[SerializeField]
		private Material m_material;
		[SerializeField]
		[HideInInspector]
		private Material m_stencilPass = null;
		[SerializeField]
		[HideInInspector]
		private ProjectorForLWRP.StencilTestOptions m_stencilTestOptions = ProjectorForLWRP.StencilTestOptions.None;

		public UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent
		{
			get { return m_renderPassEvent; }
			set { m_renderPassEvent = value; }
		}
		public PerObjectData perObjectData
		{
			get { return m_perObjectData; }
			set { m_perObjectData = value; }
		}
		public bool useStencilTest
		{
			get { return m_stencilPass != null; }
		}
		public ProjectorForLWRP.StencilTestOptions stencilTestOptions
		{
			get { return m_stencilTestOptions; }
		}
		public Material stencilPassMaterial
		{
			get { return m_stencilPass; }
			set { m_stencilPass = value; }
		}

		private ProjectorForLWRP m_projectorForLWRP = null;
		private Material m_copiedMaterial = null;
		private ProjectorForLWRP parentProjector
		{
			get
			{
				return m_projectorForLWRP;
			}
		}
		public Material sharedMaterial
		{
			get { return m_material; }
			set { m_material = value; }
		}
		public Material material
		{
			get
			{
				Material originalMaterial = sharedMaterial;
#if UNITY_EDITOR
				if (originalMaterial == null)
				{
					originalMaterial = ProjectorForSRP.ProjectorForSRP.debugMaterial;
				}
#endif
#if UNITY_EDITOR || DEBUG
				if (ProjectorRendererFeature.replaceProjectorMaterialForDebug != null)
				{
					originalMaterial = ProjectorRendererFeature.replaceProjectorMaterialForDebug;
				}
#endif
				if (m_copiedMaterial == null)
				{
					parentProjector.CheckProjectorForLWRPKeyword(originalMaterial);
					m_copiedMaterial = new Material(originalMaterial);
				}
				else if (m_copiedMaterial.shader != originalMaterial.shader)
				{
					parentProjector.CheckProjectorForLWRPKeyword(originalMaterial);
					m_copiedMaterial.shader = originalMaterial.shader;
				}
				m_copiedMaterial.CopyPropertiesFromMaterial(originalMaterial);
				parentProjector.propertyBlock.CopyPropertiesToMaterial(m_copiedMaterial);
				return m_copiedMaterial;
			}
		}
		private ShaderTagId[] m_shaderTagIdList;
		public void UpdateShaderTagIdList()
		{
			if (m_shaderTagList == null || m_shaderTagList.Length == 0)
			{
				m_shaderTagIdList = parentProjector.defaultShaderTagIdList;
			}
			else
			{
				if (m_shaderTagIdList == null || m_shaderTagIdList.Length != m_shaderTagList.Length)
				{
					m_shaderTagIdList = new ShaderTagId[m_shaderTagList.Length];
				}
				for (int i = 0; i < m_shaderTagList.Length; ++i)
				{
					m_shaderTagIdList[i] = new ShaderTagId(m_shaderTagList[i]);
				}
			}
		}

		void Initialize()
		{
			m_projectorForLWRP = GetComponent<ProjectorForLWRP>();
			m_actionOnAddProjectorToRenderer = AddRenderer;
			UpdateShaderTagIdList();
		}

		void OnValidate()
		{
			if (m_projectorForLWRP == null)
			{
				Initialize();
			}
			else
			{
				UpdateShaderTagIdList();
			}
		}

		System.Action<Camera> m_actionOnAddProjectorToRenderer;
		void OnEnable()
		{
			if (m_projectorForLWRP == null)
			{
				Initialize();
			}
			parentProjector.onAddProjectorToRenderer += m_actionOnAddProjectorToRenderer;
		}

		void OnDisable()
		{
			parentProjector.onAddProjectorToRenderer -= m_actionOnAddProjectorToRenderer;
		}

		void AddRenderer(Camera camera)
		{
			CustomRendererPassManager.staticInstance.AddCustomRenderer(camera, this);
		}

		protected void GetDefaultDrawSettings(ref UnityEngine.Rendering.Universal.RenderingData renderingData, Material material, out DrawingSettings drawingSettings, out FilteringSettings filteringSettings, out RenderStateBlock renderStateBlock)
		{
			drawingSettings = new DrawingSettings(m_shaderTagIdList[0], new SortingSettings(renderingData.cameraData.camera));
			for (int i = 1; i < m_shaderTagIdList.Length; ++i)
			{
				drawingSettings.SetShaderPassName(i, m_shaderTagIdList[i]);
			}
			drawingSettings.overrideMaterial = material;
			drawingSettings.overrideMaterialPassIndex = 0;
			drawingSettings.enableDynamicBatching = renderingData.supportsDynamicBatching;
			drawingSettings.enableInstancing = material.enableInstancing;
			drawingSettings.perObjectData = perObjectData;

			// default render queue range is opaque. please change it before draw call if needed.
			filteringSettings = new FilteringSettings(new RenderQueueRange(renderQueueLowerBound, renderQueueUpperBound), ~parentProjector.projector.ignoreLayers);
			filteringSettings.renderingLayerMask = (uint)renderingLayerMask;

			renderStateBlock = parentProjector.GetDefaultRenderStateBlock(useStencilTest, m_stencilTestOptions);
		}

		Material m_runtimeStencilPassMaterial = null;
		public void Render(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
		{
			CullingResults cullingResults;
			if (!parentProjector.TryGetCullingResults(renderingData.cameraData.camera, out cullingResults))
			{
				return;
			}
			Material tempMaterial = material;
			// The keyword must be already enabled. It does not need to be enabled here.
			// EnableProjectorForLWRPKeyword(material);
			parentProjector.SetupProjectorMatrix(tempMaterial);

			if (useStencilTest)
			{
				if (m_runtimeStencilPassMaterial == null)
				{
					m_runtimeStencilPassMaterial = new Material(stencilPassMaterial);
				}
				parentProjector.WriteFrustumStencil(context, renderingData.cameraData.camera, m_runtimeStencilPassMaterial);
			}

			ProjectorForLWRP.SetupCullingResultsForRendering(ref renderingData, ref cullingResults, perObjectData);
			DrawingSettings drawingSettings;
			FilteringSettings filteringSettings;
			RenderStateBlock renderStateBlock;
			GetDefaultDrawSettings(ref renderingData, tempMaterial, out drawingSettings, out filteringSettings, out renderStateBlock);
			context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);

			if (useStencilTest)
			{
				parentProjector.ClearStencil(context, m_runtimeStencilPassMaterial, m_stencilTestOptions);
			}
		}
	}
}
