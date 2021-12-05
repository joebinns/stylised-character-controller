//
// PropertyBlockForTransparentReceiverEditor.cs
//
// Dynamic Shadow Projector
//
// Copyright 2021 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DynamicShadowProjector.Editor
{
    [CustomEditor(typeof(PropertyBlockForTransparentReceiver))]
    public class PropertyBlockForTransparentReceiverEditor : EditorBase
    {
		SerializedProperty m_options;
		SerializedProperty m_mainTexture;
		SerializedProperty m_mainTextureST_x;
		SerializedProperty m_mainTextureST_y;
		SerializedProperty m_mainTextureST_z;
		SerializedProperty m_mainTextureST_w;
		SerializedProperty m_baseAlpha;
		SerializedProperty m_alphaCutoff;
		SerializedProperty m_mainTextureName;
		SerializedProperty m_baseColorName;
		SerializedProperty m_alphaCutoffName;
		SerializedProperty m_alphaCutoffKeyword;
		SerializedProperty m_cullModeName;
		Renderer m_renderer;
		Material m_material;
		Shader m_shader;
		List<string> m_texturePropertyNames;
		string[] m_texturePropertyDisplayNames;
		List<string> m_colorPropertyNames;
		string[] m_colorPropertyDisplayNames;
		List<string> m_rangePropertyNames;
		string[] m_rangePropertyDisplayNames;
		List<string> m_floatPropertyNames;
		string[] m_floatPropertyDisplayNames;
		GUIContent m_propertyNameLabel;
		GUIContent m_valueLabel;
		GUIContent m_keywordLabel;
		void UpdateMaterialPropertyList()
		{
			if (m_renderer != null && m_renderer.sharedMaterial != null)
			{
				if (m_material != m_renderer.sharedMaterial || m_shader != m_renderer.sharedMaterial.shader)
				{
					m_material = m_renderer.sharedMaterial;
					m_shader = m_material.shader;
					MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(new Material[] { m_material });
					m_texturePropertyNames = new List<string>();
					List<string> texturePropertyDisplayNames = new List<string>();
					m_colorPropertyNames = new List<string>();
					List<string> colorPropertyDisplayNames = new List<string>();
					m_rangePropertyNames = new List<string>();
					List<string> rangePropertyDisplayNames = new List<string>();
					m_floatPropertyNames = new List<string>();
					List<string> floatPropertyDisplayNames = new List<string>();
					foreach (MaterialProperty materialProperty in materialProperties)
					{
						if (materialProperty.type == MaterialProperty.PropType.Texture)
						{
							m_texturePropertyNames.Add(materialProperty.name);
							texturePropertyDisplayNames.Add(materialProperty.name + " (" + materialProperty.displayName + ")");
						}
						else if (materialProperty.type == MaterialProperty.PropType.Color)
						{
							m_colorPropertyNames.Add(materialProperty.name);
							colorPropertyDisplayNames.Add(materialProperty.name + " (" + materialProperty.displayName + ")");
						}
						else if (materialProperty.type == MaterialProperty.PropType.Range)
						{
							m_rangePropertyNames.Add(materialProperty.name);
							rangePropertyDisplayNames.Add(materialProperty.name + " (" + materialProperty.displayName + ")");
						}
						else if (materialProperty.type == MaterialProperty.PropType.Float)
						{
							m_floatPropertyNames.Add(materialProperty.name);
							floatPropertyDisplayNames.Add(materialProperty.name + " (" + materialProperty.displayName + ")");
						}
					}
					if (!m_texturePropertyNames.Contains(m_mainTextureName.stringValue))
					{
						m_texturePropertyNames.Add(m_mainTextureName.stringValue);
						texturePropertyDisplayNames.Add(m_mainTextureName + " (Not Exist)");
					}
					if (!m_colorPropertyNames.Contains(m_baseColorName.stringValue))
					{
						m_colorPropertyNames.Add(m_baseColorName.stringValue);
						colorPropertyDisplayNames.Add(m_baseColorName + " (Not Exist)");
					}
					if (!m_rangePropertyNames.Contains(m_alphaCutoffName.stringValue))
					{
						m_rangePropertyNames.Add(m_alphaCutoffName.stringValue);
						rangePropertyDisplayNames.Add(m_alphaCutoffName + " (Not Exist)");
					}
					if (!m_floatPropertyNames.Contains(m_cullModeName.stringValue))
					{
						m_floatPropertyNames.Add(m_cullModeName.stringValue);
						floatPropertyDisplayNames.Add(m_cullModeName.stringValue + " (Not Exist)");
					}
					m_texturePropertyDisplayNames = texturePropertyDisplayNames.ToArray();
					m_colorPropertyDisplayNames = colorPropertyDisplayNames.ToArray();
					m_rangePropertyDisplayNames = rangePropertyDisplayNames.ToArray();
					m_floatPropertyDisplayNames = floatPropertyDisplayNames.ToArray();
				}
			}
			else
			{
				m_material = null;
				m_shader = null;
				m_texturePropertyNames = null;
				m_texturePropertyDisplayNames = null;
				m_colorPropertyNames = null;
				m_colorPropertyDisplayNames = null;
				m_rangePropertyNames = null;
				m_rangePropertyDisplayNames = null;
			}
		}
		void OnEnable()
		{
			m_options = serializedObject.FindProperty("m_options");
			m_mainTexture = serializedObject.FindProperty("mainTexture");
			SerializedProperty mainTextureST = serializedObject.FindProperty("mainTextureST");
			m_mainTextureST_x = mainTextureST.FindPropertyRelative("x");
			m_mainTextureST_y = mainTextureST.FindPropertyRelative("y");
			m_mainTextureST_z = mainTextureST.FindPropertyRelative("z");
			m_mainTextureST_w = mainTextureST.FindPropertyRelative("w");
			m_baseAlpha = serializedObject.FindProperty("baseAlpha");
			m_alphaCutoff = serializedObject.FindProperty("alphaCutoff");
			m_mainTextureName = serializedObject.FindProperty("mainTextureName");
			m_baseColorName = serializedObject.FindProperty("baseColorName");
			m_alphaCutoffName = serializedObject.FindProperty("alphaCutoffName");
			m_alphaCutoffKeyword = serializedObject.FindProperty("alphaCutoffKeyword");
			m_cullModeName = serializedObject.FindProperty("cullModeName");
			m_renderer = ((Component)target).GetComponent<Renderer>();
			m_propertyNameLabel = new GUIContent("Property Name");
			m_valueLabel = new GUIContent("Value");
			m_keywordLabel = new GUIContent("Keyword");
			UpdateMaterialPropertyList();
		}

		static string[] s_sourceOptions = { "Material Property", "Value" };
		static string[] s_alphaCutoffOptions = { "Disable", "Enable", "Enable By Keyword" };
		public override void OnInspectorGUI()
		{
			UpdateMaterialPropertyList();
			PropertyBlockForTransparentReceiver.Options options = (PropertyBlockForTransparentReceiver.Options)m_options.intValue;
			bool copyMainTexFromMaterial = (options & PropertyBlockForTransparentReceiver.Options.CopyMainTexFromMaterial) != 0;
			bool copyBaseColorFromMaterial = (options & PropertyBlockForTransparentReceiver.Options.CopyBaseColorFromMaterial) != 0;
			bool copyAlphaCutoffFromMaterial = (options & PropertyBlockForTransparentReceiver.Options.CopyAlphaCutoffFromMaterial) != 0;
			bool enableAlphaCutoffByKeyword = (options & PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoffByKeyword) != 0;
			bool enableAlphaCutoff = (options & PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoff) != 0;
			bool copyCullModeFromMaterial = (options & PropertyBlockForTransparentReceiver.Options.CopyCullModeFromMaterial) != 0;

			EditorGUILayout.LabelField("Main Texture (which contains alpha of the surface)");
			++EditorGUI.indentLevel;
			copyMainTexFromMaterial = EditorGUILayout.Popup("Source", copyMainTexFromMaterial ? 0 : 1, s_sourceOptions) == 0;
			if (copyMainTexFromMaterial)
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.CopyMainTexFromMaterial) == 0)
				{
					m_options.intValue |= (int)PropertyBlockForTransparentReceiver.Options.CopyMainTexFromMaterial;
				}
				if (m_texturePropertyNames != null)
				{
					int index = m_texturePropertyNames.IndexOf(m_mainTextureName.stringValue);
					int newIndex = EditorGUILayout.Popup(m_propertyNameLabel, index, m_texturePropertyDisplayNames);
					if (index != newIndex)
					{
						m_mainTextureName.stringValue = m_texturePropertyNames[newIndex];
					}
				}
				else
				{
					EditorGUILayout.PropertyField(m_mainTextureName, m_propertyNameLabel);
					EditorGUILayout.TextArea("Missing Material!", errorTextStyle);
				}
			}
			else
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.CopyMainTexFromMaterial) != 0)
				{
					m_options.intValue &= (int)~PropertyBlockForTransparentReceiver.Options.CopyMainTexFromMaterial;
				}
				EditorGUILayout.PropertyField(m_mainTexture, m_valueLabel);
				Vector2 tiling = new Vector2(m_mainTextureST_x.floatValue, m_mainTextureST_y.floatValue);
				Vector2 offset = new Vector2(m_mainTextureST_z.floatValue, m_mainTextureST_w.floatValue);
				tiling = EditorGUILayout.Vector2Field("Tiling", tiling);
				offset = EditorGUILayout.Vector2Field("Offset", offset);
				if (tiling.x != m_mainTextureST_x.floatValue) m_mainTextureST_x.floatValue = tiling.x;
				if (tiling.y != m_mainTextureST_y.floatValue) m_mainTextureST_y.floatValue = tiling.y;
				if (offset.x != m_mainTextureST_z.floatValue) m_mainTextureST_z.floatValue = offset.x;
				if (offset.y != m_mainTextureST_w.floatValue) m_mainTextureST_w.floatValue = offset.y;
			}
			--EditorGUI.indentLevel;

			EditorGUILayout.LabelField("Base Alpha (Color property which contains alpha of the surface)");
			++EditorGUI.indentLevel;
			copyBaseColorFromMaterial = EditorGUILayout.Popup("Source", copyBaseColorFromMaterial ? 0 : 1, s_sourceOptions) == 0;
			if (copyBaseColorFromMaterial)
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.CopyBaseColorFromMaterial) == 0)
				{
					m_options.intValue |= (int)PropertyBlockForTransparentReceiver.Options.CopyBaseColorFromMaterial;
				}
				if (m_colorPropertyNames != null)
				{
					int index = m_colorPropertyNames.IndexOf(m_baseColorName.stringValue);
					int newIndex = EditorGUILayout.Popup(m_propertyNameLabel, index, m_colorPropertyDisplayNames);
					if (index != newIndex)
					{
						m_baseColorName.stringValue = m_colorPropertyNames[newIndex];
					}
				}
				else
				{
					EditorGUILayout.PropertyField(m_baseColorName, m_propertyNameLabel);
					EditorGUILayout.TextArea("Missing Material!", errorTextStyle);
				}
			}
			else
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.CopyBaseColorFromMaterial) != 0)
				{
					m_options.intValue &= (int)~PropertyBlockForTransparentReceiver.Options.CopyBaseColorFromMaterial;
				}
				EditorGUILayout.PropertyField(m_baseAlpha, m_valueLabel);
			}
			--EditorGUI.indentLevel;

			EditorGUILayout.LabelField("Cull Mode");
			++EditorGUI.indentLevel;
			copyCullModeFromMaterial = EditorGUILayout.Popup("Source", copyCullModeFromMaterial ? 0 : 1, s_sourceOptions) == 0;
			if (copyCullModeFromMaterial)
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.CopyCullModeFromMaterial) == 0)
				{
					m_options.intValue |= (int)PropertyBlockForTransparentReceiver.Options.CopyCullModeFromMaterial;
				}
				if (m_floatPropertyNames != null)
				{
					int index = m_floatPropertyNames.IndexOf(m_cullModeName.stringValue);
					int newIndex = EditorGUILayout.Popup(m_propertyNameLabel, index, m_floatPropertyDisplayNames);
					if (index != newIndex)
					{
						m_cullModeName.stringValue = m_floatPropertyNames[newIndex];
					}
				}
				else
				{
					EditorGUILayout.PropertyField(m_cullModeName, m_propertyNameLabel);
					EditorGUILayout.TextArea("Missing Material!", errorTextStyle);
				}
			}
			else
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.CopyCullModeFromMaterial) != 0)
				{
					m_options.intValue &= (int)~PropertyBlockForTransparentReceiver.Options.CopyCullModeFromMaterial;
				}
				PropertyBlockForTransparentReceiver.Options cullModeMask = (PropertyBlockForTransparentReceiver.Options.CullFront | PropertyBlockForTransparentReceiver.Options.CullBack);
				PropertyBlockForTransparentReceiver.Options cullMode = options & cullModeMask;
				PropertyBlockForTransparentReceiver.Options newCullMode = (PropertyBlockForTransparentReceiver.Options)EditorGUILayout.EnumPopup(m_valueLabel, (PropertyBlockForTransparentReceiver.CullMode)cullMode);
				if (cullMode != newCullMode)
				{
					m_options.intValue &= (int)~cullModeMask;
					m_options.intValue |= (int)newCullMode;
				}
			}
			--EditorGUI.indentLevel;

			int alphaCutoffOption = enableAlphaCutoffByKeyword ? 2 : enableAlphaCutoff ? 1 : 0;
			alphaCutoffOption = EditorGUILayout.Popup("Alpha Cutoff", alphaCutoffOption, s_alphaCutoffOptions);
			enableAlphaCutoff = (alphaCutoffOption == 1);
			enableAlphaCutoffByKeyword = (alphaCutoffOption == 2);
			++EditorGUI.indentLevel;
			if (enableAlphaCutoffByKeyword)
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoffByKeyword) == 0)
				{
					m_options.intValue |= (int)PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoffByKeyword;
				}
				EditorGUILayout.PropertyField(m_alphaCutoffKeyword, m_keywordLabel);
			}
			else
			{
				if ((options & PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoffByKeyword) != 0)
				{
					m_options.intValue &= (int)~PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoffByKeyword;
				}
				if (enableAlphaCutoff)
				{
					if ((options & PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoff) == 0)
					{
						m_options.intValue |= (int)PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoff;
					}
				}
				else
				{
					if ((options & PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoff) != 0)
					{
						m_options.intValue &= (int)~PropertyBlockForTransparentReceiver.Options.EnableAlphaCutoff;
					}
				}
			}
			--EditorGUI.indentLevel;

			if (enableAlphaCutoff || enableAlphaCutoffByKeyword)
			{
				EditorGUILayout.LabelField("Alpha Cutoff Threshold");
				++EditorGUI.indentLevel;
				copyAlphaCutoffFromMaterial = EditorGUILayout.Popup("Source", copyAlphaCutoffFromMaterial ? 0 : 1, s_sourceOptions) == 0;
				if (copyAlphaCutoffFromMaterial)
				{
					if ((options & PropertyBlockForTransparentReceiver.Options.CopyAlphaCutoffFromMaterial) == 0)
					{
						m_options.intValue |= (int)PropertyBlockForTransparentReceiver.Options.CopyAlphaCutoffFromMaterial;
					}
					if (m_rangePropertyNames != null)
					{
						int index = m_rangePropertyNames.IndexOf(m_alphaCutoffName.stringValue);
						int newIndex = EditorGUILayout.Popup(m_propertyNameLabel, index, m_rangePropertyDisplayNames);
						if (index != newIndex)
						{
							m_alphaCutoffName.stringValue = m_rangePropertyNames[newIndex];
						}
					}
					else
					{
						EditorGUILayout.PropertyField(m_baseColorName, m_propertyNameLabel);
						EditorGUILayout.TextArea("Material Not Found", errorTextStyle);
					}
				}
				else
				{
					if ((options & PropertyBlockForTransparentReceiver.Options.CopyAlphaCutoffFromMaterial) != 0)
					{
						m_options.intValue &= (int)~PropertyBlockForTransparentReceiver.Options.CopyAlphaCutoffFromMaterial;
					}
					EditorGUILayout.PropertyField(m_alphaCutoff, m_valueLabel);
				}
				--EditorGUI.indentLevel;
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
