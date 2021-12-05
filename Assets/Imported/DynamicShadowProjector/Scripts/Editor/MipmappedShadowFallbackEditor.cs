//
// MipmappedShadowFallbackEditor.cs
//
// Fast Shadow Receiver
//
// Copyright 2019 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector.Editor
{
	[CustomEditor(typeof(MipmappedShadowFallback))]
	public class MipmappedShadowFallbackEditor : EditorBase
	{
		private bool m_testFallback = false;
		private int m_mipLevelBackup;
		private int m_blurLevelBackup;
		private float m_blurSizeBackup;
		private ShadowTextureRenderer.TextureMultiSample m_multiSamplingBackup;
		private ShadowTextureRenderer.TextureSuperSample m_superSamplingBackup;
		private int m_textureWidthBackup;
		private int m_textureHeightBackup;
		private Material m_projectorMaterialBackup;
		private Shader m_projectorShaderBackup;
		private Projector m_projector;
		private SerializedProperty m_fallbackShaderOrMaterial;
		private SerializedProperty m_blurLevel;
		private SerializedProperty m_blurSize;
		private SerializedProperty m_modifyTextureSize;
		private SerializedProperty m_textureWidth;
		private SerializedProperty m_textureHeight;
		private SerializedProperty m_multiSampling;
		private SerializedProperty m_superSampling;
		const string s_defaultFallbackShader = "DynamicShadowProjector/Projector/Shadow With Linear Falloff";
		void OnEnable()
		{
			MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
			m_projector = fallback.GetComponent<Projector>();
			bool modified = false;
			if (fallback.m_tex2DlodCheckShader == null)
			{
				fallback.m_tex2DlodCheckShader = Shader.Find("Hidden/DynamicShadowProjector/Caps/tex2Dlod");
				modified = true;
			}
			if (fallback.m_glslCheckShader == null)
			{
				fallback.m_glslCheckShader = Shader.Find("Hidden/DynamicShadowProjector/Caps/GLSL");
				modified = true;
			}
			if (fallback.m_fallbackShaderOrMaterial == null)
			{
				fallback.m_fallbackShaderOrMaterial = Shader.Find(s_defaultFallbackShader);
				modified = true;
			}
			if (modified)
			{
				EditorUtility.SetDirty(fallback);
				serializedObject.Update();
			}
			m_fallbackShaderOrMaterial = serializedObject.FindProperty("m_fallbackShaderOrMaterial");
			m_blurLevel = serializedObject.FindProperty("m_blurLevel");
			m_blurSize = serializedObject.FindProperty("m_blurSize");
			m_modifyTextureSize = serializedObject.FindProperty("m_modifyTextureSize");
			m_textureWidth = serializedObject.FindProperty("m_textureWidth");
			m_textureHeight = serializedObject.FindProperty("m_textureHeight");
			m_multiSampling = serializedObject.FindProperty("m_multiSampling");
			m_superSampling = serializedObject.FindProperty("m_superSampling");
		}
		void TestFallback()
		{
			if (!m_testFallback)
			{
				MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
				ShadowTextureRenderer shadowRenderer = fallback.GetComponent<ShadowTextureRenderer>();
				m_mipLevelBackup = shadowRenderer.mipLevel;
				m_blurLevelBackup = shadowRenderer.blurLevel;
				m_blurSizeBackup = shadowRenderer.blurSize;
				m_multiSamplingBackup = shadowRenderer.multiSampling;
				m_superSamplingBackup = shadowRenderer.superSampling;
				m_textureWidthBackup = shadowRenderer.textureWidth;
				m_textureHeightBackup = shadowRenderer.textureHeight;
				m_projectorShaderBackup = m_projector.material.shader;
				m_projectorMaterialBackup = m_projector.material;
				m_testFallback = true;
				fallback.ApplyFallback(m_projector);
				shadowRenderer.hideFlags |= HideFlags.NotEditable;
			}
		}
		void RestoreShadowTextureRenderer()
		{
			if (m_testFallback)
			{
				m_testFallback = false;
				MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
				if (fallback != null)
				{
					ShadowTextureRenderer shadowRenderer = fallback.GetComponent<ShadowTextureRenderer>();
					shadowRenderer.hideFlags &= ~HideFlags.NotEditable;
					shadowRenderer.mipLevel = m_mipLevelBackup;
					shadowRenderer.blurLevel = m_blurLevelBackup;
					shadowRenderer.blurSize = m_blurSizeBackup;
					shadowRenderer.multiSampling = m_multiSamplingBackup;
					shadowRenderer.superSampling = m_superSamplingBackup;
					shadowRenderer.textureWidth = m_textureWidthBackup;
					shadowRenderer.textureHeight = m_textureHeightBackup;
					m_projector.material = m_projectorMaterialBackup;
					m_projector.material.shader = m_projectorShaderBackup;
					m_testFallback = false;
				}
			}
		}
		void OnDisable()
		{
			RestoreShadowTextureRenderer();
		}
		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Online Document");
			if (GUILayout.Button("http://nyahoon.com/products/dynamic-shadow-projector/mipmapped-shadow-fallback-component", hyperlinkTextState))
			{
				Application.OpenURL("http://nyahoon.com/products/dynamic-shadow-projector/mipmapped-shadow-fallback-component");
			}
			EditorGUILayout.EndHorizontal();
			bool isGUIEnabled = GUI.enabled;
			Object newObject = EditorGUILayout.ObjectField("Fallback Shader/Material", m_fallbackShaderOrMaterial.objectReferenceValue, typeof(Object), false);
			if (newObject != m_fallbackShaderOrMaterial.objectReferenceValue)
			{
				if (newObject == null)
				{
					m_fallbackShaderOrMaterial.objectReferenceValue = Shader.Find(s_defaultFallbackShader);
				}
				else if (newObject is Shader || newObject is Material)
				{
					m_fallbackShaderOrMaterial.objectReferenceValue = newObject;
				}
			}
			EditorGUILayout.IntPopup(m_blurLevel, s_blurLevelDisplayOption, s_blurLevelOption);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && 0 < m_blurLevel.intValue;
			EditorGUILayout.Slider(serializedObject.FindProperty("m_blurSize"), 1.0f, 4.0f);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;

			EditorGUILayout.PropertyField(m_modifyTextureSize);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && m_modifyTextureSize.boolValue;
			EditorGUILayout.IntPopup(m_textureWidth, s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.IntPopup(m_textureHeight, s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.PropertyField(m_multiSampling);
			EditorGUILayout.PropertyField(m_superSampling);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;

			EditorGUILayout.Separator();
			if (m_projector.material != null)
			{
				bool test = GUILayout.Toggle(m_testFallback, "Test Fallback");
				if (test && !m_testFallback)
				{
					TestFallback();
				}
				else if (m_testFallback && !test)
				{
					RestoreShadowTextureRenderer();
				}
			}
			if (serializedObject.ApplyModifiedProperties())
			{
				if (m_testFallback)
				{
					MipmappedShadowFallback fallback = target as MipmappedShadowFallback;
					fallback.ApplyFallback(m_projector);
				}
			}
		}
	}
}
