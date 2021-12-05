//
// ProjectorEditor.cs
//
// Projector For LWRP
//
// Copyright (c) 2019 NYAHOON GAMES PTE. LTD.
//

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace ProjectorForLWRP
{
	[CustomEditor(typeof(ProjectorForLWRP))]
	public class ProjectorEditor : Editor
	{
		private SerializedProperty m_stencilPassProperty;
		private SerializedProperty m_stencilOptionProperty;
		private SerializedObject m_pipelineAssetObject;
		private ScriptableRendererData m_rendererData;
		private Projector m_projectorComponent;
		private GUIStyle m_errorTextStyle;
		private GUIStyle errorTextStyle
		{
			get
			{
				if (m_errorTextStyle == null)
				{
					m_errorTextStyle = new GUIStyle();
					m_errorTextStyle.richText = true;
					m_errorTextStyle.wordWrap = true;
					m_errorTextStyle.alignment = TextAnchor.MiddleCenter;
					GUIStyleState textStyle = new GUIStyleState();
					textStyle.textColor = Color.red;
					m_errorTextStyle.normal = textStyle;
					m_errorTextStyle.hover = textStyle;
					m_errorTextStyle.active = textStyle;
					m_errorTextStyle.focused = textStyle;
				}
				return m_errorTextStyle;
			}
		}
		private void OnEnable()
		{
			// It is disabled to edit Projector component in Unity 2020.3 or later.
			// Hide inspector GUI of Projector, and show Projector properties in this GUI.
			m_projectorComponent = ((Component)target).GetComponent<Projector>();
			if (m_projectorComponent != null)
			{
				m_projectorComponent.hideFlags |= HideFlags.HideInInspector;
			}

			m_stencilOptionProperty = null;
			m_stencilPassProperty = null;

			// check render pipeline asset
			m_pipelineAssetObject = null;
			PrepareURPAssetSerializedProperties();
		}
		private void PrepareURPAssetSerializedProperties()
		{
			if (m_pipelineAssetObject == null)
			{
				UniversalRenderPipelineAsset renderPipelineAsset = UniversalRenderPipeline.asset;
				if (renderPipelineAsset != null)
				{
					m_pipelineAssetObject = new SerializedObject(renderPipelineAsset);
				}
			}
			if (m_pipelineAssetObject != null && m_rendererData == null)
			{
				SerializedProperty rendererDataListProperty = m_pipelineAssetObject.FindProperty("m_RendererDataList");
				SerializedProperty defaultRendererIndexProperty = m_pipelineAssetObject.FindProperty("m_DefaultRendererIndex");
				if (defaultRendererIndexProperty.intValue < rendererDataListProperty.arraySize)
				{
					m_rendererData = rendererDataListProperty.GetArrayElementAtIndex(defaultRendererIndexProperty.intValue).objectReferenceValue as ScriptableRendererData;
				}
			}
		}
		private void OnDisable()
		{
			// Revert the hide flags so that Projector can be visible in Inspector when ProjectorForLWRP component is removed.
			// this will not make the scene dirty.
			ProjectorForLWRP projector = target as ProjectorForLWRP;
			if (projector != null && projector.projector != null)
			{
				projector.projector.hideFlags &= ~HideFlags.HideInInspector;
			}
		}
		public override void OnInspectorGUI()
		{
			ProjectorForLWRP projector = target as ProjectorForLWRP;
			DrawDefaultInspector();
			bool useStencil = EditorGUILayout.Toggle("Use Stencil Test", projector.useStencilTest);
			if (useStencil)
			{
				if (m_stencilOptionProperty == null)
				{
					m_stencilOptionProperty = serializedObject.FindProperty("m_stencilTestOptions");
				}
				EditorGUI.indentLevel++;
				bool clearStencil = (m_stencilOptionProperty.intValue & (int)ProjectorForLWRP.StencilTestOptions.ClearStencil) != 0;
				bool preventOverwriting = (m_stencilOptionProperty.intValue & (int)ProjectorForLWRP.StencilTestOptions.PreventOverwriting) != 0;
				bool newClearStencil = EditorGUILayout.Toggle("Clear stencil after draw", clearStencil);
				bool newPreventOverwriting = EditorGUILayout.Toggle("Prevent overwriting", preventOverwriting);
				if (clearStencil != newClearStencil)
				{
					if (newClearStencil)
					{
						m_stencilOptionProperty.intValue |= (int)ProjectorForLWRP.StencilTestOptions.ClearStencil;
					}
					else
					{
						m_stencilOptionProperty.intValue &= ~(int)ProjectorForLWRP.StencilTestOptions.ClearStencil;
					}
				}
				if (preventOverwriting != newPreventOverwriting)
				{
					if (newPreventOverwriting)
					{
						m_stencilOptionProperty.intValue |= (int)ProjectorForLWRP.StencilTestOptions.PreventOverwriting;
					}
					else
					{
						m_stencilOptionProperty.intValue &= ~(int)ProjectorForLWRP.StencilTestOptions.PreventOverwriting;
					}
				}
				EditorGUI.indentLevel--;
			}
			if (useStencil != projector.useStencilTest)
			{
				if (m_stencilPassProperty == null)
				{
					m_stencilPassProperty = serializedObject.FindProperty("m_stencilPass");
				}
				if (useStencil)
				{
					if (projector.stencilPassMaterial == null)
					{
						Shader stencilPassShader = Shader.Find("Hidden/ProjectorForLWRP/StencilPass");
						string path = AssetDatabase.GetAssetPath(stencilPassShader);
						path = path.Substring(0, path.Length - 6); // remove "shader" extension
						path += "mat"; // add "mat" extension
						m_stencilPassProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
					}
				}
				else
				{
					m_stencilPassProperty.objectReferenceValue = null;
				}
			}
			serializedObject.ApplyModifiedProperties();
			PrepareURPAssetSerializedProperties();
			bool isGUIEnabled = GUI.enabled;
			if (m_pipelineAssetObject == null)
			{
				EditorGUILayout.TextArea("Universal Render Pipeline is not used. The projector will not be rendered with the above settings.", errorTextStyle);
			}
			else if (m_rendererData == null)
			{
				EditorGUILayout.TextArea("No Forward Renderer found. Please check the Render Pipeline Asset below.", errorTextStyle);
				GUI.enabled = false;
				EditorGUILayout.ObjectField("Render Pipeline Asset", m_pipelineAssetObject.targetObject, typeof(UnityEngine.Rendering.RenderPipelineAsset), false);
				GUI.enabled = isGUIEnabled;
			}
			else
			{
				bool found = false;
				foreach (var rendererFeature in m_rendererData.rendererFeatures)
				{
					if (rendererFeature is ProjectorRendererFeature)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					EditorGUILayout.TextArea("Please Add 'Projector Renderer Feature' to the default renderer (" + m_rendererData.name + ").", errorTextStyle);
					GUI.enabled = false;
					EditorGUILayout.ObjectField("Current Default Renderer", m_rendererData, typeof(ScriptableRendererData), false);
					GUI.enabled = isGUIEnabled;
					if (GUILayout.Button("Add Projector Renderer Feature to the default renderer"))
					{
						ProjectorRendererFeature rendererFeature = CreateInstance<ProjectorRendererFeature>();
						rendererFeature.name = "Projector Renderer Feature";
						Undo.RegisterCreatedObjectUndo(rendererFeature, "Add Projector Renderer Feature");

						// Store this new effect as a sub-asset so we can reference it safely afterwards
						// Only when we're not dealing with an instantiated asset
						if (EditorUtility.IsPersistent(m_rendererData))
						{
							AssetDatabase.AddObjectToAsset(rendererFeature, m_rendererData);
						}
						AssetDatabase.TryGetGUIDAndLocalFileIdentifier(rendererFeature, out var guid, out long localId);

						SerializedObject rendererDataSerializedObject = new SerializedObject(m_rendererData);
						SerializedProperty rendererFeaturesProperty = rendererDataSerializedObject.FindProperty("m_RendererFeatures");
						SerializedProperty rendererFeaturesMapProperty = rendererDataSerializedObject.FindProperty("m_RendererFeatureMap");
						// Grow the list first, then add - that's how serialized lists work in Unity
						rendererFeaturesProperty.arraySize++;
						SerializedProperty componentProp = rendererFeaturesProperty.GetArrayElementAtIndex(rendererFeaturesProperty.arraySize - 1);
						componentProp.objectReferenceValue = rendererFeature;

						// Update GUID Map
						rendererFeaturesMapProperty.arraySize++;
						SerializedProperty guidProp = rendererFeaturesMapProperty.GetArrayElementAtIndex(rendererFeaturesMapProperty.arraySize - 1);
						guidProp.longValue = localId;
						rendererDataSerializedObject.ApplyModifiedProperties();

						if (EditorUtility.IsPersistent(m_rendererData))
						{
							EditorUtility.SetDirty(m_rendererData);
						}
					}
				}
			}

			// It is disabled to edit Projector component in Unity 2020.3 or later.
			// We need to show Projector properties in this GUI.
			if (m_projectorComponent != null)
			{
				s_showProjectorGUI = EditorGUILayout.BeginFoldoutHeaderGroup(s_showProjectorGUI, "Projector Properties");
				if (s_showProjectorGUI)
				{
					DrawUnityProjectorGUI();
				}
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
		static bool s_showProjectorGUI = true;
		SerializedObject m_serializedProjectorObject = null;
		SerializedProperty m_nearClipProperty = null;
		SerializedProperty m_farClipProperty = null;
		SerializedProperty m_fieldOfViewProperty = null;
		SerializedProperty m_aspectRatioProperty = null;
		SerializedProperty m_orthographicProperty = null;
		SerializedProperty m_orthographicSizeProperty = null;
		SerializedProperty m_materialProperty = null;
		SerializedProperty m_ignoreLayersProperty = null;
		private void DrawUnityProjectorGUI()
		{
			if (m_serializedProjectorObject == null)
			{
				m_serializedProjectorObject = new SerializedObject(m_projectorComponent);
				m_nearClipProperty = m_serializedProjectorObject.FindProperty("m_NearClipPlane");
				m_farClipProperty = m_serializedProjectorObject.FindProperty("m_FarClipPlane");
				m_fieldOfViewProperty = m_serializedProjectorObject.FindProperty("m_FieldOfView");
				m_aspectRatioProperty = m_serializedProjectorObject.FindProperty("m_AspectRatio");
				m_orthographicProperty = m_serializedProjectorObject.FindProperty("m_Orthographic");
				m_orthographicSizeProperty = m_serializedProjectorObject.FindProperty("m_OrthographicSize");
				m_materialProperty = m_serializedProjectorObject.FindProperty("m_Material");
				m_ignoreLayersProperty = m_serializedProjectorObject.FindProperty("m_IgnoreLayers");
			}
			EditorGUILayout.PropertyField(m_nearClipProperty);
			EditorGUILayout.PropertyField(m_farClipProperty);
			EditorGUILayout.PropertyField(m_orthographicProperty);
			if (m_orthographicProperty.boolValue)
			{
				EditorGUILayout.PropertyField(m_orthographicSizeProperty);
			}
			else
			{
				EditorGUILayout.PropertyField(m_fieldOfViewProperty);
			}
			EditorGUILayout.PropertyField(m_aspectRatioProperty);
			EditorGUILayout.PropertyField(m_materialProperty);
			EditorGUILayout.PropertyField(m_ignoreLayersProperty);
			m_serializedProjectorObject.ApplyModifiedProperties();
		}
	}
}
