//
// ProjectorForLWRP.cs
//
// Projector For LWRP
//
// Copyright (c) 2019 NYAHOON GAMES PTE. LTD.
//

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProjectorForLWRP
{
	[ExecuteAlways]
	[RequireComponent(typeof(Projector))]
	public class ProjectorForLWRP : ProjectorForSRP.ProjectorForSRP, ICustomRenderer
	{
		[System.Flags]
		public enum StencilTestOptions
		{
			None = 0,
			ClearStencil = 1,
			PreventOverwriting = 2,
		}
		// serialize field
		[Header("Projector Rendering")]
		[SerializeField]
		private RenderPassEvent m_renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
		[SerializeField]
		private PerObjectData m_perObjectData = PerObjectData.None;
		[SerializeField]
		[HideInInspector]
		private Material m_stencilPass = null;
		[SerializeField]
		[HideInInspector]
		private StencilTestOptions m_stencilTestOptions = StencilTestOptions.None;

		// public properties
		public RenderPassEvent renderPassEvent
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
		public StencilTestOptions stencilTestOptions
		{
			get { return m_stencilTestOptions; }
		}
		public Material stencilPassMaterial
		{
			get { return m_stencilPass; }
			set { m_stencilPass = value; }
		}

		static private ShaderTagId[] s_defaultShaderTagIdList = null;
		public override ShaderTagId[] defaultShaderTagIdList
		{
			get
			{
				if (s_defaultShaderTagIdList == null)
				{
					s_defaultShaderTagIdList = new ShaderTagId[2];
					s_defaultShaderTagIdList[0] = new ShaderTagId("UniversalForward");
					s_defaultShaderTagIdList[1] = new ShaderTagId("SRPDefaultUnlit");
				}
				return s_defaultShaderTagIdList;
			}
		}

		protected override void OnProjectorFrustumChanged()
		{
			m_meshFrustumDirty = true;
		}

		public void CopySerializedPropertiesFrom(ProjectorForLWRP src)
		{
			renderPassEvent = src.renderPassEvent;
			renderQueueLowerBound = src.renderQueueLowerBound;
			renderQueueUpperBound = src.renderQueueUpperBound;
			perObjectData = src.perObjectData;
			stencilPassMaterial = src.stencilPassMaterial;
		}

		private Mesh m_meshFrustum;
		private bool m_meshFrustumDirty = true;
		public Mesh GetFrustumMesh()
		{
			if (m_meshFrustum == null)
			{
				m_meshFrustum = new Mesh();
				m_meshFrustum.hideFlags = HideFlags.HideAndDontSave;
			}
			else if (!m_meshFrustumDirty)
			{
				return m_meshFrustum;
			}
			CreateProjectorFrustumMesh(m_meshFrustum);
			m_meshFrustumDirty = false;
			return m_meshFrustum;
		}

		private static bool s_isInitialized = false;
		private static int s_shaderPropIdStencilRef = -1;
		private static int s_shaderPropIdStencilMask = -1;
		static protected new void StaticInitialize()
		{
			if (!s_isInitialized)
			{
				ProjectorForSRP.ProjectorForSRP.StaticInitialize();
				s_shaderPropIdStencilRef = Shader.PropertyToID("P4LWRP_StencilRef");
				s_shaderPropIdStencilMask = Shader.PropertyToID("P4LWRP_StencilMask");
				s_isInitialized = true;
			}
		}

		static ProjectorForLWRP()
		{
			StaticInitialize();
		}

		internal event System.Action<Camera> onAddProjectorToRenderer;
		protected override void AddProjectorToRenderer(Camera camera)
		{
			CustomRendererPassManager.staticInstance.AddCustomRenderer(camera, this);
			onAddProjectorToRenderer?.Invoke(camera);
		}

		private static new void DestroyObject(Object obj)
		{
			if (obj != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(obj);
#else
				Destroy(obj);
#endif
			}
		}

		protected override void Cleanup()
		{
			if (m_meshFrustum != null)
			{
				DestroyObject(m_meshFrustum);
				m_meshFrustum = null;
			}
		}

		private enum StencilTestState
		{
			None = 0,
			BothSide = 1,
			BackfaceOnly = 2,
		}
		StencilTestState m_stencilTestState = StencilTestState.None;
		static Mesh s_fullScreenMesh = null;
		static Material s_fullScreenClearStencilMaterial = null;
		Material m_runtimeStencilPassMaterial = null;
		private void ClearFullscreenStencil(CommandBuffer commandBuffer)
		{
			if (s_fullScreenClearStencilMaterial == null)
			{
				s_fullScreenClearStencilMaterial = new Material(stencilPassMaterial);
			}
			s_fullScreenClearStencilMaterial.SetFloat(s_shaderPropIdStencilRef, StencilMaskAllocator.availableBits);
			s_fullScreenClearStencilMaterial.SetFloat(s_shaderPropIdStencilMask, StencilMaskAllocator.availableBits);
			if (s_fullScreenMesh == null)
			{
				s_fullScreenMesh = new Mesh();
				s_fullScreenMesh.vertices = new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(1, 1, 0) };
				s_fullScreenMesh.SetTriangles(new int[] { 0, 2, 1, 1, 2, 3 }, 0, false);
			}
			commandBuffer.DrawMesh(s_fullScreenMesh, Matrix4x4.identity, s_fullScreenClearStencilMaterial, 0, 4);
		}
		internal void WriteFrustumStencil(ScriptableRenderContext context, Camera camera, Material nonSharedStencilMaterial)
		{
			int stencilMask = StencilMaskAllocator.GetCurrentBit();
			if (stencilMask == 0)
			{
				m_stencilTestState = StencilTestState.None;
				Debug.LogError("Couldn't use stencil test. No stencil bits available. Please change Stencil Mask value in Projector Renderer Feature.");
				return;
			}

			// check if the frustum intersect with camera near plane.
			Vector3 cameraLocalPos = transform.InverseTransformPoint(camera.transform.position);
			Vector3 nearClipRect = camera.nearClipPlane * 2.0f * Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView) * new Vector3(camera.aspect, 1.0f, 0.0f);
			nearClipRect.z = camera.nearClipPlane;
			Matrix4x4 localToCamera = camera.worldToCameraMatrix * transform.localToWorldMatrix;
			bool clipped = true;
			for (int i = 0; i < 6; ++i)
			{
				int face = i >> 1;
				int n = i & 1;
				int axis1 = (face + 1 + n) % 3;
				int axis2 = (face + 2 - n) % 3;
				int vtx1 = n * (1 << face);
				int vtx2 = vtx1 + (1 << axis1);
				int vtx3 = vtx1 + (1 << axis2);
				Plane frustumPlane = new Plane(GetProjectorFrustumVertex(vtx1), GetProjectorFrustumVertex(vtx2), GetProjectorFrustumVertex(vtx3));
				Vector3 offset = Vector3.Scale(nearClipRect, localToCamera.MultiplyVector(frustumPlane.normal));
				if (Mathf.Abs(offset.x) + Mathf.Abs(offset.y) + Mathf.Max(0, offset.z) < frustumPlane.GetDistanceToPoint(cameraLocalPos))
				{
					clipped = false;
					break;
				}
			}
			CommandBuffer commandBuffer = CommandBufferPool.Get();
			if (StencilMaskAllocator.loopFlag)
			{
				StencilMaskAllocator.ClearLoopFlag();
				ClearFullscreenStencil(commandBuffer);
			}
			nonSharedStencilMaterial.SetFloat(s_shaderPropIdStencilRef, stencilMask);
			nonSharedStencilMaterial.SetFloat(s_shaderPropIdStencilMask, stencilMask);
			Mesh frustumMesh = GetFrustumMesh();
			if (clipped)
			{
				m_stencilTestState = StencilTestState.BackfaceOnly;
				commandBuffer.DrawMesh(frustumMesh, transform.localToWorldMatrix, nonSharedStencilMaterial, 0, 2);
			}
			else
			{
				m_stencilTestState = StencilTestState.BothSide;
				commandBuffer.DrawMesh(frustumMesh, transform.localToWorldMatrix, nonSharedStencilMaterial, 0, 0);
				commandBuffer.DrawMesh(frustumMesh, transform.localToWorldMatrix, nonSharedStencilMaterial, 0, 1);
			}
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);

		}
		internal void ClearStencil(ScriptableRenderContext context, Material nonSharedStencilMaterial, StencilTestOptions stencilTestOptions)
		{
			if (m_stencilTestState != StencilTestState.None)
			{
				if ((stencilTestOptions & StencilTestOptions.ClearStencil) == StencilTestOptions.ClearStencil)
				{
					CommandBuffer commandBuffer = CommandBufferPool.Get();
					if (m_stencilTestState == StencilTestState.BackfaceOnly)
					{
						if ((stencilTestOptions & StencilTestOptions.PreventOverwriting) == 0)
						{
							commandBuffer.DrawMesh(GetFrustumMesh(), transform.localToWorldMatrix, nonSharedStencilMaterial, 0, 1);
						}
						else
						{
							ClearFullscreenStencil(commandBuffer);
							StencilMaskAllocator.Init(StencilMaskAllocator.availableBits); // reset stencil allocator
						}
					}
					else
					{
						commandBuffer.DrawMesh(GetFrustumMesh(), transform.localToWorldMatrix, nonSharedStencilMaterial, 0, 3);
					}
					context.ExecuteCommandBuffer(commandBuffer);
					CommandBufferPool.Release(commandBuffer);
				}
				else
				{
					// allocate new stencil bit for the next projector
					StencilMaskAllocator.AllocateSingleBit();
				}
				m_stencilTestState = StencilTestState.None;
			}
		}
		protected void WriteFrustumStencil(ScriptableRenderContext context, Camera camera)
		{
			if (m_runtimeStencilPassMaterial == null)
			{
				m_runtimeStencilPassMaterial = new Material(stencilPassMaterial);
			}
			WriteFrustumStencil(context, camera, m_runtimeStencilPassMaterial);
		}
		protected void ClearStencil(ScriptableRenderContext context)
		{
			ClearStencil(context, m_runtimeStencilPassMaterial, m_stencilTestOptions);
		}
		internal static void SetupCullingResultsForRendering(ref RenderingData renderingData, ref CullingResults cullingResults, PerObjectData perObjectData)
		{
			if (0 < renderingData.lightData.additionalLightsCount && (perObjectData & PerObjectData.LightIndices) != 0)
			{
				cullingResults.visibleLights.CopyFrom(renderingData.cullResults.visibleLights);
				var lightIndexMap = renderingData.cullResults.GetLightIndexMap(Unity.Collections.Allocator.Temp);
				cullingResults.SetLightIndexMap(lightIndexMap);
				lightIndexMap.Dispose();
			}
			if ((perObjectData & PerObjectData.ReflectionProbes) != 0)
			{
				cullingResults.visibleReflectionProbes.CopyFrom(renderingData.cullResults.visibleReflectionProbes);
				var indexMap = renderingData.cullResults.GetReflectionProbeIndexMap(Unity.Collections.Allocator.Temp);
				cullingResults.SetReflectionProbeIndexMap(indexMap);
				indexMap.Dispose();
			}
		}
		internal RenderStateBlock GetDefaultRenderStateBlock(bool stencilTest, StencilTestOptions stencilTestOptions)
		{
			RenderStateBlock renderStateBlock = new RenderStateBlock();
			if (stencilTest && m_stencilTestState != StencilTestState.None)
			{
				int stencilMask = StencilMaskAllocator.GetCurrentBit();
				renderStateBlock.mask = RenderStateMask.Stencil;
				renderStateBlock.stencilReference = stencilMask;
				if (m_stencilTestState == StencilTestState.BothSide)
				{
					if ((stencilTestOptions & StencilTestOptions.PreventOverwriting) == 0)
					{
						renderStateBlock.stencilState = new StencilState(true, (byte)stencilMask, 0, CompareFunction.Equal, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
					}
					else
					{
						renderStateBlock.stencilState = new StencilState(true, (byte)stencilMask, (byte)stencilMask, CompareFunction.Equal, StencilOp.Zero, StencilOp.Keep, StencilOp.Keep);
					}
				}
				else
				{
					if ((stencilTestOptions & StencilTestOptions.PreventOverwriting) == 0)
					{
						renderStateBlock.stencilState = new StencilState(true, (byte)stencilMask, 0, CompareFunction.NotEqual, StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
					}
					else
					{
						renderStateBlock.stencilState = new StencilState(true, (byte)stencilMask, (byte)stencilMask, CompareFunction.NotEqual, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);
					}
				}
			}
			return renderStateBlock;
		}
		protected void GetDefaultDrawSettings(ref RenderingData renderingData, Material material, out DrawingSettings drawingSettings, out FilteringSettings filteringSettings, out RenderStateBlock renderStateBlock)
		{
			base.GetDefaultDrawSettings(renderingData.cameraData.camera, material, out drawingSettings, out filteringSettings);
			drawingSettings.enableDynamicBatching = renderingData.supportsDynamicBatching;
			drawingSettings.perObjectData = perObjectData;
			renderStateBlock = GetDefaultRenderStateBlock(useStencilTest, m_stencilTestOptions);
		}
		public virtual void Render(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CullingResults cullingResults;
			if (!TryGetCullingResults(renderingData.cameraData.camera, out cullingResults))
			{
				return;
			}
#if UNITY_EDITOR || DEBUG
			Material material;
			if (ProjectorRendererFeature.replaceProjectorMaterialForDebug != null)
			{
				material = GetDuplicatedProjectorMaterial(ProjectorRendererFeature.replaceProjectorMaterialForDebug);
			}
			else
			{
				material = GetDuplicatedProjectorMaterial();
			}
#else
			Material material = GetDuplicatedProjectorMaterial();
#endif
			if (material == null)
			{
				return;
			}
			// The keyword must be already enabled. It does not need to be enabled here.
			// EnableProjectorForLWRPKeyword(material);
			SetupProjectorMatrix(material);

			if (useStencilTest)
			{
				WriteFrustumStencil(context, renderingData.cameraData.camera);
			}

			SetupCullingResultsForRendering(ref renderingData, ref cullingResults, perObjectData);
			DrawingSettings drawingSettings;
			FilteringSettings filteringSettings;
			RenderStateBlock renderStateBlock;
			GetDefaultDrawSettings(ref renderingData, material, out drawingSettings, out filteringSettings, out renderStateBlock);
			context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);

			if (useStencilTest)
			{
				ClearStencil(context);
			}
		}
	}
}
