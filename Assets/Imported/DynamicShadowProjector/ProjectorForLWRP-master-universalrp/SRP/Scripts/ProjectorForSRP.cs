//
// ProjectorForSRP.cs
//
// Projector For SRP
//
// Copyright (c) 2020 NYAHOON GAMES PTE. LTD.
//

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace ProjectorForSRP
{
	/// <summary>
	/// Abstract class of Projector for Scriptable Render Pipeline
	///
	/// This class implements OnEnable, OnDisable, OnDestroy and OnValidate events.
	/// If the derived class needs these events, please override them and call the base class function.
	/// Instead of implementing OnEnable and OnDestroy, you can override Initialize and Cleanup function.
	/// Initialize is called when OnEnable is invoked for the first time and Cleanup is called from OnDestroy if already initialized.
	///
	/// The derived class must implement AddProjectorToRenderer(Camera camera) function,
	/// which registers the projector to your custom scriptable render pipeline.
	/// Then, the render pipline must render the projector in some way.
	/// The following code is a sample render function supposd to be implemented in the derived class.
	/// 
	/// void Render(ScriptableRenderContext context, Camera camera)
	/// {
	///		CullingResults cullingResults;
	///		if (!TryGetCullingResults(camera, out cullingResults))
	///		{
	///			return;
	///		}
	///		Material material = GetDuplicatedProjectorMaterial();
	///		EnableProjectorForLWRPKeyword(material); // Enable FSR_PROJECTOR_FOR_LWRP keyword
	///		SetupProjectorMatrix(material);
	///		
	///		DrawingSettings drawingSettings;
	///		FilteringSettings filteringSettings;
	///		GetDefaultDrawSettings(camera, material, out drawingSettings, out filteringSettings);
	///		
	///		// modify drawing setting if necessary
	///		// drawingSettings.enableDynamicBatching = false;   // the default value is true. change it false if necessary.
	///		// drawingSettings.perObjectData = m_perObjectData; // the default value is PerObjectData.None.
	///		
	///		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
	///	}
	///
	/// Also, the derived class must implement defaultShaderTagIdList property as folloes.
	///
	/// static private ShaderTagId[] s_defaultShaderTagIdList = null;
	/// public override ShaderTagId[] defaultShaderTagIdList
	/// {
	///		get {
	///			if (s_defaultShaderTagIdList == null) {
	///				s_defaultShaderTagIdList = new ShaderTagId[2];
	///				s_defaultShaderTagIdList[0] = new ShaderTagId("LightweightForward"); // render pipeline specific shader tag
	///				s_defaultShaderTagIdList[1] = new ShaderTagId("SRPDefaultUnlit");    // default Unlit shader tag
	///			}
	///			return s_defaultShaderTagIdList;
	///		}
	///	}
	/// 
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Projector))]
	public abstract class ProjectorForSRP : MonoBehaviour
	{
		// will be replaced with Unity defined RenderingLayerMask when available.
		[System.Flags]
		public enum RenderingLayerMask
		{
			Nothing = 0,
			Everything = -1,
			Layer1 = (1 << 0),
			Layer2 = (1 << 1),
			Layer3 = (1 << 2),
			Layer4 = (1 << 3),
			Layer5 = (1 << 4),
			Layer6 = (1 << 5),
			Layer7 = (1 << 6),
			Layer8 = (1 << 7),
			Layer9 = (1 << 8),
			Layer10 = (1 << 9),
			Layer11 = (1 << 10),
			Layer12 = (1 << 11),
			Layer13 = (1 << 12),
			Layer14 = (1 << 13),
			Layer15 = (1 << 14),
			Layer16 = (1 << 15),
			Layer17 = (1 << 16),
			Layer18 = (1 << 17),
			Layer19 = (1 << 18),
			Layer20 = (1 << 19),
			Layer21 = (1 << 20),
			Layer22 = (1 << 21),
			Layer23 = (1 << 22),
			Layer24 = (1 << 23),
			Layer25 = (1 << 24),
			Layer26 = (1 << 25),
			Layer27 = (1 << 26),
			Layer28 = (1 << 27),
			Layer29 = (1 << 28),
			Layer30 = (1 << 29),
			Layer31 = (1 << 30),
			Layer32 = (1 << 31),
		}
		// property block which can modify shader properties without changing shared material.
		public class PropertyBlock
		{
			public bool HasProperty(int id)
			{
				if (m_textureProperties != null && m_textureProperties.ContainsKey(id)) return true;
				if (m_floatProperties != null && m_floatProperties.ContainsKey(id)) return true;
				if (m_colorProperties != null && m_colorProperties.ContainsKey(id)) return true;
				if (m_vectorProperties != null && m_vectorProperties.ContainsKey(id)) return true;
				if (m_matrixProperties != null && m_matrixProperties.ContainsKey(id)) return true;
				return false;
			}
			public void CopyPropertiesToMaterial(Material material)
			{
				if (m_textureProperties != null)
				{
					foreach (var pair in m_textureProperties)
					{
						material.SetTexture(pair.Key, pair.Value);
					}
				}
				if (m_floatProperties != null)
				{
					foreach (var pair in m_floatProperties)
					{
						material.SetFloat(pair.Key, pair.Value);
					}
				}
				if (m_colorProperties != null)
				{
					foreach (var pair in m_colorProperties)
					{
						material.SetColor(pair.Key, pair.Value);
					}
				}
				if (m_vectorProperties != null)
				{
					foreach (var pair in m_vectorProperties)
					{
						material.SetVector(pair.Key, pair.Value);
					}
				}
				if (m_matrixProperties != null)
				{
					foreach (var pair in m_matrixProperties)
					{
						material.SetMatrix(pair.Key, pair.Value);
					}
				}
			}
			public void SetTexture(int id, Texture value)
			{
				if (m_textureProperties == null) m_textureProperties = new Dictionary<int, Texture>();
				m_textureProperties[id] = value;
			}
			public void SetFloat(int id, float value)
			{
				if (m_floatProperties == null) m_floatProperties = new Dictionary<int, float>();
				m_floatProperties[id] = value;
			}
			public void SetColor(int id, Color value)
			{
				if (m_colorProperties == null) m_colorProperties = new Dictionary<int, Color>();
				m_colorProperties[id] = value;
			}
			public void SetVector(int id, Vector4 value)
			{
				if (m_vectorProperties == null) m_vectorProperties = new Dictionary<int, Vector4>();
				m_vectorProperties[id] = value;
			}
			public void SetMatrix(int id, Matrix4x4 value)
			{
				if (m_matrixProperties == null) m_matrixProperties = new Dictionary<int, Matrix4x4>();
				m_matrixProperties[id] = value;
			}
			public Texture GetTexture(int id)
			{
				Texture value;
				if (m_textureProperties != null && m_textureProperties.TryGetValue(id, out value))
				{
					return value;
				}
				return null;
			}
			public float GetFloat(int id)
			{
				float value;
				if (m_floatProperties != null && m_floatProperties.TryGetValue(id, out value))
				{
					return value;
				}
				return 0;
			}
			public Color GetColor(int id)
			{
				Color value;
				if (m_colorProperties != null && m_colorProperties.TryGetValue(id, out value))
				{
					return value;
				}
				return new Color(0, 0, 0, 0);
			}
			public Vector4 GetVector(int id)
			{
				Vector4 value;
				if (m_vectorProperties != null && m_vectorProperties.TryGetValue(id, out value))
				{
					return value;
				}
				return Vector4.zero;
			}
			public Matrix4x4 GetMatrix(int id)
			{
				Matrix4x4 value;
				if (m_matrixProperties != null && m_matrixProperties.TryGetValue(id, out value))
				{
					return value;
				}
				return Matrix4x4.zero;
			}
			Dictionary<int, float> m_floatProperties;
			Dictionary<int, Color> m_colorProperties;
			Dictionary<int, Vector4> m_vectorProperties;
			Dictionary<int, Matrix4x4> m_matrixProperties;
			Dictionary<int, Texture> m_textureProperties;
		}
		// serialize field
		[Header("Receiver Object Filter")]
		public RenderingLayerMask renderingLayerMask = RenderingLayerMask.Everything;
		public int renderQueueLowerBound = RenderQueueRange.opaque.lowerBound;
		public int renderQueueUpperBound = RenderQueueRange.opaque.upperBound;
		public TerrainRenderFlags terrainRenderFlags = TerrainRenderFlags.Details | TerrainRenderFlags.Trees;
		public Terrain[] terrainsToBeFilteredWithRenderFlags;
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

		// get Unity Projector component
		public Projector projector { get; private set; }

		public Matrix4x4 localToProjectorTexcoordMatrix
		{
			get { return m_projectionMatrix; }
		}
		public Matrix4x4 worldToProjectorTexcoordMatrix
		{
			get
			{
				return m_projectionMatrix * transform.worldToLocalMatrix;
			}
		}
		public Vector3 localProjectorDirection
		{
			get { return Vector3.forward; }
		}
		public Vector3 worldProjectorDirection
		{
			get
			{
				return transform.forward;
			}
		}

		// set this to false if your scriptable render pipeline does not need culling results for rendering projectors.
		// for example, deferred rendering can render projectors without culling results.
		protected bool m_requiresCullingResult = true;

		// try to get the culling results. only valid within a frame after AddProjectorToRenderer called.
		internal bool TryGetCullingResults(Camera camera, out CullingResults cullingResults)
		{
			Debug.Assert(m_requiresCullingResult);
			return m_cullingResults.TryGetValue(camera, out cullingResults);
		}

		protected PropertyBlock m_propertyBlock;
		/// <summary>
		/// PropertyBlock can modify the properties of projector's material without changing the shared material.
		/// PropertyBlock is also applied to the material in AdditionalProjectorRenderer.
		/// </summary>
		public PropertyBlock propertyBlock
		{
			get
			{
				if (m_propertyBlock == null)
				{
					m_propertyBlock = new PropertyBlock();
				}
				return m_propertyBlock;
			}
		}
#if UNITY_EDITOR
		private static Material s_debugMaterial = null;
		public static Material debugMaterial
		{
			get
			{
				if (s_debugMaterial == null)
				{
					Shader shader = Shader.Find("Hidden/Projector For LWRP/Debug");
					if (shader != null)
					{
						string path = UnityEditor.AssetDatabase.GetAssetPath(shader);
						if (path != null && 6 < path.Length)
						{
							path = path.Substring(0, path.Length - 6); // remove "shader" extension
							path += "mat"; // add "mat" extension
							s_debugMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
						}
					}
				}
				return s_debugMaterial;
			}
		}
#endif
		private Material m_copiedProjectorMaterial = null;
		protected Material GetDuplicatedProjectorMaterial(Material originalMaterial = null)
		{
			if (originalMaterial == null)
			{
				originalMaterial = projector.material;
#if UNITY_EDITOR
				if (originalMaterial == null)
				{
					originalMaterial = debugMaterial;
				}
#endif
				if (originalMaterial == null)
				{
					Debug.LogError("Missing Projector Material!!! (" + name + ")", this);
					return null;
				}
			}
			if (m_copiedProjectorMaterial == null)
			{
				CheckProjectorForLWRPKeyword(originalMaterial);
				m_copiedProjectorMaterial = new Material(originalMaterial);
			}
			else if (m_copiedProjectorMaterial.shader != originalMaterial.shader)
			{
				CheckProjectorForLWRPKeyword(originalMaterial);
				m_copiedProjectorMaterial.shader = originalMaterial.shader;
			}
			m_copiedProjectorMaterial.CopyPropertiesFromMaterial(originalMaterial);
			if (m_propertyBlock != null)
			{
				m_propertyBlock.CopyPropertiesToMaterial(m_copiedProjectorMaterial);
			}
			return m_copiedProjectorMaterial;
		}

		internal void CheckProjectorForLWRPKeyword(Material material)
		{
#if UNITY_EDITOR
			if (!material.IsKeywordEnabled(PROJECTOR_SHADER_KEYWORD) && string.Compare(material.GetTag("CompatibleWithProjectorForLWRP", false), "True", true) != 0)
			{
				Debug.LogError(PROJECTOR_SHADER_KEYWORD + " is not enabled for " + material.name + " material! Please check 'Build for Universal RP' property of the material", this);
			}
#endif
		}
		protected void EnableProjectorForLWRPKeyword(Material material)
		{
			material.EnableKeyword(PROJECTOR_SHADER_KEYWORD);
		}

		internal void SetupProjectorMatrix(Material material)
		{
			material.SetMatrix(s_shaderPropIdFsrWorldToProjector, worldToProjectorTexcoordMatrix);
			material.SetVector(s_shaderPropIdFsrWorldProjectDir, worldProjectorDirection);
		}

		protected void GetDefaultDrawSettings(Camera camera, Material material, out DrawingSettings drawingSettings, out FilteringSettings filteringSettings)
		{
			drawingSettings = new DrawingSettings(m_shaderTagIdList[0], new SortingSettings(camera));
			for (int i = 1; i < m_shaderTagIdList.Length; ++i)
			{
				drawingSettings.SetShaderPassName(i, m_shaderTagIdList[i]);
			}
			drawingSettings.overrideMaterial = material;
			drawingSettings.overrideMaterialPassIndex = 0;
			drawingSettings.enableDynamicBatching = true; // default value is true. please change it before draw call if needed.
			drawingSettings.enableInstancing = material.enableInstancing;
			drawingSettings.perObjectData = PerObjectData.None; // default value is None. please change it before draw call if needed.

			// default render queue range is opaque. please change it before draw call if needed.
			filteringSettings = new FilteringSettings(new RenderQueueRange(renderQueueLowerBound, renderQueueUpperBound), ~projector.ignoreLayers);
			filteringSettings.renderingLayerMask = (uint)renderingLayerMask;
		}

		private ShaderTagId[] m_shaderTagIdList;
		public void UpdateShaderTagIdList()
		{
			if (m_shaderTagList == null || m_shaderTagList.Length == 0)
			{
				m_shaderTagIdList = defaultShaderTagIdList;
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

		//
		// functions to be overridden
		//

		public abstract ShaderTagId[] defaultShaderTagIdList { get; }

		/// <summary>
		/// This function is called when OnEnabled is invoked for the first time.
		/// 
		/// base.Initialize() must be called in the overriding function.
		/// Please be careful of the timing. Because OnProjectorFrustumChanged and defaultShaderTagIdList will be called from base.Initialize().
		/// If these function and property require some setups, they must be done before base.Initialize().
		///
		/// Initialize() will also be called when script files are re-compiled in Unity Editor to re-initialize non-serializable fields.
		/// In this case, serializable fields are already restored when Initialize() is called.
		/// Please take into account this fact when implementing Initiazlize function.
		/// 
		/// <seealso cref="Cleanup"/>
		/// <seealso cref="OnProjectorFrustumChanged"/>
		/// </summary>
		protected virtual void Initialize()
		{
			projector = GetComponent<Projector>();
			if (projector == null)
			{
				projector = gameObject.AddComponent<Projector>();
			}
			UpdateFrustum();
			m_projectorFrustumHash = CalculateProjectorFrustumHash(projector);
			UpdateShaderTagIdList();
		}

		/// <summary>
		/// This function is called from OnDestroy(), if Initialize() was called already.
		/// <seealso cref="Initialize"/>
		/// </summary>
		protected virtual void Cleanup()
		{
		}

		/// <summary>
		/// Register this projector to the render pipeline specific renderer.
		/// This function is called every frame if the projector frustum is visible from the camera.
		/// </summary>
		protected abstract void AddProjectorToRenderer(Camera camera);

		/// <summary>
		/// This function is called when projector frustum changed at the begining of frame rendering.
		/// Also this function is called from Initialize. Please finish necessary setups for this function befor base.Initialize called.
		///
		/// If you need a mesh of projector frustum for stencil test or deferred rendering,
		/// please use SetProjectorFrustumVerticesToMesh(mesh) function inside the ovrriding function.
		///
		/// <seealso cref="Initialize"/>
		/// <seealso cref="SetProjectorFrustumVerticesToMesh"/>
		/// </summary>
		protected virtual void OnProjectorFrustumChanged()
		{
		}

		// When Unity Editor re-compile script files,
		// serializable fields are serialized before OnDestroy, and they are restored after re-compile completed.
		// If there are non-serializable fields (for example, m_shaderTagIdList in ProjectorForLWRP class),
		// they will not be restored, and cause null reference exceptions.
		// To prevent such problems, make m_initialized non-serialized.
		// Initialize function must take into accout the fact that serializable fields might be restored already.
		[System.NonSerialized]
		private bool m_initialized = false;
		protected virtual void OnDestroy()
		{
			if (m_initialized)
			{
				Cleanup();
				m_initialized = false;
			}
		}

		protected virtual void OnEnable()
		{
			if (!m_initialized)
			{
				StaticInitialize();
				Initialize();
				m_initialized = true;
			}
			RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
		}

		protected virtual void OnDisable()
		{
			RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
		}

		protected virtual void OnValidate()
		{
			UpdateShaderTagIdList();
		}

		//
		// Helper functions
		//

		/// <summary>
		/// Create a mesh of the projector frustom which contains only vertex position attribute.
		/// Use this funtion in OnProjectorFrustumChanged() if necessary.
		/// </summary>
		protected void CreateProjectorFrustumMesh(Mesh mesh)
		{
			mesh.Clear();
			mesh.vertices = m_frustumVertices;
			mesh.SetTriangles(s_frustumTriangles, 0, false);
		}

		protected Vector3 GetProjectorFrustumVertex(int index)
		{
			return m_frustumVertices[index];
		}
		//
		// private functions
		//
		private Vector3[] m_frustumVertices = new Vector3[8];
		private Matrix4x4 m_projectionMatrix;

		private static bool s_isInitialized = false;
		private static int s_shaderPropIdFsrWorldToProjector = -1;
		private static int s_shaderPropIdFsrWorldProjectDir = -1;
		static readonly int[] s_frustumTriangles = {
			0, 1, 2, 2, 1, 3, // near plane
 			0, 4, 1, 1, 4, 5, // left
 			1, 5, 3, 3, 5, 7, // top
			3, 7, 2, 2, 7, 6, // right
			2, 6, 0, 0, 6, 4, // bottom
			6, 7, 4, 4, 7, 5  // far plane
		};

		private const string PROJECTOR_SHADER_KEYWORD = "FSR_PROJECTOR_FOR_LWRP";

		static protected void StaticInitialize()
		{
			if (!s_isInitialized)
			{
				s_shaderPropIdFsrWorldToProjector = Shader.PropertyToID("_FSRWorldToProjector");
				s_shaderPropIdFsrWorldProjectDir = Shader.PropertyToID("_FSRWorldProjectDir");
				s_isInitialized = true;
			}
		}

		static ProjectorForSRP()
		{
			StaticInitialize();
		}

		private ulong m_projectorFrustumHash = 0;
		private Dictionary<Camera, CullingResults> m_cullingResults;
		private void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}
			if (!projector.enabled)
			{
				return;
			}
			if (m_cullingResults == null)
			{
				m_cullingResults = new Dictionary<Camera, CullingResults>();
			}
			m_cullingResults.Clear();
			ulong hash = CalculateProjectorFrustumHash(projector);
			if (hash != m_projectorFrustumHash)
			{
				UpdateFrustum();
				m_projectorFrustumHash = hash;
			}
			for (int i = 0, count = cameras.Length; i < count; ++i)
			{
				Camera cam = cameras[i];
				if ((cam.cullingMask & (1 << gameObject.layer)) != 0)
				{
					if (StartCullingIfVisible(context, cam))
					{
						AddProjectorToRenderer(cam);
					}
				}
			}
		}
		static ulong CalculateProjectorFrustumHash(Projector projector)
		{
			ulong hash = (ulong)projector.nearClipPlane.GetHashCode();
			hash = (hash << 16) | (hash >> 48);
			if (projector.orthographic)
			{
				hash = (hash << 1) | (hash >> 63);
				hash ^= (ulong)projector.orthographicSize.GetHashCode();
			}
			else
			{
				hash ^= 0x1;
				hash = (hash << 1) | (hash >> 63);
				hash ^= (ulong)projector.fieldOfView.GetHashCode();
			}
			hash = (hash << 16) | (hash >> 48);
			hash ^= (ulong)projector.farClipPlane.GetHashCode();
			hash = (hash << 16) | (hash >> 48);
			hash ^= (ulong)projector.aspectRatio.GetHashCode();
			return hash;
		}

		private void UpdateFrustum()
		{
			float w, h;
			if (projector.orthographic)
			{
				h = projector.orthographicSize;
				w = h * projector.aspectRatio;
				m_frustumVertices[0].x = m_frustumVertices[1].x = m_frustumVertices[4].x = m_frustumVertices[5].x = -w;
				m_frustumVertices[2].x = m_frustumVertices[3].x = m_frustumVertices[6].x = m_frustumVertices[7].x = w;
				m_frustumVertices[0].y = m_frustumVertices[2].y = m_frustumVertices[4].y = m_frustumVertices[6].y = -h;
				m_frustumVertices[1].y = m_frustumVertices[3].y = m_frustumVertices[5].y = m_frustumVertices[7].y = h;
			}
			else
			{
				float tan = Mathf.Tan(0.5f * Mathf.Deg2Rad * projector.fieldOfView);
				h = tan * projector.farClipPlane;
				w = h * projector.aspectRatio;
				float nearH = tan * projector.nearClipPlane;
				float nearW = nearH * projector.aspectRatio;
				m_frustumVertices[0].x = m_frustumVertices[1].x = -nearW;
				m_frustumVertices[4].x = m_frustumVertices[5].x = -w;
				m_frustumVertices[2].x = m_frustumVertices[3].x = nearW;
				m_frustumVertices[6].x = m_frustumVertices[7].x = w;
				m_frustumVertices[0].y = m_frustumVertices[2].y = -nearH;
				m_frustumVertices[4].y = m_frustumVertices[6].y = -h;
				m_frustumVertices[1].y = m_frustumVertices[3].y = nearH;
				m_frustumVertices[5].y = m_frustumVertices[7].y = h;
			}
			m_frustumVertices[0].z = m_frustumVertices[1].z = m_frustumVertices[2].z = m_frustumVertices[3].z = projector.nearClipPlane;
			m_frustumVertices[4].z = m_frustumVertices[5].z = m_frustumVertices[6].z = m_frustumVertices[7].z = projector.farClipPlane;
			UpdateProjectionMatrix();
			OnProjectorFrustumChanged();
		}
		private class TemporaryData
		{
			public TemporaryData()
			{
				m_vertices = new Vector3[8];
				m_clipPlanes = new Plane[12];
			}
			public Vector3[] m_vertices;
			public Plane[] m_clipPlanes;
		};
		private TemporaryData m_temporaryData = null;
		private TerrainRenderFlags[] m_originalTerrainRenderFlags = null;
		private bool StartCullingIfVisible(ScriptableRenderContext context, Camera cam)
		{
			if (m_frustumVertices == null)
			{
				return false;
			}
			ScriptableCullingParameters cullingParameters = new ScriptableCullingParameters();
			if (!cam.TryGetCullingParameters(IsStereoEnabled(cam), out cullingParameters))
			{
				return false;
			}
			if (m_temporaryData == null)
			{
				m_temporaryData = new TemporaryData();
			}
			uint flags = 0xff;
			ulong flags64 = 0;
			for (int i = 0; i < 8; ++i)
			{
				Vector3 v = m_temporaryData.m_vertices[i] = transform.TransformPoint(m_frustumVertices[i]);
				uint f = 0;
				for (int j = 0; j < cullingParameters.cullingPlaneCount; ++j)
				{
					Plane plane = cullingParameters.GetCullingPlane(j);
					if (plane.GetDistanceToPoint(v) < 0)
					{
						f |= (1U << j);
					}
				}
				flags &= f;
				flags64 |= (((ulong)f) << (8 * i));
			}
			if (flags != 0)
			{
				// projector is not visible from the camera
				return false;
			}
			if (!m_requiresCullingResult)
			{
				return true;
			}
			uint cameraPlanes = 0;
			int planeCount = 0;
			// -x
			flags = (uint)((flags64 >> 0) & (flags64 >> 8) & (flags64 >> 32) & (flags64 >> 40)) & 0xFF;
			if (flags == 0)
			{
				m_temporaryData.m_clipPlanes[planeCount++] = new Plane(m_temporaryData.m_vertices[0], m_temporaryData.m_vertices[1], m_temporaryData.m_vertices[4]);
			}
			else
			{
				cameraPlanes |= flags;
			}
			// +x
			flags = (uint)((flags64 >> 16) & (flags64 >> 24) & (flags64 >> 48) & (flags64 >> 56)) & 0xFF;
			if (flags == 0)
			{
				m_temporaryData.m_clipPlanes[planeCount++] = new Plane(m_temporaryData.m_vertices[3], m_temporaryData.m_vertices[2], m_temporaryData.m_vertices[7]);
			}
			else
			{
				cameraPlanes |= flags;
			}
			// -y
			flags = (uint)((flags64 >> 0) & (flags64 >> 16) & (flags64 >> 32) & (flags64 >> 48)) & 0xFF;
			if (flags == 0)
			{
				m_temporaryData.m_clipPlanes[planeCount++] = new Plane(m_temporaryData.m_vertices[2], m_temporaryData.m_vertices[0], m_temporaryData.m_vertices[6]);
			}
			else
			{
				cameraPlanes |= flags;
			}
			// +y
			flags = (uint)((flags64 >> 8) & (flags64 >> 24) & (flags64 >> 40) & (flags64 >> 56)) & 0xFF;
			if (flags == 0)
			{
				m_temporaryData.m_clipPlanes[planeCount++] = new Plane(m_temporaryData.m_vertices[1], m_temporaryData.m_vertices[3], m_temporaryData.m_vertices[5]);
			}
			else
			{
				cameraPlanes |= flags;
			}
			// near
			flags = (uint)((flags64 >> 0) & (flags64 >> 8) & (flags64 >> 16) & (flags64 >> 24)) & 0xFF;
			if (flags == 0)
			{
				m_temporaryData.m_clipPlanes[planeCount++] = new Plane(m_temporaryData.m_vertices[0], m_temporaryData.m_vertices[2], m_temporaryData.m_vertices[1]);
			}
			else
			{
				cameraPlanes |= flags;
			}
			// far
			flags = (uint)((flags64 >> 32) & (flags64 >> 40) & (flags64 >> 48) & (flags64 >> 56)) & 0xFF;
			if (flags == 0)
			{
				m_temporaryData.m_clipPlanes[planeCount++] = new Plane(m_temporaryData.m_vertices[4], m_temporaryData.m_vertices[5], m_temporaryData.m_vertices[6]);
			}
			else
			{
				cameraPlanes |= flags;
			}
			int maxPlaneCount = ScriptableCullingParameters.maximumCullingPlaneCount;
			Debug.Assert(cullingParameters.cullingPlaneCount == 6);
			const int farPlaneIndex = 5;
			uint addedCameraPlanes = 0;
			for (int i = 0; i < cullingParameters.cullingPlaneCount && planeCount < maxPlaneCount; ++i)
			{
				if ((cameraPlanes & (1U << i)) != 0)
				{
					addedCameraPlanes |= 1U << i;
					m_temporaryData.m_clipPlanes[planeCount++] = cullingParameters.GetCullingPlane(i);
				}
			}
			if (farPlaneIndex < planeCount)
			{
				// keep the camera far clip plane unchanged so that Layer Culling Distances can be handled correctly.
				if ((addedCameraPlanes & (1U << farPlaneIndex)) != 0)
				{
					// already have the camera far clip plane in m_temporaryData.m_clipPlanes[planeCount - 1]
					int currentFarPlaneIndex = planeCount - 1;
					// we need the following lines between #else and #endif if farPlaneIndex != 5
#if true
					Debug.Assert(farPlaneIndex == cullingParameters.cullingPlaneCount - 1);
#else
					uint cameraPlaneFlags = addedCameraPlanes >> (farPlaneIndex + 1);
					while (cameraPlaneFlags != 0)
					{
						if ((cameraPlaneFlags & 1U) == 1U)
						{
							--currentFarPlaneIndex;
						}
					}
#endif
					if (currentFarPlaneIndex != farPlaneIndex)
					{
						Plane farPlane = m_temporaryData.m_clipPlanes[currentFarPlaneIndex];
						m_temporaryData.m_clipPlanes[currentFarPlaneIndex] = m_temporaryData.m_clipPlanes[farPlaneIndex];
						m_temporaryData.m_clipPlanes[farPlaneIndex] = farPlane;
					}
				}
				else
				{
					// check if we really need to care abount Layer Culling Distances
					bool useLayerCullingDistances = false;
					Vector3 cameraForward = cam.transform.forward;
					float defaultCullingDistance = cam.farClipPlane + Vector3.Dot(cameraForward, cam.transform.position);
					defaultCullingDistance *= 0.9999f;
					float maxCullingDistance = Vector3.Dot(m_temporaryData.m_vertices[0], cameraForward);
					for (int i = 1; i < 8; ++i)
					{
						maxCullingDistance = Mathf.Max(maxCullingDistance, Vector3.Dot(m_temporaryData.m_vertices[i], cameraForward));
					}
					maxCullingDistance = Mathf.Min(maxCullingDistance, defaultCullingDistance);
					int layerMask = cam.cullingMask & ~projector.ignoreLayers;
					for (int i = 0, endi = ScriptableCullingParameters.layerCount; i < endi && layerMask != 0; ++i, layerMask >>= 1)
					{
						if ((layerMask & 1) != 0)
						{
							float layerCullingDistance = cullingParameters.GetLayerCullingDistance(i);
							if (layerCullingDistance < maxCullingDistance)
							{
								useLayerCullingDistances = true;
								break;
							}
						}
					}
					if (useLayerCullingDistances)
					{
						// we need to care about Layer Culling Distances. so keep far plane unchanged
						// otherwise, projector might be drawn on invisible objects.
						if (planeCount < ScriptableCullingParameters.maximumCullingPlaneCount)
						{
							m_temporaryData.m_clipPlanes[planeCount++] = m_temporaryData.m_clipPlanes[farPlaneIndex];
						}
						else
						{
							m_temporaryData.m_clipPlanes[planeCount - 1] = m_temporaryData.m_clipPlanes[farPlaneIndex];
						}
						m_temporaryData.m_clipPlanes[farPlaneIndex] = cullingParameters.GetCullingPlane(farPlaneIndex);
					}
					else
					{
						// we don't need to care abount Layer Culling Distances, but need to SetCullingDistance
						// so that culling can work correctly.
						Vector3 farPlaneNormal = m_temporaryData.m_clipPlanes[farPlaneIndex].normal;
						float maxDistance = Vector3.Dot(farPlaneNormal, m_temporaryData.m_vertices[0]);
						for (int i = 1; i < 8; ++i)
						{
							maxDistance = Mathf.Min(maxDistance, Vector3.Dot(m_temporaryData.m_vertices[i], farPlaneNormal));
						}
						maxDistance = -maxDistance;
						for (int i = 0, endi = ScriptableCullingParameters.layerCount; i < endi; ++i)
						{
							cullingParameters.SetLayerCullingDistance(i, maxDistance);
						}
					}
				}
			}
#if DEBUG
			// make sure that farPlaneIndex is correct.
			Plane farClipPlane = cullingParameters.GetCullingPlane(farPlaneIndex);
			float farClipDistance = farClipPlane.GetDistanceToPoint(cam.transform.position);
			float error = Mathf.Abs(farClipDistance - cam.farClipPlane);
			float dirError = (farClipPlane.normal + cam.transform.forward).sqrMagnitude;
			// sometimes, dirError becomes very large. try to fix it...
			if (3.9f < dirError)
			{
				dirError = (farClipPlane.normal - cam.transform.forward).sqrMagnitude;
			}
			else if (1.4f < dirError)
			{
				dirError = (farClipPlane.normal + cam.transform.right).sqrMagnitude;
				if (3.9f < dirError)
				{
					dirError = (farClipPlane.normal - cam.transform.right).sqrMagnitude;
				}
				else if (1.4f < dirError)
				{
					dirError = (farClipPlane.normal + cam.transform.up).sqrMagnitude;
					if (3.9f < dirError)
					{
						dirError = (farClipPlane.normal - cam.transform.up).sqrMagnitude;
					}
				}
			}
			if (!(Mathf.Approximately(error, 0) && Mathf.Approximately(dirError, 0)))
			{
				for (int i = 0; i < 6; ++i)
				{
					if (i != farPlaneIndex)
					{
						Plane plane = cullingParameters.GetCullingPlane(i);
						float err = Mathf.Abs(cam.farClipPlane - plane.GetDistanceToPoint(cam.transform.position));
						float dirErr = (plane.normal + cam.transform.forward).sqrMagnitude;
						if (err < error)
						{
							Debug.LogAssertion("CullingPlane[" + i + "] has less error (" + err + ") than CullingPlane[faPlaneIndex] does (" + error + ").");
						}
						if (dirErr < dirError)
						{
							Debug.LogAssertion("CullingPlane[" + i + "] has less dir error (" + dirErr + ") than CullingPlane[faPlaneIndex] does (" + dirError + ").");
						}
					}
				}
			}
			// To avoid the error: Assertion failed on expression: 'params.cullingPlaneCount == kPlaneFrustumNum'
			while (planeCount < 6)
			{
				m_temporaryData.m_clipPlanes[planeCount] = cullingParameters.GetCullingPlane(planeCount);
				++planeCount;
			}
			planeCount = 6;
#endif
			cullingParameters.cullingPlaneCount = planeCount;
			for (int i = 0; i < planeCount; ++i)
			{
				cullingParameters.SetCullingPlane(i, m_temporaryData.m_clipPlanes[i]);
			}
			cullingParameters.cullingOptions &= ~(CullingOptions.NeedsReflectionProbes | CullingOptions.ShadowCasters);
			if (terrainsToBeFilteredWithRenderFlags != null && 0 < terrainsToBeFilteredWithRenderFlags.Length)
			{
				if (m_originalTerrainRenderFlags == null || m_originalTerrainRenderFlags.Length < terrainsToBeFilteredWithRenderFlags.Length)
				{
					m_originalTerrainRenderFlags = new TerrainRenderFlags[terrainsToBeFilteredWithRenderFlags.Length];
				}
				for (int i = 0; i < terrainsToBeFilteredWithRenderFlags.Length; ++i)
				{
					if (terrainsToBeFilteredWithRenderFlags[i] != null)
					{
						m_originalTerrainRenderFlags[i] = terrainsToBeFilteredWithRenderFlags[i].editorRenderFlags;
						terrainsToBeFilteredWithRenderFlags[i].editorRenderFlags &= terrainRenderFlags;
					}
				}
			}
			CullingResults cullingResults = context.Cull(ref cullingParameters);
			if (terrainsToBeFilteredWithRenderFlags != null && 0 < terrainsToBeFilteredWithRenderFlags.Length)
			{
				for (int i = 0; i < terrainsToBeFilteredWithRenderFlags.Length; ++i)
				{
					if (terrainsToBeFilteredWithRenderFlags[i] != null)
					{
						terrainsToBeFilteredWithRenderFlags[i].editorRenderFlags = m_originalTerrainRenderFlags[i];
					}
				}
			}
			m_cullingResults.Add(cam, cullingResults);
			return true;
		}

		static bool IsStereoEnabled(Camera camera)
		{
			bool isGameCamera = (camera.cameraType == CameraType.Game || camera.cameraType == CameraType.VR);
			bool isCompatWithXRDimension = true;
#if ENABLE_VR && ENABLE_VR_MODULE
            isCompatWithXRDimension &= (camera.targetTexture ? camera.targetTexture.dimension == UnityEngine.XR.XRSettings.deviceEyeTextureDimension : true);
#endif
			return XRGraphics.enabled && isGameCamera && (camera.stereoTargetEye == StereoTargetEyeMask.Both) && isCompatWithXRDimension;
		}

		private void UpdateProjectionMatrix()
		{
			Matrix4x4 matProjection;
			if (projector.orthographic)
			{
				float x = projector.aspectRatio * projector.orthographicSize;
				float y = projector.orthographicSize;
				matProjection = Matrix4x4.Ortho(x, -x, y, -y, projector.nearClipPlane, projector.farClipPlane);
			}
			else
			{
				matProjection = Matrix4x4.Perspective(projector.fieldOfView, projector.aspectRatio, projector.nearClipPlane, projector.farClipPlane);
			}
			matProjection.m00 *= -0.5f;
			matProjection.m02 += 0.5f * matProjection.m32;
			matProjection.m03 += 0.5f * matProjection.m33;
			matProjection.m11 *= -0.5f;
			matProjection.m12 += 0.5f * matProjection.m32;
			matProjection.m13 += 0.5f * matProjection.m33;
			float zScale = 1.0f / (projector.farClipPlane - projector.nearClipPlane);
			matProjection.m22 = zScale;
			matProjection.m23 = -zScale * projector.nearClipPlane;
			m_projectionMatrix = matProjection;
		}
	}
}
