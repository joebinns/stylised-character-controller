//
// CustomRendererPass.cs
//
// Projector For LWRP
//
// Copyright (c) 2019 NYAHOON GAMES PTE. LTD.
//

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace ProjectorForLWRP
{
    public class CustomRendererPass : ScriptableRenderPass
    {
		List<ICustomRenderer> m_customRenderers = new List<ICustomRenderer>();

		static CustomRendererPass()
		{
			ObjectPool<CustomRendererPass>.clearFunction = x => x.m_customRenderers.Clear();
		}
		public int rendererCount { get { return m_customRenderers.Count; } }
		public void AddRenderer(ICustomRenderer renderer)
		{
			Debug.Assert(m_customRenderers.Count == 0 || renderPassEvent == renderer.renderPassEvent);
			m_customRenderers.Add(renderer);
			renderPassEvent = renderer.renderPassEvent;
		}
		public bool isActive
		{
			get { return 0 < m_customRenderers.Count; }
		}
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			base.Configure(cmd, cameraTextureDescriptor);
		}
		static ProfilingSampler sampler = new ProfilingSampler("Projector For LWRP");
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get();
			using (new ProfilingScope(cmd, sampler))
			{
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();
				foreach (ICustomRenderer renderer in m_customRenderers)
				{
					renderer.Render(context, ref renderingData);
				}
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}
}
