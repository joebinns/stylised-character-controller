//
// ProjectorMaterialPreprocessor.cs
//
// Projector For LWRP
//
// Copyright (c) 2020 NYAHOON GAMES PTE. LTD.
//

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ProjectorForLWRP
{
	public class ProjectorMaterialPreprocessor : IProcessSceneWithReport
	{
		public int callbackOrder { get { return 0; } }
		private void ProcessGameObject(GameObject gameObject, bool isLWRP)
		{
			Projector projector;
			ProjectorForLWRP projectorForLWRP;
			if (gameObject.TryGetComponent<Projector>(out projector) && gameObject.TryGetComponent<ProjectorForLWRP>(out projectorForLWRP))
			{
				if (projector.material != null)
				{
					if (isLWRP)
					{
						projector.material.EnableKeyword("FSR_PROJECTOR_FOR_LWRP");
					}
					else
					{
						projector.material.DisableKeyword("FSR_PROJECTOR_FOR_LWRP");
					}
				}
			}
			for (int i = 0; i < gameObject.transform.childCount; ++i)
			{
				ProcessGameObject(gameObject.transform.GetChild(i).gameObject, isLWRP);
			}
		}
		public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
		{
			bool isLWRP = GraphicsSettings.renderPipelineAsset is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
			var rootObjects = scene.GetRootGameObjects();
			foreach (var rootObj in rootObjects)
			{
				ProcessGameObject(rootObj, isLWRP);
			}
		}
	}
}

