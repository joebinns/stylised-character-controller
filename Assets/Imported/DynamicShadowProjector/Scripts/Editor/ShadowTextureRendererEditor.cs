//
// ShadowTextureRendererEditor.cs
//
// Dynamic Shadow Projector
//
// Copyright 2015 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//

using UnityEngine;
using UnityEditor;

namespace DynamicShadowProjector.Editor {
	[CustomEditor(typeof(ShadowTextureRenderer))]
	public class ShadowTextureRendererEditor : EditorBase {
		static bool s_showAdvancedOptions = false;
		static ShadowTextureRenderer s_lastInstance = null;
#if UNITY_2019_1_OR_NEWER
		[System.Flags]
		enum URPSupportState
		{
			UsingSRP = 1 << 0,
			UsingURP = 1 << 1,
			UsingLWRP = 1 << 2,
			HasProjectorForLWRP = 1 << 3,
			HasProjectorForLWRPComponent = 1 << 4,
			HasExtensionForLWRP = 1 << 5,
		}
		URPSupportState m_urpSupportStarte;
		System.Type m_projectorForLWRPType = null;
		static bool IsType(System.Type typeToCheck, string fullnameOfType)
		{
			while (typeToCheck != null)
			{
				if (typeToCheck.FullName == fullnameOfType)
				{
					return true;
				}
				typeToCheck = typeToCheck.BaseType;
			}
			return false;
		}
#endif
		SerializedProperty m_shadowColor;
		SerializedProperty m_textureWidth;
		SerializedProperty m_textureHeight;
		SerializedProperty m_multiSampling;
		SerializedProperty m_superSampling;
		SerializedProperty m_blurLevel;
		SerializedProperty m_blurSize;
		SerializedProperty m_singlePassMipmapBlur;
		SerializedProperty m_mipLevel;
		SerializedProperty m_mipmapBlurSize;
		SerializedProperty m_testViewClip;
		SerializedProperty m_camerasForViewClipTest;
		SerializedProperty m_blurFilter;
		SerializedProperty m_mipmapFalloff;
		SerializedProperty m_customMipmapFalloff;
		SerializedProperty m_blurShader;
		SerializedProperty m_downsampleShader;
		SerializedProperty m_copyMipmapShader;
		SerializedProperty m_preferredTextureFormats;
		void OnEnable()
		{
			ShadowTextureRenderer shadowRenderer = target as ShadowTextureRenderer;
			bool modified = false;
			if (shadowRenderer.blurShader == null) {
				shadowRenderer.blurShader = FindMaterial("DynamicShadowProjector/Blit/Blur");
				ClearMaterialProperties(shadowRenderer.blurShader);
				modified = true;
			}
			if (shadowRenderer.downsampleShader == null) {
				shadowRenderer.downsampleShader = FindMaterial("DynamicShadowProjector/Blit/Downsample");
				ClearMaterialProperties(shadowRenderer.downsampleShader);
				modified = true;
			}
			if (shadowRenderer.copyMipmapShader == null) {
				shadowRenderer.copyMipmapShader = FindMaterial("DynamicShadowProjector/Blit/CopyMipmap");
				ClearMaterialProperties(shadowRenderer.copyMipmapShader);
				modified = true;
			}
			if (shadowRenderer.eraseShadowShader == null) {
				shadowRenderer.eraseShadowShader = FindMaterial("DynamicShadowProjector/Blit/EraseShadow");
				ClearMaterialProperties(shadowRenderer.eraseShadowShader);
				modified = true;
			}
			Projector projector = shadowRenderer.GetComponent<Projector>();
			if (projector.material != null) {
				RemoveUnusedMaterialProperties(projector.material);
			}
			if (modified) {
				EditorUtility.SetDirty(shadowRenderer);
				serializedObject.Update();
			}
			if (s_lastInstance != shadowRenderer)
			{
				shadowRenderer.projectorCamera.hideFlags |= HideFlags.HideInInspector;
			}
			s_lastInstance = shadowRenderer;

			m_shadowColor = serializedObject.FindProperty("m_shadowColor");
			m_textureWidth = serializedObject.FindProperty("m_textureWidth");
			m_textureHeight = serializedObject.FindProperty("m_textureHeight");
			m_multiSampling = serializedObject.FindProperty("m_multiSampling");
			m_superSampling = serializedObject.FindProperty("m_superSampling");
			m_blurLevel = serializedObject.FindProperty("m_blurLevel");
			m_blurSize = serializedObject.FindProperty("m_blurSize");
			m_singlePassMipmapBlur = serializedObject.FindProperty("m_singlePassMipmapBlur");
			m_mipLevel = serializedObject.FindProperty("m_mipLevel");
			m_mipmapBlurSize = serializedObject.FindProperty("m_mipmapBlurSize");
			m_testViewClip = serializedObject.FindProperty("m_testViewClip");
			m_camerasForViewClipTest = serializedObject.FindProperty("m_camerasForViewClipTest");
			m_blurFilter = serializedObject.FindProperty("m_blurFilter");
			m_mipmapFalloff = serializedObject.FindProperty("m_mipmapFalloff");
			m_customMipmapFalloff = serializedObject.FindProperty("m_customMipmapFalloff");
			m_blurShader = serializedObject.FindProperty("m_blurShader");
			m_downsampleShader = serializedObject.FindProperty("m_downsampleShader");
			m_copyMipmapShader = serializedObject.FindProperty("m_copyMipmapShader");
			m_preferredTextureFormats = serializedObject.FindProperty("m_preferredTextureFormats");

#if UNITY_2019_1_OR_NEWER
			m_urpSupportStarte = 0;
			if (UnityEngine.Rendering.RenderPipelineManager.currentPipeline != null)
			{
				m_urpSupportStarte |= URPSupportState.UsingSRP;
				System.Type renderPipelineType = UnityEngine.Rendering.RenderPipelineManager.currentPipeline.GetType();
				bool isURP = IsType(renderPipelineType, "UnityEngine.Rendering.Universal.UniversalRenderPipeline");
				if (isURP)
				{
					m_urpSupportStarte |= URPSupportState.UsingURP;
				}
				else
				{
					isURP = IsType(renderPipelineType, "UnityEngine.Rendering.LWRP.LightweightRenderPipeline");
					if (isURP)
					{
						m_urpSupportStarte |= URPSupportState.UsingLWRP;
					}
				}
				if (isURP)
				{
					m_projectorForLWRPType = typeof(ShadowTextureRenderer).Assembly.GetType("ProjectorForLWRP.ProjectorForLWRP");
					if (m_projectorForLWRPType != null)
					{
						m_urpSupportStarte |= URPSupportState.HasProjectorForLWRP;
						if (shadowRenderer.TryGetComponent(m_projectorForLWRPType, out _))
						{
							m_urpSupportStarte |= URPSupportState.HasProjectorForLWRPComponent;
						}
					}
					System.Type rendererPassType = typeof(ShadowTextureRenderer).Assembly.GetType("DynamicShadowProjector.LWRP.RenderShadowTexturePass");
					if (rendererPassType != null)
					{
						m_urpSupportStarte |= URPSupportState.HasExtensionForLWRP;
					}
				}
			}
#endif
		}
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Online Document");
			if (GUILayout.Button("http://nyahoon.com/products/dynamic-shadow-projector/shadow-texture-renderer-component", hyperlinkTextState)) {
				Application.OpenURL("http://nyahoon.com/products/dynamic-shadow-projector/shadow-texture-renderer-component");
			}
			EditorGUILayout.EndHorizontal();
			bool isGUIEnabled = GUI.enabled;
			EditorGUILayout.PropertyField(m_shadowColor);
			EditorGUILayout.IntPopup(m_textureWidth, s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.IntPopup(m_textureHeight, s_textureSizeDisplayOption, s_textureSizeOption);
			EditorGUILayout.PropertyField(m_multiSampling);
			EditorGUILayout.PropertyField(m_superSampling);
			EditorGUILayout.IntPopup(m_blurLevel, s_blurLevelDisplayOption, s_blurLevelOption);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && 0 < m_blurLevel.intValue;
			ShadowTextureRenderer shadowRenderer = target as ShadowTextureRenderer;
			EditorGUILayout.Slider(m_blurSize, 1.0f, shadowRenderer.blurFilter == ShadowTextureRenderer.BlurFilter.Uniform ? 6.0f : 4.0f);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;
			EditorGUILayout.PropertyField(m_mipLevel);
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && 0 < m_mipLevel.intValue;
			EditorGUILayout.PropertyField(m_singlePassMipmapBlur);
			float maxBlurSize;
			if (m_singlePassMipmapBlur.boolValue) {
				maxBlurSize = 1.0f;
			}
			else {
				maxBlurSize = shadowRenderer.blurFilter == ShadowTextureRenderer.BlurFilter.Uniform ? 3.0f : 2.0f;
			}
			EditorGUILayout.Slider(m_mipmapBlurSize, 0.0f, maxBlurSize);
			if (shadowRenderer.GetComponent<MipmappedShadowFallback>() == null) {
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add Fallback Component")) {
					Undo.AddComponent<MipmappedShadowFallback>(shadowRenderer.gameObject);
				}
				EditorGUILayout.EndHorizontal();
				GUI.enabled = isGUIEnabled;
				--EditorGUI.indentLevel;
			}
			else if (m_mipLevel.intValue == 0) {
				GUI.enabled = isGUIEnabled;
				--EditorGUI.indentLevel;
				EditorGUILayout.BeginHorizontal();
				GUILayout.TextArea("Still has Mipmap Fallback Component!", errorTextStyle);
				if (GUILayout.Button("Remove the Component")) {
					Undo.DestroyObjectImmediate(shadowRenderer.GetComponent<MipmappedShadowFallback>());
				}
				EditorGUILayout.EndHorizontal();
			}
			else {
				GUI.enabled = isGUIEnabled;
				--EditorGUI.indentLevel;
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Don't update while out of view");
			m_testViewClip.boolValue = EditorGUILayout.Toggle(m_testViewClip.boolValue);
			EditorGUILayout.EndHorizontal();
			++EditorGUI.indentLevel;
			GUI.enabled = isGUIEnabled && m_testViewClip.boolValue;
			EditorGUILayout.PropertyField(m_camerasForViewClipTest, true);
			GUI.enabled = isGUIEnabled;
			--EditorGUI.indentLevel;

			s_showAdvancedOptions = GUILayout.Toggle(s_showAdvancedOptions, "Show Advanced Options");
			if (s_showAdvancedOptions) {
				++EditorGUI.indentLevel;
				EditorGUILayout.PropertyField(m_blurFilter);
				EditorGUILayout.PropertyField(m_mipmapFalloff);
				if (m_mipmapFalloff.intValue == (int)ShadowTextureRenderer.MipmapFalloff.Custom) {
					if (shadowRenderer.customMipmapFalloff != null && shadowRenderer.customMipmapFalloff.Length != m_customMipmapFalloff.arraySize) {
						serializedObject.Update();
						m_customMipmapFalloff = serializedObject.FindProperty("m_customMipmapFalloff");
					}
					EditorGUILayout.PropertyField(m_customMipmapFalloff, true);
				}
				float near = EditorGUILayout.FloatField("Near Clip Plane", shadowRenderer.cameraNearClipPlane);
				if (near != shadowRenderer.cameraNearClipPlane) {
					Undo.RecordObject(shadowRenderer.projectorCamera, "Inspector");
					shadowRenderer.cameraNearClipPlane = near;
				}
				bool bShowCamera = (shadowRenderer.projectorCamera.hideFlags & HideFlags.HideInInspector) == 0;
				bool newValue = EditorGUILayout.Toggle("Show Camera in Inspector", bShowCamera);
				if (bShowCamera != newValue) {
					if (newValue) {
						shadowRenderer.projectorCamera.hideFlags &= ~HideFlags.HideInInspector;
					}
					else {
						shadowRenderer.projectorCamera.hideFlags |= HideFlags.HideInInspector;
					}
				}
				EditorGUILayout.PropertyField(m_blurShader);
				EditorGUILayout.PropertyField(m_downsampleShader);
				EditorGUILayout.PropertyField(m_copyMipmapShader);
				EditorGUILayout.PropertyField(m_preferredTextureFormats, true);
				--EditorGUI.indentLevel;
			}
			serializedObject.ApplyModifiedProperties();
#if UNITY_2019_1_OR_NEWER
			if ((m_urpSupportStarte & URPSupportState.UsingSRP) != 0)
			{
				if ((m_urpSupportStarte & (URPSupportState.UsingURP | URPSupportState.UsingLWRP)) != 0)
				{
					if ((m_urpSupportStarte & URPSupportState.HasProjectorForLWRP) != 0)
					{
						if ((m_urpSupportStarte & URPSupportState.HasProjectorForLWRPComponent) == 0)
						{
							if (!shadowRenderer.TryGetComponent(m_projectorForLWRPType, out _))
							{
								GUILayout.TextArea("Please add Projector For LWRP component to use projector with LWRP/URP", errorTextStyle);
								if (GUILayout.Button("Add Projector For LWRP component"))
								{
									Undo.AddComponent(shadowRenderer.gameObject, m_projectorForLWRPType);
									m_urpSupportStarte |= URPSupportState.HasProjectorForLWRPComponent;
								}
							}
							else
							{
								m_urpSupportStarte |= URPSupportState.HasProjectorForLWRPComponent;
							}
						}
					}
					else
					{
						GUILayout.TextArea("Please install Projector For LWRP from Github to support LWRP/URP", errorTextStyle);
						string url, zipUrl;
						if ((m_urpSupportStarte & URPSupportState.UsingURP) != 0)
						{
							url = "https://github.com/nyahoon-games/ProjectorForLWRP";
							zipUrl = "https://github.com/nyahoon-games/ProjectorForLWRP/archive/master-universalrp.zip";
						}
						else
						{
							url = "https://github.com/nyahoon-games/ProjectorForLWRP/tree/master";
							zipUrl = "https://github.com/nyahoon-games/ProjectorForLWRP/archive/master.zip";
						}
						if (GUILayout.Button("Download Zip file"))
						{
							Application.OpenURL(zipUrl);
						}
						if (GUILayout.Button("Go to Github repository"))
						{
							Application.OpenURL(url);
						}
					}
					if ((m_urpSupportStarte & URPSupportState.HasExtensionForLWRP) == 0)
					{
						string url, zipUrl;
						if ((m_urpSupportStarte & URPSupportState.UsingURP) != 0)
						{
							url = "https://github.com/nyahoon-games/DynamicShadowProjectorExtensionForLWRP";
							zipUrl = "https://github.com/nyahoon-games/DynamicShadowProjectorExtensionForLWRP/archive/master-universalrp.zip";
						}
						else
						{
							url = "https://github.com/nyahoon-games/DynamicShadowProjectorExtensionForLWRP/tree/master";
							zipUrl = "https://github.com/nyahoon-games/DynamicShadowProjectorExtensionForLWRP/archive/master.zip";
						}
						GUILayout.TextArea("Please install Dynamic Shadow Projector Extension For LWRP from Github to support LWRP/URP", errorTextStyle);
						if (GUILayout.Button("Download Zip file"))
						{
							Application.OpenURL(zipUrl);
						}
						if (GUILayout.Button("Go to Github repository"))
						{
							Application.OpenURL(url);
						}
					}
				}
				else
				{
					GUILayout.TextArea("Scriptable Renderpipeline is not supported other than URP!", errorTextStyle);
				}
			}
#endif
		}
		private Material   m_blitMaterial;
		void OnSceneGUI()
		{
			ShadowTextureRenderer shadowRenderer = target as ShadowTextureRenderer;
			if (shadowRenderer.shadowTexture == null || shadowRenderer.GetComponent<Projector>().material == null) {
				return;
			}

			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			if (m_blitMaterial == null) {
				m_blitMaterial = FindMaterial("DynamicShadowProjector/Blit/Blit");
			}
			int mipLevel = shadowRenderer.mipLevel + 1;
			int w = shadowRenderer.textureWidth;
			int h = shadowRenderer.textureHeight;
			for (int i = 1; i < mipLevel; ++i) {
				if ((w >> i) <= 4 || (h >> i) <=4) {
					mipLevel = i;
					break;
				}
			}
			int displayWidth = 128;
			float mipBias = Mathf.Log(displayWidth/w)/Mathf.Log(2.0f);
			int displayHeight = h*displayWidth/w;
			int marginSize = 4;
			int windowWidth = mipLevel * (displayWidth + marginSize) + marginSize;
			int windowHeight = displayHeight + 2*GUI.skin.window.border.bottom + marginSize;
			int windowPosX = (int)(Screen.width/EditorGUIUtility.pixelsPerPoint) - windowWidth - 10;
			int windowPosY = (int)(Screen.height/EditorGUIUtility.pixelsPerPoint) - windowHeight - GUI.skin.window.border.top - 10;
			GUI.WindowFunction func = id => {
				if (Event.current.type.Equals(EventType.Repaint)) {
					int x = marginSize;
					int y = GUI.skin.window.border.top;
					for (int i = 0; i < mipLevel; ++i) {
						m_blitMaterial.SetFloat("_MipLevel", i);
						m_blitMaterial.SetFloat("_MipBias", mipBias + i);
						Graphics.DrawTexture(new Rect(x, y, displayWidth, displayHeight), shadowRenderer.shadowTexture, m_blitMaterial);
						x += displayWidth + marginSize;
					}
				}
			};
			GUI.Window(0, new Rect(windowPosX, windowPosY, windowWidth, windowHeight), func, "Shadow Texture");
		}
	}
}
