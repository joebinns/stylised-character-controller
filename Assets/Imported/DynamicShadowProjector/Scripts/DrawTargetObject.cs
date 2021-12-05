//
// DrawTargetObject.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace DynamicShadowProjector {
#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
#else
	[ExecuteInEditMode]
#endif
	[RequireComponent(typeof(ShadowTextureRenderer))]
	public class DrawTargetObject : MonoBehaviour {
		[System.Serializable]
		public struct ReplaceShader {
			public string renderType;
			public Shader shader;
		}
		public enum TextureAlignment {
			None = 0,
			TargetAxisX,
			TargetAxisY,
			TargetAxisZ,
		}
		public enum UpdateFunction {
			OnPreCull = 0,
			LateUpdate,
			UpdateTransform,
		}
		// Serialize Fields
		[SerializeField]
		private Transform m_target;
		[SerializeField]
		private Transform m_targetDirection = null;
		[SerializeField]
		private LayerMask m_layerMask = -1;
		[SerializeField]
		private TextureAlignment m_textureAlignment = TextureAlignment.None;
		[SerializeField]
		private UpdateFunction m_updateFunction = UpdateFunction.OnPreCull;
		[SerializeField]
		private Material m_shadowShader;
		[SerializeField]
		private ReplaceShader[] m_replacementShaders;
		[SerializeField]
		private bool m_renderChildren = true;
		[SerializeField]
		private bool m_autoDetectHierarchyChanges = false;
		[SerializeField]
		private bool m_followTarget = true;

		// public properties
		public Transform target
		{
			get { return m_target; }
			set {
				if (m_target != value) {
					m_target = value;
					SetCommandBufferDirty();
				}
			}
		}
		public Transform targetDirection
		{
			get { return m_targetDirection; }
			set { m_targetDirection = value; }
		}
		public bool renderChildren
		{
			get { return m_renderChildren; }
			set {
				if (m_renderChildren != value) {
					m_renderChildren = value;
					SetCommandBufferDirty();
				}
			}
		}
		public bool autoDetectHierarchyChanges
		{
			get { return m_autoDetectHierarchyChanges; }
			set { m_autoDetectHierarchyChanges = value; }
		}
		public LayerMask layerMask
		{
			get { return m_layerMask; }
			set {
				if (m_layerMask != value) {
					m_layerMask = value;
					if (m_renderChildren) {
						SetCommandBufferDirty();
					}
				}
			}
		}
		public TextureAlignment textureAlignment
		{
			get { return m_textureAlignment; }
			set { m_textureAlignment = value; }
		}
		public UpdateFunction updateFunction
		{
			get { return m_updateFunction; }
			set { m_updateFunction = value; }
		}
		public bool followTarget
		{
			get { return m_followTarget; }
			set { m_followTarget = value; }
		}
		public Material shadowShader
		{
			get { return m_shadowShader; }
			set {
				if (m_shadowShader != value) {
					m_shadowShader = value;
					SetCommandBufferDirty();
				}
			}
		}
		public ReplaceShader[] replacementShaders
		{
			get { return m_replacementShaders; }
			set {
				m_replacementShaders = value;
				SetCommandBufferDirty();
			}
		}

		// public functions
		// Call SetCommandBufferDirty or UpdateCommandBuffer when child objects are added/deleted/disabled/enabled.
		public void SetCommandBufferDirty()
		{
			m_isCommandBufferDirty = true;
		}
		public void UpdateCommandBuffer()
		{
			if (m_target == null) {
				return;
			}
			m_commandBuffer.Clear();
			m_commandBufferHash.Init();
			AddDrawCommandForGameObject(m_target.gameObject, m_renderChildren, ref m_commandBufferHash, false);
			m_isCommandBufferDirty = false;
		}
		public void UpdateMaterial(Material mat)
		{
			if (m_replacedMaterialCache != null) {
				Material replacedMaterial;
				if (m_replacedMaterialCache.TryGetValue(mat, out replacedMaterial)) {
					replacedMaterial.CopyPropertiesFromMaterial(mat);
				}
			}
		}
		public void UpdateTransform()
		{
			if (m_textureAlignment != TextureAlignment.None || m_targetDirection != null) {
				// rotate projector to align texture parallel to target object.
				Vector3 up;
				switch (m_textureAlignment) {
				case TextureAlignment.TargetAxisX:
					up = m_target.right;
					break;
				case TextureAlignment.TargetAxisZ:
					up = m_target.forward;
					break;
				default:
					up = m_target.up;
					break;
				}
				Vector3 z = m_targetDirection != null ? m_targetDirection.forward : transform.forward;
				transform.LookAt(transform.position + z, up);
			}
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#if UNITY_2018_3_OR_NEWER
			if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
			{
				return;
			}
#endif
#endif
			if (m_followTarget) {
				Vector3 targetPos = transform.TransformPoint(m_localTargetPosition);
				transform.position += m_target.position - targetPos;
			}
		}

		// private fields
		private CommandBufferHash m_commandBufferHash = new CommandBufferHash();
		private bool m_isCommandBufferDirty;
		private CommandBuffer m_commandBuffer;
		private ShadowTextureRenderer m_shadowRenderer;
		private Vector3 m_localTargetPosition;
		private Dictionary<Material, Material> m_replacedMaterialCache;
		internal CommandBuffer commandBuffer { get { return m_commandBuffer; } }

		// message handlers
		void Awake()
		{
			m_shadowRenderer = GetComponent<ShadowTextureRenderer>();
			if (m_target != null) {
				m_localTargetPosition = transform.InverseTransformPoint(m_target.position);
			}
			CreateCommandBuffer();
		}

		void OnValidate()
		{
			if (m_commandBuffer != null) {
				UpdateCommandBuffer();
			}
		}

		void OnEnable()
		{
			if (m_commandBuffer == null) {
				CreateCommandBuffer();
			}
			else if (m_shadowRenderer != null && m_shadowRenderer.isProjectorVisible) {
				m_shadowRenderer.AddCommandBuffer(m_commandBuffer);
			}
		}

		void OnDisable()
		{
			if (m_shadowRenderer != null && m_commandBuffer != null) {
				m_shadowRenderer.RemoveCommandBuffer(m_commandBuffer);
			}
		}

		void OnDestroy()
		{
			if (m_commandBuffer != null) {
				m_commandBuffer.Dispose();
				m_commandBuffer = null;
			}
			if (m_replacedMaterialCache != null) {
				foreach (var pair in m_replacedMaterialCache) {
					DestroyImmediate(pair.Value);
				}
				m_replacedMaterialCache.Clear();
			}
		}

		void LateUpdate()
		{
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				if (IsTargetObjectTreeStructureChanged())
				{
					if (Application.isPlaying && !m_autoDetectHierarchyChanges)
					{
						Debug.LogWarning("The hierarchy of the target object changed but 'Auto Detect Hierarchy Changes' is not enabled. Please call SetCommandBufferDirty function after you add/delete/activate/deactivate child objects from script.", this);
					}
					SetCommandBufferDirty();
				}
			}
#endif
			if (m_autoDetectHierarchyChanges && IsTargetObjectTreeStructureChanged())
			{
				SetCommandBufferDirty();
			}

			if (m_updateFunction == UpdateFunction.LateUpdate) {
				UpdateTransform();
			}
		}

		internal void OnPreCull()
		{
			if (m_isCommandBufferDirty) {
				UpdateCommandBuffer();
			}
			if (m_updateFunction == UpdateFunction.OnPreCull) {
				UpdateTransform();
			}
		}

		void OnVisibilityChanged(bool isVisible)
		{
#if UNITY_EDITOR
			if (m_shadowRenderer == null) {
				m_shadowRenderer = GetComponent<ShadowTextureRenderer>();
			}
			if (m_commandBuffer == null) {
				CreateCommandBuffer();
				UpdateCommandBuffer();
				return;
			}
			if (m_isCommandBufferDirty) {
				UpdateCommandBuffer();
			}
#endif
			if (isVisible) {
				m_shadowRenderer.AddCommandBuffer(m_commandBuffer);
			}
			else {
				m_shadowRenderer.RemoveCommandBuffer(m_commandBuffer);
			}
		}

		// helper functions
		void CreateCommandBuffer()
		{
			m_commandBuffer = new CommandBuffer();
			if (m_shadowRenderer.isProjectorVisible) {
				m_shadowRenderer.AddCommandBuffer(m_commandBuffer);
			}
			m_isCommandBufferDirty = true;
		}
		bool IsTargetObjectTreeStructureChanged()
		{
			if (!m_isCommandBufferDirty)
			{
				CommandBufferHash hash = new CommandBufferHash();
				hash.Init();
				AddDrawCommandForGameObject(m_target.gameObject, m_renderChildren, ref hash, true);
				return !hash.Equals(m_commandBufferHash);
			}
			return false;
		}
		void AddDrawCommandForGameObject(GameObject obj, bool recursive, ref CommandBufferHash commandBufferHash, bool hashOnly)
		{
			if (!obj.activeSelf)
			{
				return;
			}
			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null && renderer.enabled)
			{
				if ((m_layerMask & (1 << renderer.gameObject.layer)) != 0 || !recursive) {
					commandBufferHash.AddRenderer(renderer);
					if (!hashOnly)
					{
						int materialCount = m_replacementShaders == null ? 0 : m_replacementShaders.Length;
						for (int i = -1; i < materialCount; ++i)
						{
							AddDrawCommand(renderer, i);
						}
					}
				}
			}
			else if ((!recursive) && (Debug.isDebugBuild || Application.isEditor))
			{
				Debug.LogError("The target object does not have a Renderer component!", m_target);
			}
			if (recursive)
			{
				Transform t = obj.transform;
				for (int i = 0, count = t.childCount; i < count; ++i)
				{
					AddDrawCommandForGameObject(t.GetChild(i).gameObject, recursive, ref commandBufferHash, hashOnly);
				}
			}
		}
		void AddDrawCommand(Renderer renderer, int renderTypeIndex)
		{
			Material[] materials = renderer.sharedMaterials;
			for (int i = 0; i < materials.Length; ++i) {
				Material m = materials[i];
				if (m == null) {
					Debug.LogWarning("The target object has a null material!", renderer);
					continue;
				}
				string renderType = m.GetTag("RenderType", false);
				if (3000 <= m.renderQueue || m.IsKeywordEnabled("_ALPHABLEND_ON") || m.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"))
				{
					renderType = "Transparent";
				}
				else if (m.IsKeywordEnabled("_ALPHATEST_ON"))
				{
					renderType = "TransparentCutout";
				}
				int foundIndex = -1;
				if (m_replacementShaders != null && !string.IsNullOrEmpty(renderType)) {
					for (int index = 0; index < m_replacementShaders.Length; ++index) {
						if (renderType == m_replacementShaders[index].renderType) {
							foundIndex = index;
							Shader shader = m_replacementShaders[index].shader;
							if (renderTypeIndex == index && shader != null) {
								if (m_replacedMaterialCache == null) {
									m_replacedMaterialCache = new Dictionary<Material, Material>();
								}
								Material replacedMaterial;
								if (!m_replacedMaterialCache.TryGetValue(m, out replacedMaterial)) {
									replacedMaterial = new Material(m);
									replacedMaterial.shader = shader;
									replacedMaterial.hideFlags = HideFlags.HideAndDontSave;
									m_replacedMaterialCache.Add(m, replacedMaterial);
								}
								else {
									replacedMaterial.CopyPropertiesFromMaterial(m);
									replacedMaterial.shader = shader;
								}
								m_commandBuffer.DrawRenderer(renderer, replacedMaterial, i);
							}
							break;
						}
					}
				}
				if (foundIndex == -1 && renderTypeIndex == -1) {
					m_commandBuffer.DrawRenderer(renderer, m_shadowShader, i);
				}
			}
		}
		private struct CommandBufferHash
		{
			System.UInt64 hash;
			public void Init()
			{
				hash = 0;
			}
			public void AddRenderer(Renderer renderer)
			{
				int instanceId = renderer.GetInstanceID();
				hash = (hash << 11) | (hash >> 53);
				hash ^= (System.UInt64)instanceId;
			}
			public bool Equals(CommandBufferHash rhs)
			{
				return hash == rhs.hash;
			}
		}
	}
}
