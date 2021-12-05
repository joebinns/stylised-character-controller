//
// DrawTargetObjectEditor.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector.Editor {
	[CustomEditor(typeof(DrawTargetObject))]
	public class DrawTargetObjectEditor : EditorBase {
		private SerializedProperty m_target;
		private SerializedProperty m_renderChildren;
		private SerializedProperty m_layerMask;
		private SerializedProperty m_autoDetectHierarchyChanges;
		private SerializedProperty m_textureAlignment;
		private SerializedProperty m_followTarget;
		private SerializedProperty m_updateFunction;
		private SerializedProperty m_targetDirection;
		private SerializedProperty m_shadowShader;
		private SerializedProperty m_replacementShaders;
		void OnEnable()
		{
			DrawTargetObject component = target as DrawTargetObject;
			if (component.shadowShader == null && (component.replacementShaders == null || component.replacementShaders.Length == 0)) {
				component.shadowShader = FindMaterial("DynamicShadowProjector/Shadow/Opaque");
				component.replacementShaders = new DrawTargetObject.ReplaceShader[2];
				component.replacementShaders[0].renderType = "Transparent";
				component.replacementShaders[0].shader = Shader.Find("DynamicShadowProjector/Shadow/Transparent");
				component.replacementShaders[1].renderType = "TransparentCutout";
				component.replacementShaders[1].shader = Shader.Find("DynamicShadowProjector/Shadow/Transparent Cutout");
				serializedObject.Update();
				EditorUtility.SetDirty(component);
			}
			m_target = serializedObject.FindProperty("m_target");
			m_renderChildren = serializedObject.FindProperty("m_renderChildren");
			m_layerMask = serializedObject.FindProperty("m_layerMask");
			m_autoDetectHierarchyChanges = serializedObject.FindProperty("m_autoDetectHierarchyChanges");
			m_textureAlignment = serializedObject.FindProperty("m_textureAlignment");
			m_followTarget = serializedObject.FindProperty("m_followTarget");
			m_updateFunction = serializedObject.FindProperty("m_updateFunction");
			m_targetDirection = serializedObject.FindProperty("m_targetDirection");
			m_shadowShader = serializedObject.FindProperty("m_shadowShader");
			m_replacementShaders = serializedObject.FindProperty("m_replacementShaders");
	}
	static bool s_showAdvancedOptions = false;
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Online Document");
			if (GUILayout.Button("http://nyahoon.com/products/dynamic-shadow-projector/draw-target-object-component", hyperlinkTextState)) {
				Application.OpenURL("http://nyahoon.com/products/dynamic-shadow-projector/draw-target-object-component");
			}
			EditorGUILayout.EndHorizontal();
			bool isGUIEnabled = GUI.enabled;
			EditorGUILayout.PropertyField(m_target);
			EditorGUILayout.PropertyField(m_renderChildren);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && m_renderChildren.boolValue;
			EditorGUILayout.PropertyField(m_layerMask);
			EditorGUILayout.PropertyField(m_autoDetectHierarchyChanges);
			if (m_renderChildren.boolValue && m_autoDetectHierarchyChanges.boolValue)
			{
				GUI.enabled = false;
				EditorGUILayout.TextArea("This component will check the hierarchy under the Target object EVERY FRAME! Instead of enabling this property, you can call 'SetCommandBufferDirty' function of this component after you add/delete/activate/deactivate child objects from script.", normalTextStyle);
			}
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;
			EditorGUILayout.PropertyField(m_textureAlignment);
			bool bUpdate = m_textureAlignment.intValue != (int)DrawTargetObject.TextureAlignment.None;
			EditorGUILayout.PropertyField(m_followTarget);
			bUpdate = bUpdate || m_followTarget.boolValue;
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && bUpdate;
			EditorGUILayout.PropertyField(m_updateFunction);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;
			EditorGUILayout.PropertyField(m_targetDirection);

			s_showAdvancedOptions = GUILayout.Toggle(s_showAdvancedOptions, "Show Advanced Options");
			if (s_showAdvancedOptions) {
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_shadowShader);
				EditorGUILayout.PropertyField(m_replacementShaders, true);
				--EditorGUI.indentLevel;
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
