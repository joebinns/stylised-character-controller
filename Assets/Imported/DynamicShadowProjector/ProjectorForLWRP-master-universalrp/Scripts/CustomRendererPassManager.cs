//
// CustomRendererPassManager.cs
//
// Projector For LWRP
//
// Copyright (c) 2020 NYAHOON GAMES PTE. LTD.
//

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProjectorForLWRP
{
    public class CustomRendererPassManager
	{
		public static CustomRendererPassManager staticInstance { get; private set; }

		static CustomRendererPassManager()
		{
			staticInstance = new CustomRendererPassManager();
		}
		public CustomRendererPassManager()
		{
			RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
		}
		~CustomRendererPassManager()
		{
			RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
		}

		public void AddCustomRenderer(Camera camera, ICustomRenderer customRenderer)
		{
			CustomRendererPass pass = m_cameraToPassMap[camera][customRenderer.renderPassEvent];
			if (pass.rendererCount == 0)
			{
				ProjectorRendererFeature.AddRenderPass(camera, pass);
			}
			pass.AddRenderer(customRenderer);
		}

		private ObjectPool<ObjectPool<CustomRendererPass>.AutoClearMap<RenderPassEvent>>.Map<Camera> m_cameraToPassMap = new ObjectPool<ObjectPool<CustomRendererPass>.AutoClearMap<RenderPassEvent>>.Map<Camera>();
		protected void OnEndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			m_cameraToPassMap.Clear();
		}
	}
}
